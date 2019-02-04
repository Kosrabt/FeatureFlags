using System.Linq;
using Microsoft.Extensions.Options;

namespace FeatureFlags.FeatureFlagProviders.Configuration
{
    public class ConfigurationFeatureFlagProvider : FeatureFlagProvider
    {
        public ConfigurationFeatureFlagProvider(IOptionsMonitor<FeatureFlagOption> optionsMonitor)
        {
            optionsMonitor.OnChange(ReloadOption);
            ReloadOption(optionsMonitor.CurrentValue, null);
        }

        private void ReloadOption(FeatureFlagOption featureFlagOption, string name)
        {
            Flags = featureFlagOption?.ToList() ?? Enumerable.Empty<FeatureFlag>();
        }
    }
}
