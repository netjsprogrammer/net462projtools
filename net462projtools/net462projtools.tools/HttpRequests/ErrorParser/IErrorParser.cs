using System.Net.Http;
using System.Threading.Tasks;

namespace net462projtools.tools.HttpRequests.ErrorParser
{
    public interface IErrorParser
    {
        Task<HttpRequestResult<T>> ParseException<T>(HttpResponseMessage httpResponse);

        Task<HttpRequestResult> ParseException(HttpResponseMessage httpResponse);
    }
}
