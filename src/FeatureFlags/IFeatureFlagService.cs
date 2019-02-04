namespace FeatureFlags
{
    public interface IFeatureFlagService
    {
        bool IsEnabled(string featureFlagName);
    }
}