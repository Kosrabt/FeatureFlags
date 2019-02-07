namespace FeatureFlags.FeatureFlagProviders.Inmemory
{
    public interface IInMemoryFeatureFlagProvider
    {
        void AddFlag(string flagName);
        void ClearFlags();
        void RemoveFlag(string flagName);
    }
}