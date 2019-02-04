using System;

namespace FeatureFlags
{
    public interface IClock
    {
        DateTime UtcNow();
    }
}