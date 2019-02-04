using System.Collections.Generic;

namespace FeatureFlags
{
    public interface IFeatureFlagProvider
    {
        IEnumerable<FeatureFlag> GetFlags();
    }
}