using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FeatureFlags.FeatureFlagProviders.Http
{
    public class HttpRequestFeatureFlagProvider : IFeatureFlagProvider
    {
        public const string FeatureHeaderName = "X-Feature-Flags";

        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpRequestFeatureFlagProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<FeatureFlag> GetFlags()
        {
            var headers = httpContextAccessor.HttpContext?.Request?.Headers;

            if (headers == null)
                return Enumerable.Empty<FeatureFlag>();

            if (headers.TryGetValue(FeatureHeaderName, out var flagString))
            {
                var flagNames = flagString.FirstOrDefault()?.Split(';');

                return flagNames.Select(name => new FeatureFlag(name, true)).ToList() ?? Enumerable.Empty<FeatureFlag>();
            }
            return Enumerable.Empty<FeatureFlag>();
        }
    }
}
