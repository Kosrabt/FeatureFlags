using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags
{
    public static class IServiceCollectionExtensions
    {
        public static FeatureFlagsServiceCollection AddFeatureFlags(IServiceCollection services)
        {
            services.AddSingleton<IClock, Implementation.Clock>();
            services.AddTransient<IFeatureFlagService, FeatureFlagService>();

            return new FeatureFlagsServiceCollection(services);
        }
    }
}
