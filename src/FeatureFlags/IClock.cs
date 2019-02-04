using System;

namespace FeatureFlags
{
    internal interface IClock
    {
        DateTime UtcNow();
    }
}