using FeatureFlags.FeatureFlagProviders;
using Xunit;

namespace FeatureFlagTests.FeatureFlagProviders
{
    public class FeatureFlagProviderTests
    {
        [Fact]
        public void GetFlags_ReturnsEmptyCollection()
        {
            var featureFlagProvider = new FeatureFlagProvider();

            var result = featureFlagProvider.GetFlags();

            Assert.Empty(result);
        }
    }
}
