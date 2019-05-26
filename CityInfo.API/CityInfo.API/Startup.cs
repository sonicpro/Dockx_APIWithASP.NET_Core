using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace CityInfo.API
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
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
				.AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter())
				);
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
				app.UseExceptionHandler();
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
