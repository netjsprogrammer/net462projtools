using System;
using System.IO;
using net462projtools.tools.HttpRequests.Context;
using net462projtools.tools.HttpRequests.ErrorParser;
using FileInfo = net462projtools.tools.HttpRequests.Context.FileInfo;

namespace net462projtools.tools.HttpRequests
{
    public static class HttpRequestContextSetupExtensions
    {
        public static HttpRequestContext SetUrl(this HttpRequestContext httpRequestContext, string url)
        {
            httpRequestContext.SetupInfo.Url = url;
            return httpRequestContext;
        }

        public static HttpRequestContext SetVerb(this HttpRequestContext httpRequestContext, string httpVerb)
        {
            httpRequestContext.SetupInfo.HttpVerb = httpVerb;
            return httpRequestContext;
        }

        public static HttpRequestContext SetToken(this HttpRequestContext httpRequestContext, string token)
        {
            httpRequestContext.SetupInfo.Token = token;
            return httpRequestContext;
        }

        public static HttpRequestContext SetTokenType(this HttpRequestContext httpRequestContext, HttpRequestTokenType tokenType)
        {
            httpRequestContext.SetupInfo.TokenType = tokenType;
            return httpRequestContext;
        }

        public static HttpRequestContext SetAnotherFile(this HttpRequestContext httpRequestContext, MemoryStream memoryStream, string fileName)
        {
            httpRequestContext.SetupInfo.Files.Add(new FileInfo()
            {
                FileName = fileName,
                MemoryStream = memoryStream
            });
            return httpRequestContext;
        }

        public static HttpRequestContext SetContentObject(this HttpRequestContext httpRequestContext, object contentJsonObject)
        {
            if (httpRequestContext.SetupInfo.ContentJsonObject != null)
                throw new Exception("Content JSON object already defined");

            httpRequestContext.SetupInfo.ContentJsonObject = contentJsonObject;

            return httpRequestContext;
        }

        public static HttpRequestContext SetContentObject(this HttpRequestContext httpRequestContext, IErrorParser errorParser)
        {
            if (httpRequestContext.SetupInfo.ContentJsonObject != null)
                throw new Exception("Content JSON object already defined");

            httpRequestContext.SetupInfo.ErrorParser = errorParser;

            return httpRequestContext;
        }
    }
}
