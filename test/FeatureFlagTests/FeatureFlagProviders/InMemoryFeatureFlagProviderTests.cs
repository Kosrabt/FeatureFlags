using System.Linq;
using FeatureFlags.FeatureFlagProviders;
using FeatureFlags.FeatureFlagProviders.Inmemory;
using Xunit;

namespace FeatureFlagTests.FeatureFlagProviders
{
    public class InMemoryFeatureFlagProviderTests
    {
        [Fact]
        public void EmptyInmemory_GetFlags_ReturnsEmptyCollection()
        {
            var featureFlagProvider = new InMemoryFeatureFlagProvider();

            var result = featureFlagProvider.GetFlags();

            Assert.Empty(result);
        }

        [Fact]
        public void CanAddFlag()
        {
            const string testFlag = nameof(testFlag); 

            var featureFlagProvider = new InMemoryFeatureFlagProvider();

            featureFlagProvider.AddFlag(testFlag);

            var result = featureFlagProvider.GetFlags();

            var flag = Assert.Single(result);
            Assert.Equal(testFlag, flag.Name);
        }

        [Fact]
        public void CanAddFlagMultipleTimes_But_RemainOnlyOne()
        {
            const string testFlag = nameof(testFlag);

            var featureFlagProvider = new InMemoryFeatureFlagProvider();

            featureFlagProvider.AddFlag(testFlag);
            featureFlagProvider.AddFlag(testFlag);

            var result = featureFlagProvider.GetFlags();

            var flag = Assert.Single(result);
            Assert.Equal(testFlag, flag.Name);
        }

        [Fact]
        public void CanAddMultipleFlag()
        {
            const string testFlag1 = nameof(testFlag1);
            const string testFlag2 = nameof(testFlag2);
            const string testFlag3 = nameof(testFlag3);

            var featureFlagProvider = new InMemoryFeatureFlagProvider();

            featureFlagProvider.AddFlag(testFlag1);
            featureFlagProvider.AddFlag(testFlag2);
            featureFlagProvider.AddFlag(testFlag3);

            var result = featureFlagProvider.GetFlags();

            Assert.NotNull(result);
            Assert.Equal(3,result.Count());

            var items = result.ToArray();

            Assert.Equal(testFlag1, items[0].Name);
            Assert.Equal(testFlag2, items[1].Name);
            Assert.Equal(testFlag3, items[2].Name);
        }

        [Fact]
        public void CanRemoveFlag()
        {
            const string testFlag = nameof(testFlag);
            const string flagToRemove = nameof(flagToRemove);

            var featureFlagProvider = new InMemoryFeatureFlagProvider();

            featureFlagProvider.AddFlag(flagToRemove);
            featureFlagProvider.AddFlag(testFlag);

            featureFlagProvider.RemoveFlag(flagToRemove);

            var result = featureFlagProvider.GetFlags();

            var flag = Assert.Single(result);
            Assert.Equal(testFlag, flag.Name);
        }


        [Fact]
        public void CanClearFlags()
        {
            const string testFlag = nameof(testFlag);

            var featureFlagProvider = new InMemoryFeatureFlagProvider();

            featureFlagProvider.AddFlag(testFlag);
            featureFlagProvider.ClearFlags();

            var result = featureFlagProvider.GetFlags();

            Assert.Empty(result);
        }
    }
}
