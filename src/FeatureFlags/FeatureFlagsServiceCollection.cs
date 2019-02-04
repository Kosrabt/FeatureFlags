using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags
{
    public sealed class FeatureFlagsServiceCollection
    {
        public IServiceCollection Services { get; }

        public FeatureFlagsServiceCollection(IServiceCollection services)
        {
            Services = services;
        }
    }
}
