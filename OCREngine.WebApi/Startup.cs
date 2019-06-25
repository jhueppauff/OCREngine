using System;
using System.IO;
using System.Reflection;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;

namespace OCREngine.WebApi
{
    public class Startup
    {
        /// <summary>
        /// Swagger URL default
        /// </summary>
        private readonly string swaggerUrl;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
            LoggerFactory = loggerFactory;
            this.hosting = env;
            LoggerFactory.AddConsole(Configuration.GetSection("Logging"));
            Logger = LoggerFactory.CreateLogger<Startup>();

            this.swaggerUrl = string.IsNullOrEmpty(this.Configuration.GetValue<string>("SwaggerUrl")) ? "/swagger/v1/swagger.json" : this.Configuration.GetValue<string>("SwaggerUrl");

            Logger.Log(LogLevel.Trace, "Startup App");
        }

        public IConfiguration Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }

        private IHostingEnvironment hosting;

        public ILogger Logger { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oaut2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"https://login.microsoftonline.com/{Configuration.GetValue<string>("AzureAD:TenantId")}/oauth2/authorize",
                    Scopes = new Dictionary<string, string>
                    {
                      { "user_impersonation", "Access API" }
                    }
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new[] { "user_impersonation" } }
                });

                c.SwaggerDoc("v1", new Info
                {
                    Title = "OCR API",
                    Version = "v1",
                    TermsOfService = "None",
                    Description = "An API to consume the Azure Cognitive Services to work with OCR Detected Documents",
                    License = new License
                    {
                        Name = "Licensed under MIT",
                        Url = "https://github.com/jhueppauff/OCREngine/blob/master/LICENSE"
                    },
                    Contact = new Contact
                    {
                        Name = "Julian Hüppauff",
                        Email = string.Empty,
                        Url = "https://github.com/jhueppauff/OCREngine"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<IConfiguration>(Configuration);

            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();

            if (Environment.Is64BitProcess)
            {
                context.LoadUnmanagedLibrary(hosting.ContentRootPath + @"\x64\libwkhtmltox.dll");
                context.LoadUnmanagedLibrary(hosting.ContentRootPath + @"\x64\pdfium.dll");
            }
            else
            {
                context.LoadUnmanagedLibrary(hosting.ContentRootPath + @"\x86\libwkhtmltox.dll");
                context.LoadUnmanagedLibrary(hosting.ContentRootPath + @"\x86\pdfium.dll");
            }

            using (var variable = new PdfTools())
            {
                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(variable));
            }

            services.AddSingleton<ILoggerFactory>(this.LoggerFactory);
            services.AddCors(options => options.AddPolicy("CORSPolicy", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add Authentication
            services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
                            .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));


            services.AddMvc(
                   options =>
                   {
                       options.Filters.Add(new CorsAuthorizationFilterFactory("CORSPolicy"));
                   })
                   .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerUrl, "OCR API v1");

                c.OAuthClientId(Configuration.GetValue<string>("SwaggerClient:ClientId"));
                c.OAuthClientSecret(Configuration.GetValue<string>("SwaggerClient:ClientSecret"));
                c.OAuthRealm(Configuration.GetValue<string>("AzureAD:ClientId"));
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>() { { "resource", this.Configuration.GetValue<string>("AzureAD:ClientId") } });
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
