using System;
using System.Linq;
using FeatureFlags.FeatureFlagProviders.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace FeatureFlagTests.FeatureFlagProviders
{
    public class HttpRequestFeatureFlagProviderTests
    {
        private const string HeaderName = "X-Feature-Header";

        [Fact]
        public void NullHttpContextAccessor_Returns_Empty()
        {
            var provider = new HttpRequestFeatureFlagProvider(null, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }

        [Fact]
        public void NullHttpContext_Returns_Empty()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock.Object, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }

        [Fact]
        public void NullRequest_Returns_Empty()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext.Object);
            httpContext.Setup(x => x.Request).Returns((HttpRequest)null);

            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock.Object, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }

        [Fact]
        public void NullHeaders_Returns_Empty()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            var httpRequest = new Mock<HttpRequest>();

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext.Object);
            httpContext.Setup(x => x.Request).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Headers).Returns((HeaderDictionary)null);

            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock.Object, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }

        [Fact]
        public void EmptyHeaders_Returns_Empty()
        {
            var headerValues = new HeaderDictionary();

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }

        [Fact]
        public void HeadersContainsKeyButEmpty_Returns_Empty()
        {
            var headerValues = new HeaderDictionary();
            headerValues.Add("X-Test-Header", new StringValues());

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }


        [Fact]
        public void HeadersNotContainsKey_Returns_Empty()
        {
            var headerValues = new HeaderDictionary();
            headerValues.Add("X-Not-Test-Header", new StringValues("Not a value"));

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }

        [Fact]
        public void SingleValue_Returns_OnlyFlag()
        {
            string flagName = nameof(flagName);

            HeaderDictionary headerValues = GetFeatureHeader(flagName);

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            var singleFlag = Assert.Single(flags);
            Assert.Equal(flagName, singleFlag.Name);
        }

        [Fact]
        public void EmptyHeaderValue_Returns_Empty()
        {
            HeaderDictionary headerValues = GetFeatureHeader(String.Empty);

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            Assert.Empty(flags);
        }

        [Fact]
        public void SingleValue_Returns_OnlyFlagAndEnabled()
        {
            string flagName = nameof(flagName);

            HeaderDictionary headerValues = GetFeatureHeader(flagName);

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            var singleFlag = Assert.Single(flags);
            Assert.True(singleFlag.Enabled);
        }

        [Fact]
        public void MultipleCorrectValue_Returns_All()
        {
            string flagName1 = nameof(flagName1);
            string flagName2 = nameof(flagName2);

            HeaderDictionary headerValues = GetFeatureHeader($"{flagName1};{flagName2}");

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            Assert.Equal(2, flags.Count());

            var first = flags.ElementAt(0);
            Assert.NotNull(first);
            Assert.Equal(flagName1, first.Name);

            var second = flags.ElementAt(1);
            Assert.NotNull(second);
            Assert.Equal(flagName2, second.Name);
        }

        [Fact]
        public void MultipleCorrectValue_Returns_All_AndRemovesEmptyResults()
        {
            string flagName1 = nameof(flagName1);
            string flagName2 = nameof(flagName2);

            HeaderDictionary headerValues = GetFeatureHeader($"{flagName1};;  ;;{flagName2}");

            IHttpContextAccessor httpContextAccessorMock = GetHttpContextAccessor(headerValues);
            var provider = new HttpRequestFeatureFlagProvider(httpContextAccessorMock, HeaderName);

            var flags = provider.GetFlags();

            Assert.Equal(2, flags.Count());

            var first = flags.ElementAt(0);
            Assert.NotNull(first);
            Assert.Equal(flagName1, first.Name);

            var second = flags.ElementAt(1);
            Assert.NotNull(second);
            Assert.Equal(flagName2, second.Name);
        }

        private static HeaderDictionary GetFeatureHeader(string flagName)
        {
            return new HeaderDictionary
            {
                { HeaderName, new StringValues(flagName) }
            };
        }

        private static IHttpContextAccessor GetHttpContextAccessor(HeaderDictionary headerValues)
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            var httpRequest = new Mock<HttpRequest>();

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext.Object);
            httpContext.Setup(x => x.Request).Returns(httpRequest.Object);
            httpRequest.Setup(x => x.Headers).Returns(headerValues);
            return httpContextAccessorMock.Object;
        }
    }
}
