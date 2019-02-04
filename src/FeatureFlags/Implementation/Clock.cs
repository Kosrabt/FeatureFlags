using System;

namespace FeatureFlags.Implementation
{
    public class Clock : IClock
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
