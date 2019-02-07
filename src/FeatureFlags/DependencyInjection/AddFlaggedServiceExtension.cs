using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags
{
    public static class AddFlaggedServiceExtension
    {
        public static IServiceCollection AddTransientFlaggedService<TServiceType, TEnabled, TDisabled>(this IServiceCollection services, string flagName)
            where TServiceType : class
            where TEnabled : class, TServiceType
            where TDisabled : class, TServiceType
        {
            services.AddTransient<TEnabled>();
            services.AddTransient<TDisabled>();

            services.AddTransient<TServiceType>(sp =>
            {
                var flag = sp.GetRequiredService<IFeatureFlagService>();

                if (flag.IsEnabled(flagName))
                {
                    return sp.GetRequiredService<TEnabled>();
                }
                return sp.GetRequiredService<TDisabled>();
            });

            return services;
        }

        public static IServiceCollection AddScopedFlaggedService<TServiceType, TEnabled, TDisabled>(this IServiceCollection services, string flagName)
           where TServiceType : class
           where TEnabled : class, TServiceType
           where TDisabled : class, TServiceType
        {
            services.AddScoped<TEnabled>();
            services.AddScoped<TDisabled>();

            services.AddScoped<TServiceType>(sp =>
            {
                var flag = sp.GetRequiredService<IFeatureFlagService>();

                if (flag.IsEnabled(flagName))
                {
                    return sp.GetRequiredService<TEnabled>();
                }
                return sp.GetRequiredService<TDisabled>();
            });

            return services;
        }

        public static IServiceCollection AddSingletonFlaggedService<TServiceType, TEnabled, TDisabled>(this IServiceCollection services, string flagName)
           where TServiceType : class
           where TEnabled : class, TServiceType
           where TDisabled : class, TServiceType
        {
            services.AddSingleton<TEnabled>();
            services.AddSingleton<TDisabled>();

            services.AddSingleton<TServiceType>(sp =>
            {
                var flag = sp.GetRequiredService<IFeatureFlagService>();

                if (flag.IsEnabled(flagName))
                {
                    return sp.GetRequiredService<TEnabled>();
                }
                return sp.GetRequiredService<TDisabled>();
            });

            return services;
        }
    }
}
