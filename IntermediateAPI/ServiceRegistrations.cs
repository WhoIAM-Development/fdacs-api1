using IntermediateAPI.Models;
using IntermediateAPI.Services;
using IntermediateAPI.Utilities;
using Microsoft.Extensions.Configuration;

namespace IntermediateAPI
{
    public static class ServiceRegistrations
    {
        public static void AddDataServices(this IServiceCollection services)
        {
            services.AddSingleton<HomeRealmService>();

            services.AddOptions<HomeRealmSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("HomeRealmSettings").Bind(settings);
            });
            services.AddSingleton(new MessagingUtility(Environment.GetEnvironmentVariable("ENVIRONMENT") == "Development"));
            services.AddHttpClient();
        }
    }
}
