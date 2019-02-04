using System.Collections.Generic;
using System.Linq;
using FeatureFlags.FeatureFlagProviders.Configuration;

namespace FeatureFlags
{
    internal sealed class FeatureFlagService : IFeatureFlagService
    {
        private readonly IEnumerable<IFeatureFlagProvider> featureFlagProviders;
        private readonly IClock clock;

        public FeatureFlagService(IEnumerable<IFeatureFlagProvider> featureFlagProviders, IClock clock)
        {
            this.featureFlagProviders = featureFlagProviders ?? throw new System.ArgumentNullException(nameof(featureFlagProviders));
            this.clock = clock;
        }

        public bool IsEnabled(string featureFlagName)
        {
            if (string.IsNullOrEmpty(featureFlagName))
                throw new System.ArgumentException("FlagName cannot be null or Empty", nameof(featureFlagName));

            return GetEnabledFlags().Any(f => f.FeatureName.Equals(featureFlagName, System.StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<FeatureFlag> GetEnabledFlags()
        {
            var utcNow = clock.UtcNow();
            var flags = featureFlagProviders.SelectMany(x => x.GetFlags());
            return flags.Where(flag => flag.Enabled && (flag.AfterDate < utcNow && utcNow < flag.UntilDate));
        }
    }
}
