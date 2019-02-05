using System;
using System.Linq;
using FeatureFlags;
using FeatureFlags.Implementation;
using Moq;
using Xunit;

namespace FeatureFlagTests
{
    public class FeatureFlagTests
    {
        private readonly IClock clock = new Clock();

        [Fact]
        public void WithEmptyProviderList_ReturnFalse()
        {
            string testFlag = nameof(testFlag);
            var testFeatureFlagService = new FeatureFlagService(Enumerable.Empty<IFeatureFlagProvider>(), clock);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }

        [Fact]
        public void NullFlagName_NotAllowed()
        {
            string testFlag = null;
            var testFeatureFlagService = new FeatureFlagService(Enumerable.Empty<IFeatureFlagProvider>(), clock);

            Assert.Throws<ArgumentException>(() => testFeatureFlagService.IsEnabled(testFlag));
        }

        [Fact]
        public void EmptyFlagName_NotAllowed()
        {
            string testFlag = string.Empty;
            var testFeatureFlagService = new FeatureFlagService(Enumerable.Empty<IFeatureFlagProvider>(), clock);

            Assert.Throws<ArgumentException>(() => testFeatureFlagService.IsEnabled(testFlag));
        }

        [Fact]
        public void OneProvider_Returns_EnabledForExisting()
        {
            string testFlag = nameof(testFlag);

            var testProvider = CreateTestFeatureProvider(new[] { testFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.True(result);
        }

        [Fact]
        public void OneProvider_Returns_NotEnabledForNotExisting()
        {
            string testFlag = nameof(testFlag);
            string notExistingFlag = nameof(notExistingFlag);

            var testProvider = CreateTestFeatureProvider(new[] { testFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(notExistingFlag);
            Assert.False(result);
        }


        [Fact]
        public void MultipleProvider_Returns_EnabledForExistingFromFirst()
        {
            string testFlag = nameof(testFlag);
            string otherFlag = nameof(otherFlag);

            var testProvider = CreateTestFeatureProvider(new[] { testFlag });

            var otherProvider = CreateTestFeatureProvider(new[] { otherFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider, otherProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.True(result);
        }

        [Fact]
        public void MultipleProvider_Returns_EnabledForExistingFromOther()
        {
            string testFlag = nameof(testFlag);
            string otherFlag = nameof(otherFlag);

            var testProvider = CreateTestFeatureProvider(new[] { testFlag });

            var otherProvider = CreateTestFeatureProvider(new[] { otherFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider, otherProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(otherFlag);
            Assert.True(result);
        }

        [Fact]
        public void MulitpleProvider_Returns_NotEnabledForNotExisting()
        {
            string testFlag = nameof(testFlag);
            string otherFlag = nameof(otherFlag);
            string notExistingFlag = nameof(notExistingFlag);

            var testProvider = CreateTestFeatureProvider(new[] { testFlag });

            var otherProvider = CreateTestFeatureProvider(new[] { otherFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider, otherProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(notExistingFlag);
            Assert.False(result);
        }

        [Fact]
        public void EnabledTestFlag_ReturnsTrue()
        {
            string testFlag = nameof(testFlag);
            var testProvider = CreateTestFeatureProvider(new[] { new FeatureFlag(nameof(testFlag), enabled: true) });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.True(result);
        }

        [Fact]
        public void DisabledTestFlag_ReturnFalse()
        {
            string testFlag = nameof(testFlag);
            var testProvider = CreateTestFeatureProvider(new[] { new FeatureFlag(nameof(testFlag), enabled: false) });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }

        [Fact]
        public void OneEnabledOneDisabled_Returns_True()
        {
            string testFlag = nameof(testFlag);
            var testProvider = CreateTestFeatureProvider(new[] { new FeatureFlag(nameof(testFlag), enabled: true) });
            var otherProvider = CreateTestFeatureProvider(new[] { new FeatureFlag(nameof(testFlag), enabled: false) });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider, otherProvider }, clock);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.True(result);
        }

        [Fact]
        public void TwoDisabled_Returns_False()
        {
            string testFlag = nameof(testFlag);
            var testProvider = CreateTestFeatureProvider(new[] { new FeatureFlag(nameof(testFlag), enabled: false) });
            var otherProvider = CreateTestFeatureProvider(new[] { new FeatureFlag(nameof(testFlag), enabled: false) });

            var featureFlagService = new FeatureFlagService(new[] { testProvider, otherProvider }, clock);

            var result = featureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }

        [Fact]
        public void EnabledAfterAfterDate_TestFlag_ReturnsTrue()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                AfterDate = testDate.AddDays(-1)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.True(result);
        }

        [Fact]
        public void DisabledBeforeAfterDate_TestFlag_ReturnsFalse()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                AfterDate = testDate.AddDays(1)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }


        [Fact]
        public void EnabledBeforeBeforeDate_TestFlag_ReturnsTrue()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                UntilDate = testDate.AddDays(1)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.True(result);
        }

        [Fact]
        public void DisabledAfterBeforeDate_TestFlag_ReturnsFalse()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                UntilDate = testDate.AddDays(-1)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }

        [Fact]
        public void BeforeDateRange_TestFlag_ReturnsFalse()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                AfterDate = testDate.AddDays(1),
                UntilDate = testDate.AddDays(10)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }

        [Fact]
        public void OnStartOfDateRange_TestFlag_ReturnsFalse()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                AfterDate = testDate,
                UntilDate = testDate.AddDays(10)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }

        [Fact]
        public void BetweenDateRange_TestFlag_ReturnsTrue()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                AfterDate = testDate.AddDays(-1),
                UntilDate = testDate.AddDays(10)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.True(result);
        }

        [Fact]
        public void OnEndOfDateRange_TestFlag_ReturnsFalse()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                AfterDate = testDate.AddDays(-10),
                UntilDate = testDate
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }

        [Fact]
        public void AfterDateRange_TestFlag_ReturnsFalse()
        {
            var testDate = DateTime.UtcNow;
            var mockClock = new Mock<IClock>();
            mockClock.Setup(x => x.UtcNow()).Returns(testDate);

            string testFlag = nameof(testFlag);

            var testFeatureFlag = new FeatureFlag(nameof(testFlag), enabled: true)
            {
                AfterDate = testDate.AddDays(-1),
                UntilDate = testDate.AddDays(-10)
            };

            var testProvider = CreateTestFeatureProvider(new[] { testFeatureFlag });

            var testFeatureFlagService = new FeatureFlagService(new[] { testProvider }, mockClock.Object);

            var result = testFeatureFlagService.IsEnabled(testFlag);
            Assert.False(result);
        }



        private static IFeatureFlagProvider CreateTestFeatureProvider(string[] returnItems)
        {
            var testProvider = new Mock<IFeatureFlagProvider>();
            testProvider.Setup(p => p.GetFlags()).Returns(returnItems.Select(x => new FeatureFlag() { Name = x, Enabled = true }));
            return testProvider.Object;
        }

        private static IFeatureFlagProvider CreateTestFeatureProvider(FeatureFlag[] returnItems)
        {
            var testProvider = new Mock<IFeatureFlagProvider>();
            testProvider.Setup(p => p.GetFlags()).Returns(returnItems);
            return testProvider.Object;
        }
    }
}
