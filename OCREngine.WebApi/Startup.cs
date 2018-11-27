using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Hueppauff.Common.Extentions.KeyAuthentication;
using Hueppauff.Common.Extentions.KeyAuthentication.Events;
using Hueppauff.Common.Extentions.KeyAuthentication.Extentions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OCREngine.WebApi
{
    public class Startup
    {
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = KeyDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = KeyDefaults.AuthenticationScheme;
            })
                .AddKey(options =>
                {
                    options.Header = "Authorization";
                    options.HeaderKey = "ApiKey";
                    options.Events = new KeyEvents
                    {
                        OnAuthenticationFailed = httpContext =>
                        {
                            var ex = httpContext.Exception;

                            Trace.TraceError(ex.Message);

                            httpContext.Fail(ex);

                            return Task.CompletedTask;
                        },
                        OnKeyValidated = httpContext =>
                        {
                            if (httpContext.Key == "123")
                            {
                                httpContext.Principal = new ClaimsPrincipal();

                                httpContext.Success();
                            }
                            else if (httpContext.Key == "789")
                            {
                                throw new NotSupportedException("You must upgrade.");
                            }

                            return Task.CompletedTask;
                        }
                    };
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

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
