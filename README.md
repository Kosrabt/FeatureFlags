# FeatureFlags

The feature flag concept enables you to enable/disable/switch between implementations based on featue flags.

A good feature flag can enable you:
- **Kill-switch** to disable code sections if something went wrong
- **Canary-testing, A/B testing**, so you can enable features for only a specific set of users
- **Opt-in / Opt-out** to enable/disable functions for users
- **Flag-Driven-Development** to roll out any development without actually enabling it in the deploy time. Later it can be turned on by switching the flag.

## Requirements

You need **.Net core 2.1 / .Net standard 2.0** for this package to work.

## Usage

The FeatureFlag library based on the .Net Core's Dependency Injection abstraction system, so you have to have the DI framework in your project

### Adding the feature flag services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddFeatureFlags();
}
```

### Using the feature flags:

First, you have to inject the `IFeatureFlagService` to your class

```csharp
public TestService(IFeatureFlagService featureFlagService)
{
	this.featureFlagService = featureFlagService;
}
```

Than you can check that a flag is enabled or not:

```csharp
public string TestFeatureFlag()
{
    if (featureFlagService.IsEnabled("my-feature-flag"))
    {
        return "The flag is enabled";
    }
    else
    {
        return "The flag is disabled";
    }
}
```