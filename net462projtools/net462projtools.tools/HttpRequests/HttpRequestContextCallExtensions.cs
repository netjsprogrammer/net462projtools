using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using net462projtools.tools.HttpRequests.Context;
using Newtonsoft.Json;

namespace net462projtools.tools.HttpRequests
{
    public static class HttpRequestContextCallExtensions
    {
        public static async Task<HttpRequestResult> CallAsync(this HttpRequestContext requestContext)
        {
            await requestContext.CallInternalAsync();
            return await requestContext.ParseResult();
        }

        public static async Task<HttpRequestResult<T>> CallAsync<T>(this HttpRequestContext requestContext)
        {
            await CallInternalAsync(requestContext);
            return await requestContext.ParseResult<T>();
        }

        private static async Task<HttpRequestContext> CallInternalAsync(this HttpRequestContext requestContext)
        {
            requestContext.HttpClient = new HttpClient();

            using (requestContext.HttpClient)
            {
                requestContext.HttpClient.DefaultRequestHeaders.Accept.Clear();
                requestContext.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                requestContext.SetClientToken();

                using (var content = requestContext.BuildHttpContent())
                {
                    switch (requestContext.SetupInfo.HttpVerb)
                    {
                        case WebRequestMethods.Http.Get:
                            requestContext.HttpResponseMessage = await requestContext.HttpClient.GetAsync(requestContext.SetupInfo.Url);
                            break;
                        case WebRequestMethods.Http.Post:
                            requestContext.HttpResponseMessage = await requestContext.HttpClient.PostAsync(requestContext.SetupInfo.Url, content);
                            break;
                        case WebRequestMethods.Http.Put:
                            requestContext.HttpResponseMessage = await requestContext.HttpClient.PutAsync(requestContext.SetupInfo.Url, content);
                            break;
                        default:
                            throw new Exception("Allows only POST, PUT, GET verbs");
                    }
                }
            }

            return requestContext;
        }

        private static void SetClientToken(this HttpRequestContext requestContext)
        {
            var encoding = new UTF8Encoding();

            if (!string.IsNullOrEmpty(requestContext.SetupInfo.Token))
            {
                switch (requestContext.SetupInfo.TokenType)
                {
                    case HttpRequestTokenType.Basic:
                        requestContext.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                                Convert.ToBase64String(encoding.GetBytes(requestContext.SetupInfo.Token + ":")));
                        break;
                    case HttpRequestTokenType.Bearer:
                        requestContext.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestContext.SetupInfo.Token);
                        break;
                    case HttpRequestTokenType.Jwt:
                        requestContext.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("jwt", requestContext.SetupInfo.Token);
                        break;
                }
            }
        }

        private static HttpContent BuildJsonContent(this HttpRequestContext requestContext)
        {
            return requestContext.SetupInfo.ContentJsonObject != null
                ? new StringContent(JsonConvert.SerializeObject(requestContext.SetupInfo.ContentJsonObject), Encoding.UTF8, "application/json")
                : null;
        }

        private static HttpContent BuildHttpContent(this HttpRequestContext requestContext)
        {
            HttpContent content = null;

            if (requestContext.SetupInfo.Files.Any())
            {
                var multipartFormDataContent = new MultipartFormDataContent($"Upload {DateTime.UtcNow}");
                var jsonContent = requestContext.BuildJsonContent();

                if (jsonContent != null)
                {
                    multipartFormDataContent.Add(jsonContent);
                }

                requestContext.SetupInfo.Files.ForEach(file => 
                {
                    multipartFormDataContent.Add(new StreamContent(file.MemoryStream), "file", file.FileName);
                });

                content = multipartFormDataContent;
            }
            else
            {
                var jsonContent = requestContext.BuildJsonContent();

                content = jsonContent;
            }

            return content;
        }
    
        private static async Task<HttpRequestResult<T>> ParseResult<T>(this HttpRequestContext requestContext)
        {
            if (requestContext.HttpResponseMessage.StatusCode == HttpStatusCode.OK ||
                requestContext.HttpResponseMessage.StatusCode == HttpStatusCode.Created)
            {
                T returnObject = default(T);

                try
                {
                    // case of application/json
                    returnObject = await requestContext.HttpResponseMessage.Content.ReadAsAsync<T>();
                }
                catch (Exception e)
                {
                    // case of application/http with json result
                    var str = await requestContext.HttpResponseMessage.Content.ReadAsStringAsync();
                    returnObject = JsonConvert.DeserializeObject<T>(str);
                }

                return new HttpRequestResult<T>()
                {
                    Result = returnObject,
                    HttpCode = requestContext.HttpResponseMessage.StatusCode
                };
            }
            else
            {
                return await requestContext.SetupInfo.ErrorParser.ParseException<T>(requestContext.HttpResponseMessage);
            }
        }

        private static async Task<HttpRequestResult> ParseResult(this HttpRequestContext requestContext)
        {
            if (requestContext.HttpResponseMessage.StatusCode == HttpStatusCode.OK
                || requestContext.HttpResponseMessage.StatusCode == HttpStatusCode.Created)
            {
                return new HttpRequestResult()
                {
                    HttpCode = requestContext.HttpResponseMessage.StatusCode
                };
            }
            else
            {
                return await requestContext.SetupInfo.ErrorParser.ParseException(requestContext.HttpResponseMessage);
            }
        }
    }
}
