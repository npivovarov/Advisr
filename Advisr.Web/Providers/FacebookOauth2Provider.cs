using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security.Facebook;

namespace Advisr.Web.Providers
{
    public class FacebookOauth2Provider: FacebookAuthenticationProvider
    {
        public override void ApplyRedirect(FacebookApplyRedirectContext context)
        {
            context.Response.Redirect(context.RedirectUri + "&display=popup");
            base.ApplyRedirect(context);
        }

        public override Task ReturnEndpoint(FacebookReturnEndpointContext context)
        {
           
            return base.ReturnEndpoint(context);
        }

        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));

            if (context.Email != null)
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Email, context.Email));
            }
            
            return Task.FromResult<object>(null);
        }
    }
}