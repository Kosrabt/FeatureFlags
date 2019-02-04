using System;

namespace FeatureFlags.Implementation
{
    internal class Clock : IClock
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
