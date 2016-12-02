using System.Net;

namespace Advisr.Web.Models
{
    public class Error
    {
        public int ServerErrorCode { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string Message { get; set; }

        public object Details { get; set; }
    }
}
