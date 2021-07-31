using Blazored.LocalStorage;
using BlazorTable;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using PrimeView.JsonFileReader;
using System;
using System.Net.Http;
using System.Threading.Tasks;

using Constants = PrimeView.Frontend.Tools.Constants;

namespace PrimeView.Frontend
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");

			string baseAddress = builder.HostEnvironment.BaseAddress;

			builder.Services
				.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) })
				.AddBlazorTable()
				.AddBlazoredLocalStorage()
				.AddJsonFileReportReader(baseAddress, builder.Configuration.GetSection(Constants.Readers).GetSection(Constants.JsonFileReader))
				.AddSingleton(services => (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());

			await builder.Build().RunAsync();
		}
	}
}
