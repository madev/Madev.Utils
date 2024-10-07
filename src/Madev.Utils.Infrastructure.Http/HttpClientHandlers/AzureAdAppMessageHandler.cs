using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;

namespace Madev.Utils.Infrastructure.Http.HttpClientHandlers
{
    public class AzureAdAppMessageHandler : HttpClientHandler
    {
        private readonly TokenCredential _credential;
        private readonly string[] _scopes;

        public AzureAdAppMessageHandler(string resourceId)
            : this(new DefaultAzureCredential(), resourceId)
        {
        }

        public AzureAdAppMessageHandler(TokenCredential credential, string resourceId)
        {
            _credential = credential;
            _scopes = new[] { resourceId + "/.default" };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _credential.GetTokenAsync(new TokenRequestContext(_scopes), cancellationToken);

            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", "Bearer " + token);
            return await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}