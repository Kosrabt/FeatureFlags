using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FeatureFlags.FeatureFlagProviders.Http
{
    internal sealed class HttpRequestFeatureFlagProvider : IFeatureFlagProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string headerName;

        public HttpRequestFeatureFlagProvider(IHttpContextAccessor httpContextAccessor, string headerName)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.headerName = headerName;
        }

        public IEnumerable<FeatureFlag> GetFlags()
        {
            var headers = httpContextAccessor?.HttpContext?.Request?.Headers;

            if (headers == null)
                return Enumerable.Empty<FeatureFlag>();

            if (headers.TryGetValue(headerName, out var flagString))
            {
                var flagNames = flagString
                    .FirstOrDefault()
                    .Split(new[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim());
                
                return flagNames.Select(name => new FeatureFlag(name, true)).ToList();
            }
            return Enumerable.Empty<FeatureFlag>();
        }
    }
}
