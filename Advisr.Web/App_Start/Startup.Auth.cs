using Advisr.DataLayer.Context;
using Advisr.Domain.DbModels;
using Advisr.Web.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Configuration;
using System.Net;

namespace Advisr.Web
{
    public partial class Startup
    {
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; set; }
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            var ca = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieManager = new SystemWebCookieManager(),
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(Double.Parse(System.Configuration.ConfigurationManager.AppSettings["CookieAuthenticationExpireTimeSpan"].ToString())),
                LoginPath = new PathString("/angularJsRoute"), //Replace("/angularJsRoute", "/account/#/login"));
                CookieHttpOnly = false,
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(Double.Parse(System.Configuration.ConfigurationManager.AppSettings["CookieAuthenticationExpireTimeSpan"].ToString())), //need to set to 0, for work Remember Me
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),

                    OnApplyRedirect = ctx =>
                    {
                        if (!IsAjaxRequest(ctx.Request) && !IsApiRequest(ctx.Request))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri.Replace("/angularJsRoute", "/account/#/login"));
                        }
                        else
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                    },
                    OnException = ex =>
                    {

                    },
                    OnResponseSignedIn = context =>
                    {

                    },
                    OnResponseSignIn = context =>
                    {

                    },
                    OnResponseSignOut = context =>
                    {

                    },
                }
            };

            app.UseCookieAuthentication(ca);


            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(30));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = ConfigurationManager.AppSettings["facebookAppID"],
                AppSecret = ConfigurationManager.AppSettings["facebookAppSecret"],
                Provider = new FacebookOauth2Provider(),

            };

            googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["googleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["googleClientSecret"],
                Provider = new GoogleOauth2Provider()
            };

            facebookAuthOptions.Scope.Add("email");

            app.UseFacebookAuthentication(facebookAuthOptions);

            app.UseGoogleAuthentication(googleAuthOptions);
        }

        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }

        private static bool IsApiRequest(IOwinRequest request)
        {
            return request.Path.Value.Contains("/api/");
        }
    }
}
