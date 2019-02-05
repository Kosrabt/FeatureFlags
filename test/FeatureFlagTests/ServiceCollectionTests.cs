using FeatureFlags;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeatureFlagTests
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void FeatureFlagCanBeRegistered()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddFeatureFlags();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var featureFlagService = serviceProvider.GetRequiredService<IFeatureFlagService>();

            Assert.NotNull(featureFlagService);
        }

        [Fact]
        public void CustomServiceCollection_Is_Used()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var feautreServiceCollection = serviceCollection.AddFeatureFlags();

            Assert.IsType<FeatureFlagsServiceCollection>(feautreServiceCollection);
        }

        [Fact]
        public void ConfigurationFeatureFlagProviderCanBeAdded()
        {
            var configuration = new ConfigurationBuilder().Build();

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection
                .AddFeatureFlags()
                .AddConfigurationFlags(configuration.GetSection("dummy"));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var featureFlagService = serviceProvider.GetRequiredService<IFeatureFlagService>();

            Assert.NotNull(featureFlagService);
        }


        [Fact]
        public void HttpFeatureFlagProviderCanBeAdded()
        {          
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection
                .AddFeatureFlags()
                .AddHttpHeaderFlags("HeaderKey");

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var featureFlagService = serviceProvider.GetRequiredService<IFeatureFlagService>();

            Assert.NotNull(featureFlagService);
        }
    }
}
