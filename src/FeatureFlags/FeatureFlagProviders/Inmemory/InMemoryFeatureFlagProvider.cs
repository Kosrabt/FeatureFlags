using System.Collections.Generic;
using System.Linq;

namespace FeatureFlags.FeatureFlagProviders.Inmemory
{
    public class InMemoryFeatureFlagProvider : IFeatureFlagProvider, IInMemoryFeatureFlagProvider
    {
        private readonly IList<FeatureFlag> flags = new List<FeatureFlag>();

        public IEnumerable<FeatureFlag> GetFlags()
        {
            return flags;
        }

        public void AddFlag(string flagName)
        {
            if (!FlagExists(flagName))
            {
                flags.Add(new FeatureFlag(flagName));
            }
        }

        public void RemoveFlag(string flagName)
        {
            if (FlagExists(flagName))
            {
                var flagToRemove = flags.FirstOrDefault(x => x.Name == flagName);
                flags.Remove(flagToRemove);
            }
        }

        public void ClearFlags()
        {
            flags.Clear();
        }

        private bool FlagExists(string flagName) => flags.Any(f => f.Name == flagName);
    }
}
