using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RaceTo21
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// Create a WebAssembly host generator that will build a WebAssembly version of the application
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			// Add the App component to the application as the root component, and specify that it will be rendered into an HTML element with ID "#app"
			builder.RootComponents.Add<App>("#app");
			// Add an instance of HttpClient with BaseAddress to the service container so the application can communicate with the remote API
			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
			// Add the AntDesign component library to the service container
			builder.Services.AddAntDesign();
			// Build the WebAssembly host and run the application
			await builder.Build().RunAsync();
		}
	}
}
