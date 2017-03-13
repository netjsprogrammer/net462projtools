using System.IO;

namespace net462projtools.tools.HttpRequests.Context
{
    public class FileInfo
    {
        public Stream MemoryStream { get; set; }

        public string FileName { get; set; }
    }
}