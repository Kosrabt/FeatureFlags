using System;
using System.Collections.Generic;
using System.Text;
using FeatureFlags;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeatureFlagTests
{
    public class AcceptenceTests
    {
        [Fact]
        public void EmptyConfiguration_Should_ReturnFalse()
        {
            var testFlagName = "dummyFlag";
            var configSectionName = "featureFlags";
            var config = new Dictionary<string, string>(){};

            IFeatureFlagService featureFlagService = GetConfigBasedFlagService(config, configSectionName);

            Assert.False(featureFlagService.IsEnabled(testFlagName));
        }

        [Fact]
        public void ExistingFlag_Should_ReturnTrue()
        {
            var testFlagName = "dummyFlag";
            var configSectionName = "featureFlags";
            var config = new Dictionary<string, string>()
            {
                [$"{configSectionName}:{testFlagName}:Enabled"] = "true",
            };

            IFeatureFlagService featureFlagService = GetConfigBasedFlagService(config, configSectionName);

            Assert.True(featureFlagService.IsEnabled(testFlagName));
        }

        [Fact]
        public void ExistingDisabledFlag_Should_ReturnFalse()
        {
            var testFlagName = "dummyFlag";
            var configSectionName = "featureFlags";
            var config = new Dictionary<string, string>()
            {
                [$"{configSectionName}:0:Name"] = testFlagName,
                [$"{configSectionName}:0:Enabled"] = "false",
            };

            IFeatureFlagService featureFlagService = GetConfigBasedFlagService(config, configSectionName);

            Assert.False(featureFlagService.IsEnabled(testFlagName));
        }

        private static IFeatureFlagService GetConfigBasedFlagService(Dictionary<string, string> config, string configSectionName)
        {
            var configuration = GetConfiguration(config);

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection
                .AddFeatureFlags()
                .AddConfigurationFlags(configuration.GetSection(configSectionName));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var featureFlagService = serviceProvider.GetRequiredService<IFeatureFlagService>();
            return featureFlagService;
        }

        private static IConfigurationRoot GetConfiguration(Dictionary<string, string> config)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(config);            
            var configuration = configurationBuilder.Build();
            return configuration;
        }
    }
}
