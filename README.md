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

## Feature flag providers

In order to use any feature flag in your code, you have to add Feature flag providers to populate the flags list.

You are able to specify more than one `IFeatureFlagProvider`, but each of them will return only with the enabled flags.

### Configuration based feature flag provider

The configuration based feature flag provider will read a configuration section and populate the flags from that.

This code will read the feature flags from the 'FeatureFlags' section

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddFeatureFlags()
        .AddConfigurationFlags(Configuration.GetSection("FeatureFlags"));
}
```

How the configuration should look like:

```json
{
  "FeatureFlags": {
    "my-feature-flag": {}
  }
}
```

or

```json
{
  "FeatureFlags": [
    { "Name": "my-feature-flag" }
  ]
}
```

You have the following options for each feature flag:

```json
{
  "FeatureFlags": [
    {
      "Name": "my-feature-flag",
      "Enabled": true,  --optional, default true
      "AfterDate": "2019-01-01",  --optional, default DateTime.MinValue
      "UntilDate": "2019-12-31"  --optional, default DateTime.MaxValue
    }
  ]
}
```

The `featureFlagService.IsEnabled("my-feature-flag")` will return true if the `Enabled` is true and we are between the `AfterDate` and `UntilDate`.

###  Http request header based feature flag provider

You can add feature flags from your HttpRequest's header. This allows you do set your feature flags for each call,
for example disabling features for debugging or enabling new parts to test it in an isolated way.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddFeatureFlags()
        .AddHttpHeaderFlags("HeaderName");
}
```

Within the `HeaderName` header you can send a semicolon separated list of flags which will set those flags enabled.

Header sample

```
"my-feature-flag1; my-feature-flag2"
```

### Creating custom feature flag provider

If you implement the `IFeatureFlagProvider`, you are able to add your own logic to add enable feature flags.

Do not forget to register your provider to the DI.

```csharp
public interface IFeatureFlagProvider
{
    IEnumerable<FeatureFlag> GetFlags();
}
```

Each time when the `IFeatureFlagService.IsEnabled(string featureFlagName)` is called,
it calls each `IFeatureFlagProvider GetFlags()` function and make a decision based on the results.

For example, if load flags from a database, make sure, that you are not calling the database each time when the `GetFlags()` function executes.

Example:

```csharp
 public class InMemoryFeatureFlagProvider : IFeatureFlagProvider
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
```
