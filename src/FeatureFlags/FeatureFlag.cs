using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureFlags
{
    public class FeatureFlag
    {
        public string FeatureName { get; set; }
        public bool Enabled { get; set; } = false;
        public DateTime AfterDate { get; set; } = DateTime.MinValue;
        public DateTime UntilDate { get; set; } = DateTime.MaxValue;

        public FeatureFlag() { }

        public FeatureFlag(string featureName, bool enabled = false)
        {
            FeatureName = featureName;
            Enabled = enabled;
        }
    }
}
