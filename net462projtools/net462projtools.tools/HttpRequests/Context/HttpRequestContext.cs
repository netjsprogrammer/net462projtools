using System.Linq;
using System.Net.Http;

namespace net462projtools.tools.HttpRequests.Context
{
    public class HttpRequestContext
    {
        public HttpRequestContext()
        {
            SetupInfo = new SetupInfo();
        }

        public SetupInfo SetupInfo { get; set; }

        public HttpClient HttpClient { get; set; }

        public HttpResponseMessage HttpResponseMessage { get; set; }

        public bool IsMultiPart => SetupInfo.Files.Any();
    }
}
