using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace FeatureFlags.FeatureFlagProviders.Configuration
{
    internal sealed class ConfigurationFeatureFlagProvider : IFeatureFlagProvider
    {
        private IEnumerable<FeatureFlag> flags = Enumerable.Empty<FeatureFlag>();

        public ConfigurationFeatureFlagProvider(IOptionsMonitor<FeatureFlagOption> optionsMonitor)
        {
            optionsMonitor.OnChange(ReloadOption);
            ReloadOption(optionsMonitor.CurrentValue, null);
        }

        public IEnumerable<FeatureFlag> GetFlags()
        {
            return flags;
        }

        private void ReloadOption(FeatureFlagOption featureFlagOption, string name)
        {
            flags = featureFlagOption?.Select(ResolveFeatureFlag) ?? Enumerable.Empty<FeatureFlag>();
        }

        private FeatureFlag ResolveFeatureFlag(KeyValuePair<string, FeatureFlag> flagRecord)
        {
            if (!string.IsNullOrEmpty(flagRecord.Value.Name))
                return flagRecord.Value;

            flagRecord.Value.Name = flagRecord.Key;
            return flagRecord.Value;
        }
    }
}
