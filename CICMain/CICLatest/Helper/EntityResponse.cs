using System.Net;

namespace CICLatest.Helper
{
    public class EntityResponse
    {
        public HttpStatusCode Code { get; set; }
        public string Data { get; set; }
        public string Message { get; set; }
        public string NextPartitionKey { get; set; }
        public string NextRowKey { get; set; }
         
    }
}