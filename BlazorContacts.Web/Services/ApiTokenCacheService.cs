using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorContacts.Web.Services
{
    public class ApiTokenCacheService
    {
        private readonly HttpClient _httpClient;
        private static readonly Object _lock = new Object();
        private IDistributedCache _cache;
        private const int _cacheExpirationInDays = 1;

        
        private class AccessTokenItem
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime Expiry { get; set; }
        }

        public ApiTokenCacheService(IHttpClientFactory httpClientFactory, IDistributedCache cache)
        {
            _httpClient = httpClientFactory.CreateClient();
            _cache = cache;
        }

        public async Task<string> GetAccessToken(string clientName, string apiScope, string secret)
        {
            // It should first check if there is a valid access token in the cache. If there is, it should return that token
            AccessTokenItem accessToken = GetFromCache(clientName);
            if (accessToken != null && accessToken.Expiry > DateTime.UtcNow)
            {
                return accessToken.AccessToken;
            }

            // Token not cached, or token is expired. Request new token from auth server
            AccessTokenItem newAccessToken = await RequestNewToken(clientName, apiScope, secret);
            AddToCache(clientName, newAccessToken);

            return newAccessToken.AccessToken;
        }

        public async Task<string> GetAccessTokenDirectly(string clientName, string apiScope, string secret)
        {
            AccessTokenItem newAccessToken = await RequestNewToken(clientName, apiScope, secret);
            return newAccessToken.AccessToken;
        }

        private AccessTokenItem GetFromCache(string key)
        {
            var item = _cache.GetString(key);
            if (item != null)
                return JsonSerializer.Deserialize<AccessTokenItem>(item);

            return null;
        }

        private void AddToCache(string key, AccessTokenItem accessTokenItem)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(_cacheExpirationInDays));
            
            lock (_lock)
            {
                _cache.SetString(key, JsonSerializer.Serialize(accessTokenItem), options);
            }
        }

        private async Task<AccessTokenItem> RequestNewToken(string clientName, string apiScope, string secret)
        {
            try
            {
                var discovery = await HttpClientDiscoveryExtensions.GetDiscoveryDocumentAsync(
                    _httpClient, "http://localhost:5020"
                );
                if (discovery.IsError)
                {
                    throw new ApplicationException($"Error: {discovery.Error}");
                }

                var tokenResponse = await HttpClientTokenRequestExtensions.RequestClientCredentialsTokenAsync(
                    _httpClient, new ClientCredentialsTokenRequest
                    {
                        Scope = apiScope,
                        ClientSecret = secret,
                        Address = discovery.TokenEndpoint,
                        ClientId = clientName
                    }
                );
                if (tokenResponse.IsError)
                {
                    throw new ApplicationException($"Error: {tokenResponse.Error}");
                }

                return new AccessTokenItem{
                    AccessToken = tokenResponse.AccessToken,
                    Expiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                };
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception: {e}");
            }
        }
    }
}