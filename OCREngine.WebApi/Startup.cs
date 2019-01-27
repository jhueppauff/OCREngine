using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Hueppauff.Common.Extentions.KeyAuthentication;
using Hueppauff.Common.Extentions.KeyAuthentication.Events;
using Hueppauff.Common.Extentions.KeyAuthentication.Extentions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace OCREngine.WebApi
{
    public class Startup
    {
        private const string swaggerUrl = "/swagger/v1/swagger.json";

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            LoggerFactory = loggerFactory;

            LoggerFactory.AddConsole(Configuration.GetSection("Logging"));
            Logger = LoggerFactory.CreateLogger<Startup>();

            Logger.Log(LogLevel.Trace, "Startup App");
        }

        public IConfiguration Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }

        public ILogger Logger { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "OCR API",
                    Version = "v1",
                    Description = "An API to consume the Azure Cognitive Services to work with OCR Detected Documents",
                    TermsOfService = "None",
                    License = new License
                    {
                        Name = "MIT License",
                        Url = "https://github.com/jhueppauff/OCREngine/blob/master/LICENSE"
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

            using (var variable = new PdfTools())
            {
                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(variable));
            }

            services.AddApplicationInsightsTelemetry(Configuration);
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

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerUrl, "OCR API V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
