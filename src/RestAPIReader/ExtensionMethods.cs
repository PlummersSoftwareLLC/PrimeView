using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeView.Entities;
using System.Net.Http;

namespace PrimeView.RestAPIReader
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddRestAPIReportReader(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            return serviceCollection.AddScoped<IReportReader>(sp => new ReportReader(sp.GetRequiredService<HttpClient>(), configuration));
        }

    }
}
