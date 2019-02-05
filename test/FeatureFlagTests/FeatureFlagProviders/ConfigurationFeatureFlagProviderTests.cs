using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeatureFlags;
using FeatureFlags.FeatureFlagProviders.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace FeatureFlagTests.FeatureFlagProviders
{
    public class ConfigurationFeatureFlagProviderTests
    {
        private class TestOptionsMonitor : IOptionsMonitor<FeatureFlagOption>
        {
            private FeatureFlagOption flagOptions;
            private Action<FeatureFlagOption, string> listener;

            public TestOptionsMonitor(FeatureFlagOption flagOptions)
            {
                this.flagOptions = flagOptions;
            }

            public FeatureFlagOption CurrentValue => flagOptions;

            public FeatureFlagOption Get(string name)
            {
                throw new NotImplementedException();
            }

            public IDisposable OnChange(Action<FeatureFlagOption, string> listener)
            {
                this.listener = listener;
                return null;
            }

            public void Set(FeatureFlagOption flagOptions)
            {
                this.flagOptions = flagOptions;
                listener?.Invoke(flagOptions, "");
            }
        }

        [Fact]
        public void ProviderCanBeCreated()
        {
            var optionMonitor = new Mock<IOptionsMonitor<FeatureFlagOption>>();

            new ConfigurationFeatureFlagProvider(optionMonitor.Object);

            optionMonitor.Verify(x => x.CurrentValue, Times.Once);
        }

        [Fact]
        public void ProviderReadsFromTheMonitor()
        {
            string testFlagName = nameof(testFlagName);
            var testFlag = new FeatureFlag(testFlagName, false);

            var optionMonitor = new Mock<IOptionsMonitor<FeatureFlagOption>>();

            optionMonitor.Setup(x => x.CurrentValue).Returns(new FeatureFlagOption() { { testFlagName, testFlag } });

            var provider = new ConfigurationFeatureFlagProvider(optionMonitor.Object);

            var flags = provider.GetFlags();
            Assert.Single(flags);
            Assert.NotNull(flags.FirstOrDefault());
            Assert.Equal(testFlagName, flags.FirstOrDefault().Name);
        }

        [Fact]
        public void ProviderReloadWillReloadProperly()
        {
            string testFlagName = nameof(testFlagName);
            var originalFlag = new FeatureFlag(testFlagName, false);
            var featureFlagOptions = new FeatureFlagOption() { { testFlagName, originalFlag } };

            string otherTestFlagName = nameof(otherTestFlagName);
            var otherFlag = new FeatureFlag(otherTestFlagName, false);
            var otherFeatureFlagOptions = new FeatureFlagOption() { { testFlagName, otherFlag } };

            var optionsMonitor = new TestOptionsMonitor(featureFlagOptions);

            var provider = new ConfigurationFeatureFlagProvider(optionsMonitor);

            var originalFlags = provider.GetFlags();

            optionsMonitor.Set(otherFeatureFlagOptions);

            var otherFlags = provider.GetFlags();

            Assert.Single(originalFlags);
            Assert.NotNull(originalFlags.FirstOrDefault());
            Assert.Equal(testFlagName, originalFlags.FirstOrDefault().Name);


            Assert.Single(otherFlags);
            Assert.NotNull(otherFlags.FirstOrDefault());
            Assert.Equal(otherTestFlagName, otherFlags.FirstOrDefault().Name);
        }
    }
}
