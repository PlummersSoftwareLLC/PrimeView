using BlazorTable;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PrimeView.StaticJsonReader;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PrimeView.Frontend
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");

			builder.Services
				.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
				.AddBlazorTable()
				.AddStaticJsonReportReader();

			await builder.Build().RunAsync();
		}
	}
}