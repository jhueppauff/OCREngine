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

            LoggerFactory.AddConsole(Configuration.GetSection("Logging"));
            Logger = LoggerFactory.CreateLogger<Startup>();

            this.swaggerUrl = string.IsNullOrEmpty(this.Configuration.GetValue<string>("SwaggerUrl")) ? "/swagger/v1/swagger.json" : this.Configuration.GetValue<string>("SwaggerUrl");

            Logger.Log(LogLevel.Trace, "Startup App");
        }

        public IConfiguration Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }

        public ILogger Logger { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddSwaggerGen(c =>
            {
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
            context.LoadUnmanagedLibrary(Environment.CurrentDirectory + @"\libwkhtmltox.dll");
            context.LoadUnmanagedLibrary(Environment.CurrentDirectory + @"\pdfium.dll");

            using (var variable = new PdfTools())
            {
                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(variable));
            }

            services.AddSingleton<ILoggerFactory>(this.LoggerFactory);
            services.AddCors(options => options.AddPolicy("CORSPolicy", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddApplicationInsightsTelemetry(Configuration);

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
                LoggerFactory.AddDebug(LogLevel.Debug);
            }
            else
            {
                LoggerFactory.AddDebug(LogLevel.Error);
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
            });

            app.UseMvc();
        }
    }
}
