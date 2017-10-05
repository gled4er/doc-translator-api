using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// HTTP Service
    /// </summary>
    [Serializable]
    public class HttpService : IHttpService
    {
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// HTTP Service constructor
        /// </summary>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        public HttpService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Post done with access token
        /// </summary>
        /// <param name="endpoint">HTTP endpoint</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="payload">Data sent to endpoint</param>
        /// <param name="preferTimeZone">Preferred timezone</param>
        /// <returns>Task of <see cref="HttpResponseMessage"/></returns>
        public async Task<HttpResponseMessage> AuthenticatedPost(string endpoint, string accessToken, object payload, string preferTimeZone)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var serializedObject = JsonConvert.SerializeObject(payload);
                    var body = new StringContent(serializedObject);
                    body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    if (!string.IsNullOrEmpty(preferTimeZone))
                    {
                        body.Headers.Add("Prefer", preferTimeZone);
                    }
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var httpResponseMessage = await httpClient.PostAsync(endpoint, body);
                    httpResponseMessage.EnsureSuccessStatusCode();
                    return httpResponseMessage;
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Get made with access token
        /// </summary>
        /// <param name="endpoint">HTTP endpoint</param>
        /// <param name="accessToken">Access token</param>
        /// <returns>Task of <see cref="HttpResponseMessage"/></returns>
        public async Task<HttpResponseMessage> AuthenticatedGet(string endpoint, string accessToken)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var httpResponseMessage = await httpClient.GetAsync(new Uri(endpoint));
                    if (!httpResponseMessage.IsSuccessStatusCode)
                    {
                        throw new HttpException((int) httpResponseMessage.StatusCode,
                            "Not successful HTTP call in HTTPService.AuthenticatedGet");
                    }
                    return httpResponseMessage;

                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Post operation
        /// </summary>
        /// <param name="endpoint">HTTP endpoint</param>
        /// <param name="content">Instance of <see cref="FormUrlEncodedContent"/></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post(string endpoint, FormUrlEncodedContent content)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var httpResponseMessage = await client.PostAsync(endpoint, content);
                    httpResponseMessage.EnsureSuccessStatusCode();
                    return httpResponseMessage;
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error in HttpService.Post", ex);
                throw;
            }
        }
    }
}
