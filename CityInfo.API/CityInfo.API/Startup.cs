using CityInfo.API.Filters;
using CityInfo.API.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using System.Net;

namespace CityInfo.API
{
	public class Startup
	{
		public static IConfiguration Configuration { get; private set; }

		public Startup(IConfiguration configuration)
		{
			Startup.Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			// The following three lines add the configuration for FolderListerService / ListFolderController (API for testing Bio-Opt. test assignment).
			//IConfigurationSection localFolderOptions = Configuration.GetSection("LocalFolderOptions");
			//services.Configure<LocalFolderOptions>(localFolderOptions);
			//services.AddScoped<FolderListerService>();

			//IConfigurationSection mailOptions = Configuration.GetSection("mailSettings");
			//services.Configure<>

			// Setting the pre-2.2 version to prevent returning ProblemDetails instances for error status codes.
			// We are going to use UseStatusCodePages middleware instead.
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
				//.AddJsonOptions(o =>
				//{
				//	// We do not want the serialized properties start with a lower-case letter.
				//	if (o.SerializerSettings.ContractResolver != null)
				//	{
				//		var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
				//		castedResolver.NamingStrategy = null;
				//	}
				//});
				.AddMvcOptions(o =>
                {
                    o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    o.AllowEmptyInputInBodyModelBinding = true; // To make it possible to handle null [FromBody] model values ourselves.
                    o.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(mvcConfiguration => mvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>());

			services.AddLogging(loggingBuilder =>
			{
				loggingBuilder.AddNLog();
			});

#if DEBUG
			services.AddTransient<IMailService, LocalMailService>();
#else
			// It's no point to build the Release configuration because "CloudMailService" writes to the Debug output window.
			// "Debug.WriteLine()" statements are dropped out from the release build.
			services.AddTransient<IMailService, CloudMailService>();
#endif
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
				app.UseExceptionHandler(
					options =>
					{
						options.Run(async context =>
						{
							context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
							context.Response.ContentType = "text/html";
							var ex = context.Features.Get<IExceptionHandlerFeature>();
							if (ex != null)
							{
								var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace}";
								await context.Response.WriteAsync(err).ConfigureAwait(false);
							}
						});
					});
			}

			//app.Run((context) =>
			//{
			//	throw new Exception("Example exception.");
			//});

			//app.Run(async (context) =>
			//{
			//	await context.Response.WriteAsync("Hello World!");
			//});

			app.UseStatusCodePages();
			app.UseMvc();
		}
	}
}
