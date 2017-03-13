using System.Net;

namespace net462projtools.tools.HttpRequests
{
    public class HttpRequestResult
    {
        public HttpStatusCode HttpCode { get; set; }

        public virtual bool IsSuccessfull => 
            HttpCode == HttpStatusCode.OK || 
            HttpCode == HttpStatusCode.Created;
    }

    public class HttpRequestResult<T> : HttpRequestResult
    {
        public T Result { get; set; }
    }
}
