using Advisr.DataLayer;
using Advisr.Web.Helpers;
using Advisr.Web.Models;
using Advisr.Web.Providers;
using Advisr.Web.Quartz;
using Advisr.Web.Results;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Advisr.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private AutopilotAPIClient provider;

        public AccountController()
        {
            if (provider == null)
            {
                provider = AutopilotAPIClient.Create();
            }
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
#if DEBUG
            try
            {
                //TODO: temp code
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var users = unitOfWork.UserRepository.GetAll().ToList();
                }
            }
            catch (Exception e)
            {
                throw;
            }
#endif

            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var userName = model.Email;
                Advisr.Domain.DbModels.ApplicationUser user = null;
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    //Get user
                    user = unitOfWork.UserRepository.GetAll().FirstOrDefault(a => a.Email == model.Email || a.UserName == model.Email);

                    if (user != null)
                    {
                        string message = null;
                        userName = user.UserName;

                        if ((await UserManager.IsEmailConfirmedAsync(user.Id)) == false)
                        {
                            message = "Email not confirmed, please check your inbox to confirm the email.";
                        }
                        else
                        {
                            await UserManager.SetLockoutEnabledAsync(user.Id, true);

                            var validCredentials = await UserManager.FindAsync(userName, model.Password);

                            // When a user is lockedout, this check is done to ensure that even if the credentials are valid
                            // the user can not login until the lockout duration has passed
                            if (await UserManager.IsLockedOutAsync(user.Id))
                            {
                                message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts. **", ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString());
                            }

                            // if user is subject to lockouts and the credentials are invalid
                            // record the failure and check if user is lockedout and display message, otherwise, 
                            // display the number of attempts remaining before lockout
                            else if (await UserManager.GetLockoutEnabledAsync(user.Id) && validCredentials == null)
                            {
                                // Record the failure which also may cause the user to be locked out
                                await UserManager.AccessFailedAsync(user.Id);

                                if (await UserManager.IsLockedOutAsync(user.Id))
                                {
                                    message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts. **", ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString());
                                }
                                else
                                {
                                    int accessFailedCount = await UserManager.GetAccessFailedCountAsync(user.Id);

                                    int attemptsLeft = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"].ToString()) - accessFailedCount;

                                    message = string.Format("Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);
                                }
                            }
                            else if (validCredentials == null)
                            {
                                message = "Invalid login attempt **";
                            }
                            else
                            {
                                if (user.LockedByAdmin)
                                {
                                    message = "Your account has been locked by admin.";
                                    return JsonError(HttpStatusCode.BadRequest, 10, message, ModelState);
                                }

                                if ((await SignInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, shouldLockout: false)) != SignInStatus.Success)
                                {
                                    message = "Invalid login attempt **";
                                }
                                else
                                {
                                    // When token is verified correctly, clear the access failed count used for lockout
                                    await UserManager.ResetAccessFailedCountAsync(user.Id);

                                    var customer = unitOfWork.CustomerDetailsRepository.GetAll().FirstOrDefault(a => a.UserId == user.Id);

                                    if (user.AutopilotTrack && !string.IsNullOrEmpty(user.AutopilotContactId))
                                    {
                                        StartUpdatingAutopilot(customer.User.AutopilotContactId, customer.UserId);
                                    }

                                    var redirectUrl = Url.Action("Index", "Dashboard", new { }, protocol: Request.Url.Scheme);

                                    if (returnUrl != null && returnUrl != "empty")
                                    {
                                        redirectUrl = returnUrl;
                                    }

                                    if (customer != null && customer.Status == Domain.DbModels.CustomerStatus.Pending)
                                    {
                                        redirectUrl = string.Format("{0}/Dashboard/#/user/profile", Request.Url.GetLeftPart(UriPartial.Authority));
                                    }

                                    var resultJson = new
                                    {
                                        success = true,
                                        user = new
                                        {
                                            id = user.Id,
                                            firstName = user.FirstName,
                                            lastName = user.LastName
                                        },
                                        redirectUrl = redirectUrl
                                    };

                                    return Json(resultJson);
                                }
                            }
                        }

                        ModelState.AddModelError("validationInfo", message);

                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }
                    else
                    {
                        ModelState.AddModelError("validationInfo", "Invalid login attempt **");

                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        [HttpGet]
        [OverrideAuthentication]
        [System.Web.Http.HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;
            //var helper = OauthHelper.Create();

            if (error != null)
            {
                return JsonError(HttpStatusCode.BadRequest, 0, Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account"));
            }

            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            
            string redirectUri = null;

            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = unitOfWork.UserLoginRepository.GetAll().Where(a => a.ProviderKey == loginInfo.Login.ProviderKey && a.LoginProvider == loginInfo.Login.LoginProvider).Select(a => a.UserId).FirstOrDefault();

                var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(a => a.Id == userId);

                if (user != null)
                {
                    if (user.LockedByAdmin)
                    {
                        var message = "Your account has been locked by admin.";
                        redirectUri = string.Format("{0}/account/#/login?error={1}",
                                       Request.Url.GetLeftPart(UriPartial.Authority),
                                       message);

                        return Redirect(redirectUri);
                    }
                }
                // Sign in the user with this external login provider if the user already has a login
                var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                switch (result)
                {
                    case SignInStatus.Success:

                        var customer = unitOfWork.CustomerDetailsRepository.GetAll().FirstOrDefault(a => a.UserId == userId);

                        if (user.AutopilotTrack && !string.IsNullOrEmpty(user.AutopilotContactId))
                        {
                            StartUpdatingAutopilot(customer.User.AutopilotContactId, customer.UserId);
                        }

                        redirectUri = string.Format("{0}/Dashboard/", Request.Url.GetLeftPart(UriPartial.Authority));

                        if (customer != null && customer.Status == Domain.DbModels.CustomerStatus.Pending)
                        {
                            redirectUri = string.Format("{0}/Dashboard/#/user/profile", Request.Url.GetLeftPart(UriPartial.Authority));
                        }

                        return Redirect(redirectUri);
                    case SignInStatus.Failure:
                    default:
                        
                        ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(loginInfo.ExternalIdentity as ClaimsIdentity);

                        var un = externalLogin.UserName.Split(' ');

                        string firstName = un[0];
                        string lastName = un.Length > 1 ? un[1] : "";

                        if (externalLogin.LoginProvider == "Facebook")
                        {
                            var client = new FacebookClient(externalLogin.ExternalAccessToken);
                            dynamic me = client.Get("me?fields=first_name,last_name,email,birthday");
                            firstName = me.first_name;
                            lastName = me.last_name;
                            externalLogin.Email = me.email;

                            var d = me.birthday;
                        }
                        
                        //Doubting approach
                        if (string.IsNullOrWhiteSpace(externalLogin.Email))
                        {
                            externalLogin.Email = string.Format("{0}_{1}@tempadvisr.email", externalLogin.ProviderKey, externalLogin.LoginProvider[0]);
                        }

                        if (string.IsNullOrWhiteSpace(externalLogin.Email))
                        {
                            redirectUri = string.Format("{0}/account/#/register?external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&email={5}&firstName={6}&lastName={7}",
                                                       Request.Url.GetLeftPart(UriPartial.Authority),
                                                       externalLogin.ExternalAccessToken,
                                                       externalLogin.LoginProvider,
                                                       false,
                                                       externalLogin.UserName,
                                                       externalLogin.Email,
                                                       externalLogin.UserName.Split(' ')[0], //First Name
                                                       externalLogin.UserName.Split(' ').LastOrDefault()); //Last Name

                            return Redirect(redirectUri);
                        }
                        else
                        {
                            //Register new user
                            UserController userController = new UserController(this.UserManager, this.SignInManager);

                            RegisterGeneralModel model = new RegisterGeneralModel();
                            model.Email = externalLogin.Email;
                            model.FirstName = firstName;
                            model.LastName = lastName;
                            model.Provider = externalLogin.LoginProvider;
                            model.ExternalAccessToken = externalLogin.ExternalAccessToken;

                            var userRegisterResult = await userController.RegisterInternal(model);

                            if (userRegisterResult.HasError)
                            {
                                redirectUri = string.Format("{0}/account/#/login?error={1}",
                                                        Request.Url.GetLeftPart(UriPartial.Authority),
                                                        userRegisterResult.ErrorMessage);

                                return Redirect(redirectUri);
                            }
                            else
                            {
                                return RedirectToAction("ExternalLoginCallback", "Account");
                            }
                        }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return JsonError(HttpStatusCode.BadRequest, 0, "Please check parameters");
            }

            var identityResult = await UserManager.ConfirmEmailAsync(userId, code);

            var result = new
            {
                succeeded = identityResult.Succeeded,
                errors = identityResult.Errors
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Account");
        }


        /// GET: /Account/SlideSession
        /// <summary>
        /// This method needs for reset expiration time for AspNe.ApplicationCookies (athentication cookies)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult SlideSession()
        {
            string result = "ok";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void StartUpdatingAutopilot(string contact_id, string userId)
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = factory.GetScheduler();
            JobDataMap dataMap = new JobDataMap();

            dataMap["autopilotContactId"] = contact_id;
            dataMap["OperationType"] = "Login";
            dataMap["userId"] = userId;

            var job = JobBuilder.Create<UpdateContactJob>()
                .WithIdentity("UpdateContactJob").UsingJobData(dataMap)
                .Build();

            var jobKey = new JobKey("UpdateContactJob");

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1")
                .StartAt(DateTime.Now)
                .ForJob(jobKey)
                .Build();

            if (!scheduler.CheckExists(jobKey))
            {
                scheduler.ScheduleJob(job, trigger);
            }

            scheduler.Start();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion
    }
}
