using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpService _httpService;
        private readonly ILoggingService _loggingService;

        public TokenService(IHttpService httpService, ILoggingService loggingService)
        {
            _httpService = httpService;
            _loggingService = loggingService;
        }

        public async Task<string> GetAccessToken(TokenProvider tokenProvider, AutoAuthConfiguration autoAuthConfiguration)
        {
            try
            {
                const string tokenEndpointUri = "https://login.windows.net/common/oauth2/token";
                // string tokenProviderResource;
                //switch (tokenProvider)
                //{
                //        case TokenProvider.MicrosoftGraph:
                //            tokenProviderResource = "https://graph.microsoft.com";
                //        break;
                //          case TokenProvider.Outlook:
                //              tokenProviderResource = "https://outlook.office.com";
                //        break;
                //    default:
                //        throw new ArgumentOutOfRangeException(nameof(tokenProvider), tokenProvider, null);
                //}
                var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", autoAuthConfiguration.Username),
                        new KeyValuePair<string, string>("password", autoAuthConfiguration.Password),
                        new KeyValuePair<string, string>("client_id", autoAuthConfiguration.ClientId),
                        new KeyValuePair<string, string>("client_secret", autoAuthConfiguration.ClientSecret),
                        new KeyValuePair<string, string>("resource", autoAuthConfiguration.Resource)
                    }
                );

                var httpResponseMessage = await _httpService.Post(tokenEndpointUri, content);
                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!json.Contains("access_token"))
                {
                    throw new ApplicationException("Can't get Outlook API access token");
                }
                var jsonObject = JObject.Parse(json);
                var token = jsonObject["access_token"].ToString();
                return token;
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error in OutlookService.GetAccessToken", ex);
                throw;
            } 
        }
    }
}