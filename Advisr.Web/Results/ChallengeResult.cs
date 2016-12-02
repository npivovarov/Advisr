using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

namespace Advisr.Web.Results
{
    //public class ChallengeResult : IHttpActionResult
    //{
    //    public ChallengeResult(string loginProvider, ApiController controller)
    //    {
    //        LoginProvider = loginProvider;
    //        Request = controller.Request;
    //    }

    //    public string LoginProvider { get; set; }
    //    public HttpRequestMessage Request { get; set; }

    //    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    //    {
    //        Request.GetOwinContext().Authentication.Challenge(LoginProvider);

    //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
    //        response.RequestMessage = Request;
    //        return Task.FromResult(response);
    //    }
    //}

    internal class ChallengeResult : HttpUnauthorizedResult
    {
        private const string XsrfKey = "XsrfId";

        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }
        public string UserId { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary[XsrfKey] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
}
