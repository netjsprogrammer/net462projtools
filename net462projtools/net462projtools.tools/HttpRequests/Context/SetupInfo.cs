using System.Collections.Generic;
using net462projtools.tools.HttpRequests.ErrorParser;

namespace net462projtools.tools.HttpRequests.Context
{
    public class SetupInfo
    {
        public SetupInfo()
        {
            Files = new List<FileInfo>();
        }

        public string Url { get; set; }

        public string HttpVerb { get; set; }

        public object ContentJsonObject { get; set; }

        public List<FileInfo> Files { get; set; }

        public string Token { get; set; }

        public HttpRequestTokenType TokenType { get; set; }

        public IErrorParser ErrorParser { get; set; }
    }
}