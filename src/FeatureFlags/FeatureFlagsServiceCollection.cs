using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags
{
    public class FeatureFlagsServiceCollection
    {
        public IServiceCollection Services { get; }

        public FeatureFlagsServiceCollection(IServiceCollection services)
        {
            Services = services;
        }
    }
}
