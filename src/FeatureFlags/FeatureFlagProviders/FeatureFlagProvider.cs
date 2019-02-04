using System.Collections.Generic;
using System.Linq;

namespace FeatureFlags.FeatureFlagProviders
{
    public class FeatureFlagProvider : IFeatureFlagProvider
    {
        protected IEnumerable<FeatureFlag> Flags = Enumerable.Empty<FeatureFlag>();

        public IEnumerable<FeatureFlag> GetFlags()
        {
            return Flags;
        }
    }
}