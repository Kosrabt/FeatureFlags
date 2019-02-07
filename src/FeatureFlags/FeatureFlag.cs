using System;

namespace FeatureFlags
{
    public class FeatureFlag
    {
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime AfterDate { get; set; } = DateTime.MinValue;
        public DateTime UntilDate { get; set; } = DateTime.MaxValue;

        public FeatureFlag() { }

        public FeatureFlag(string name, bool enabled = true)
        {
            Name = name;
            Enabled = enabled;
        }
    }
}
