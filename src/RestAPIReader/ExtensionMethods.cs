using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeView.Entities;
using System;
using System.Net.Http;

namespace PrimeView.RestAPIReader
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddRestAPIReportReader(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var baseUrl = configuration.GetValue<string>(Constants.APIBaseURI) ?? string.Empty;

            serviceCollection.AddHttpClient(Constants.PrimesAPI, client => client.BaseAddress = new Uri(baseUrl));

            return serviceCollection.AddScoped<IReportReader, ReportReader>();
        }

    }
}
