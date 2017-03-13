using System;
using System.Net;
using net462projtools.tools.HttpRequests.Context;

namespace net462projtools.tools.HttpRequests
{
    public static class HttpRequestContextValidateExtensions
    {
        public static void Validate(this HttpRequestContext requestContext)
        {
            if (requestContext.IsMultiPart && requestContext.SetupInfo.HttpVerb != WebRequestMethods.Http.Post)
                throw new Exception("Multipart request can use only POST method");
        }
    }
}
