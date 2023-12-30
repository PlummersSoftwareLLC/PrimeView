using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeView.Entities;

namespace PrimeView.RestAPIReader
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddRestAPIReportReader(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            return serviceCollection.AddScoped<IReportReader>(sp => new ReportReader(configuration));
        }

    }
}
