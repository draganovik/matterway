using Microsoft.Extensions.Options;

namespace Matterway.ServiceDefaults.Authorization;

internal sealed class SystemAccessKeyHttpMessageHandler(IOptions<SystemAccessKeyOptions> options) : DelegatingHandler
{
    private readonly string _accessKey = options.Value.AccessKey;
    private readonly IReadOnlySet<string> _allowedAuthorities = options.Value.AllowedAuthorities;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (ShouldAttachSystemAccessKey(request.RequestUri, _allowedAuthorities) &&
            !request.Headers.Contains(SystemAccessKeyOptions.HeaderName))
            request.Headers.TryAddWithoutValidation(SystemAccessKeyOptions.HeaderName, _accessKey);

        return base.SendAsync(request, cancellationToken);
    }

    private static bool ShouldAttachSystemAccessKey(Uri? requestUri, IReadOnlySet<string> allowedAuthorities)
    {
        if (requestUri is null)
            return false;

        if (!requestUri.IsAbsoluteUri)
            return true;

        return allowedAuthorities.Contains(requestUri.Authority) ||
               allowedAuthorities.Contains(requestUri.Host);
    }
}