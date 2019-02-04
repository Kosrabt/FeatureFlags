using FeatureFlags.FeatureFlagProviders.Configuration;
using FeatureFlags.FeatureFlagProviders.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags
{
    public static class FeatureFlagsServiceCollectionExtensions
    {
        public static FeatureFlagsServiceCollection AddHttpHeaderFlags(this FeatureFlagsServiceCollection services)
        {
            services.Services.AddScoped<IFeatureFlagProvider, HttpRequestFeatureFlagProvider>();
            return services;
        }

        public static FeatureFlagsServiceCollection AddConfigurationFlags(this FeatureFlagsServiceCollection services, IConfigurationSection configurationSection)
        {
            services.Services.Configure<FeatureFlagOption>(configurationSection);
            services.Services.AddScoped<IFeatureFlagProvider, ConfigurationFeatureFlagProvider>();
            return services;
        }
    }
}
