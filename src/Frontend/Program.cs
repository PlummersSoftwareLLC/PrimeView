using Blazored.LocalStorage;
using BlazorTable;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using PrimeView.JsonFileReader;
using PrimeView.RestAPIReader;
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
                .AddSingleton(services => (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());

            switch (builder.Configuration.GetValue<string>(Constants.ActiveReader))
            {
                case Constants.JsonFileReader:
                    builder.Services.AddJsonFileReportReader(baseAddress, builder.Configuration.GetSection(Constants.Readers).GetSection(Constants.JsonFileReader));
                    break;

                case Constants.RestAPIReader:
                    builder.Services.AddRestAPIReportReader(builder.Configuration.GetSection(Constants.Readers).GetSection(Constants.RestAPIReader));
                    break;
            }

            await builder.Build().RunAsync();
        }
    }
}
