using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventWeb.App.Services;
using Microsoft.AspNetCore.Blazor.Builder;

namespace EventWeb.App
{
	public class Startup
	{

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
		{
			// Since Blazor is running on the server, we can use an application service
			// to read the forecast data.
			services.AddSingleton<WeatherForecastService>();
		}

        public void Configure(IBlazorApplicationBuilder app) =>
            // This is in all the (semi-old) samples.  App is not a type... wtf?
            app.AddComponent<App>("app");
    }
}
