using Blazored.LocalStorage;
using BlazorTable;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
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
				.AddBlazoredLocalStorage()
				.AddStaticJsonReportReader()
				.AddSingleton(services => (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());

			await builder.Build().RunAsync();
		}
	}
}
