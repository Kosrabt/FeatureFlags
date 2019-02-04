using System;

namespace FeatureFlags.Implementation
{
    internal sealed class Clock : IClock
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
