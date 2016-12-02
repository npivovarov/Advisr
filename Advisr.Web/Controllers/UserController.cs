using Advisr.Core;
using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Helpers;
using Advisr.Web.Models;
using Advisr.Web.Models.AutopilotModels;
using Advisr.Web.Providers;
using Advisr.Web.Quartz;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Advisr.Web.Controllers
{
    /// <summary>
    /// Api Controller for manage users
    /// </summary>
    [Authorize]
    public class UserController : BaseApiController
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private bool _fromUnitTest;
        private AutopilotAPIClient autopilot;
        /// <summary>
        /// Ctr 
        /// </summary>
        public UserController()
        {
        }

        /// <summary>
        /// Ctr for Unit test
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this._fromUnitTest = true;
            if (autopilot == null)
            {
                autopilot = AutopilotAPIClient.Create();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Get user info by id;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult Get(string id = null)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                if (id != null)
                {
                    userId = id;
                }

                var startLinkToPhoto = Url.Link("Default", new { controller = "files", action = "photo" });

                var userDb = unitOfWork.UserRepository.GetAll()
                    .Where(a => a.Id == userId)
                    .Select(a => new
                    {
                        id = a.Id,
                        username = a.UserName,
                        firstName = a.FirstName,
                        lastName = a.LastName,
                        email = a.Email,
                        status = a.Status,
                        avatarFileId = a.AvatarFileId,
                        hasPasssword = a.PasswordHash != null,
                        photo = new
                        {
                            smallPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=s"),
                            bigPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=b"),
                        },
                        roles = a.Roles.Select(p => new
                        {
                            id = p.Role.Name
                        }),
                        createdDate = a.CreatedDate,
                        createdBy = new
                        {
                            id = a.CreatedBy.Id,
                            firstName = a.CreatedBy.FirstName,
                            lastName = a.CreatedBy.LastName,
                        },
                    })
                    .FirstOrDefault();

                if (userDb == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the user");
                }
                var customerStatus = unitOfWork.CustomerDetailsRepository.GetAll().Where(c => c.UserId == userDb.id).Select(c => c.Status).First();

                var result = new
                {
                    id = userDb.id,
                    username = userDb.username,
                    firstName = userDb.firstName,
                    lastName = userDb.lastName,
                    email = userDb.email,
                    status = userDb.status,
                    avatarFileId = userDb.avatarFileId,
                    photo = userDb.photo,
                    roles = userDb.roles.ToList(),
                    createdDate = userDb.createdDate,
                    createdBy = userDb.createdBy,
                    isProfileCompleted = customerStatus == CustomerStatus.Completed,
                    hasPassword = userDb.hasPasssword
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Get list of all users, allow only for Admin role;
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("List")]
        [Authorize(Roles = "ADMIN")]
        public IHttpActionResult List(int offset = 0, int count = 10, string q = null)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                IQueryable<ApplicationUser> query = unitOfWork.UserRepository.GetAll()
                   .Where(a => /*a.Status == status && a.Roles.Any(r => r.Role.Id == DataLayer.DbConstants.PrivateUserRoleId) &&*/ a.Hidden == false);

                if (!string.IsNullOrEmpty(q))
                {
                    query = query.Where(e => e.FirstName.StartsWith(q)
                            || e.Id == q
                            || e.LastName.StartsWith(q)
                            || e.Email == q);
                }

                var countOfUsers = query.Count();

                var startLinkToPhoto = Url.Link("Default", new { controller = "files", action = "photo" });

                var users = query
                            .OrderByDescending(a => a.CreatedDate)
                            .Skip(offset)
                            .Take(count)
                            .ToList()
                            .Select(a => new
                            {
                                id = a.Id,
                                firstName = a.FirstName,
                                lastName = a.LastName,
                                photo = new
                                {
                                    smallPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=s"),
                                    bigPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=b"),
                                },
                                joinedDate = a.CreatedDate,
                                status = a.Status
                            }).ToList();


                var result = new
                {
                    myUserId = userId,
                    count = countOfUsers,
                    data = users
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Register new user by email and password. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>JSON with User id</returns>
        [HttpPost]
        [ActionName("Register")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register([FromBody] RegisterModel model)
        {
            if (model.Password != null)
            {
                //Validate password
                var validateResult = await ApplicationUserManager.GetPasswordValidator().ValidateAsync(model.Password);
                if (!validateResult.Succeeded)
                {
                    foreach (var error in validateResult.Errors)
                    {
                        ModelState.AddModelError("password", error);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var registerGeneral = new RegisterGeneralModel()
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ContactPhone = model.ContactPhone,
                    Password = model.Password,
                };

                var regResult = await RegisterInternal(registerGeneral);
                if(regResult.HasError)
                {
                    return JsonError(regResult.HttpStatusCode, regResult.ServerErrorCode, regResult.ErrorMessage, regResult.ModelState);
                }
                else
                {
                    var result = new
                    {
                        userId = regResult.UserId
                    };
                    return Json(result);
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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RegisterExternal")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalModel model)
        {
            var verifiedAccessToken = new ParsedExternalAccessToken();

            if (ModelState.IsValid)
            {
                var helper = OauthHelper.Create();
                if (!string.IsNullOrEmpty(model.Provider) && !string.IsNullOrEmpty(model.ExternalAccessToken))
                {
                    verifiedAccessToken = await helper.VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
                    if (verifiedAccessToken == null)
                    {
                        return this.JsonError(HttpStatusCode.BadRequest, 10, "Invalid Provider or External Access Token", ModelState);
                    }
                }

                var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();
                ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(loginInfo.ExternalIdentity as ClaimsIdentity);

                var registerGeneral = new RegisterGeneralModel()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    //FirstName = externalLogin.UserName.Split(' ')[0], //First Name
                    //LastName = externalLogin.UserName.Split(' ').LastOrDefault(), //Last Name
                    ExternalAccessToken = model.ExternalAccessToken,
                    Provider = model.Provider
                };

                var regResult = await RegisterInternal(registerGeneral);
                if (regResult.HasError)
                {
                    return JsonError(regResult.HttpStatusCode, regResult.ServerErrorCode, regResult.ErrorMessage, regResult.ModelState);
                }
                else
                {
                    var result = new
                    {
                        userId = regResult.UserId
                    };
                    return Json(result);
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

        /// <summary>
        /// Change password for authorized user;
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model.NewPassword != null)
            {
                //Validates password
                var validateResult = await ApplicationUserManager.GetPasswordValidator().ValidateAsync(model.NewPassword);
                if (!validateResult.Succeeded)
                {
                    foreach (var error in validateResult.Errors)
                    {
                        ModelState.AddModelError("newPassword", error);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    bool succeeded = false;
                    List<string> errors = new List<string>();

                    var userId = User.Identity.GetUserId();

                    var user = unitOfWork.UserRepository.GetAll().Where(a => a.Id == userId).First();

                    if (user.PasswordHash != null)
                    {
                        var result = await this.UserManager.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
                        succeeded = result.Succeeded;

                        if (!succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("validationInfo", error);
                            }
                        }
                    }
                    else
                    {
                        user.PasswordHash = new PasswordHasher().HashPassword(model.NewPassword);
                        unitOfWork.UserRepository.Edit(user);
                        await unitOfWork.SaveAsync();
                    }

                    if (succeeded || model.OldPassword == "external")
                    {
                        if (user != null)
                        {
                            await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            return Ok(); // Need to refresh workspace page for get new Cookies 
                        }
                    }

                    return this.JsonError(HttpStatusCode.InternalServerError, 0, "Warning", ModelState);
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

        /// <summary>
        /// Reset password any user, allow only for Admin role;
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ResetPassword")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IHttpActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            ///Validate password
            if (model.Password != null)
            {
                var validateResult = await ApplicationUserManager.GetPasswordValidator().ValidateAsync(model.Password);
                if (!validateResult.Succeeded)
                {
                    foreach (var error in validateResult.Errors)
                    {
                        ModelState.AddModelError("password", error);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();

                    var adminUserName = unitOfWork.UserRepository.GetAll().Where(a => a.Id == userId).Select(a => a.FirstName + " " + a.LastName).First();

                    var currentUser = unitOfWork.UserRepository.GetAll().First(a => a.Id == model.Id);

                    currentUser.PasswordHash = new PasswordHasher().HashPassword(model.Password);
                    currentUser.SecurityStamp = Guid.NewGuid().ToString();
                    currentUser.HasTempPassword = true;

                    unitOfWork.UserRepository.Edit(currentUser);
                    await unitOfWork.SaveAsync();

                    var userJson = new
                    {
                        id = currentUser.Id,
                    };

                    return Json(userJson);
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

        /// <summary>
        /// Method to send email with reset password link;
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    //ModelState.AddModelError("email", "Invalid email. Enter your registered email*");
                    //return this.JsonError(HttpStatusCode.BadRequest, 0, "Warning", ModelState);
                }
                else
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                    var callbackUrl = string.Format("{0}/#/set-new-password?userId={1}&code={2}", Url.Link("Default", new { controller = "account" }), user.Id, Uri.EscapeDataString(code));

                    await EmailNitificationHelper.Create().SendEmailResetPassword(user, UserManager, callbackUrl);
                }

                return this.Ok();
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

        /// <summary>
        /// Set new pussword by link from Email;
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ActionName("SetNewPassword")]
        public async Task<IHttpActionResult> SetNewPassword([FromBody] ResetPasswordModel model)
        {
            ///Validate password
            if (model.Password != null)
            {
                var validateResult = await ApplicationUserManager.GetPasswordValidator().ValidateAsync(model.Password);
                if (!validateResult.Succeeded)
                {
                    foreach (var error in validateResult.Errors)
                    {
                        ModelState.AddModelError("password", error);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ModelState.AddModelError("email", "The email is missing in the system **");
                    return this.JsonError(HttpStatusCode.BadRequest, 0, "Warning", ModelState);
                }

                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                    {
                        user = unitOfWork.UserRepository.GetAll().First(a => a.Id == user.Id);
                        user.HasTempPassword = false;
                        user.EmailConfirmed = true;
                        unitOfWork.UserRepository.Edit(user);
                        await unitOfWork.SaveAsync();
                    }

                    return Ok();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("validationInfo", error);
                    }
                    return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }




        /// <summary>
        /// Register User Internal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal async Task<RegisterUserResult> RegisterInternal(RegisterGeneralModel model)
        {
            var userResult = new RegisterUserResult();
            
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var checkUserEmail = unitOfWork.UserRepository.GetAll().FirstOrDefault(a => a.Email == model.Email || a.UserName == model.Email);

                if (checkUserEmail != null)
                {
                    userResult.AddError(HttpStatusCode.BadRequest, 10, "This email is already taken ***");
                    return userResult;
                }
                
                unitOfWork.BeginTransaction();

                //Create user record
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName ?? string.Empty,
                    LastName = model.LastName ?? string.Empty,
                    UserName = model.Email,
                    Email = model.Email,
                    //AvatarFileId = model.AvatarFileId,
                    HasTempPassword = true,
                    CreatedById = DbConstants.AdminUserId,
                    CreatedDate = DateTime.Now
                };

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    var names = model.UserName.Split(' ');

                    user.FirstName = names[0];
                    user.LastName = names.Length > 1 ? names[1] : "";
                }

                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.PasswordHash = new PasswordHasher().HashPassword(model.Password);
                }

                user.AutopilotTrack = true;
                user.SecurityStamp = Guid.NewGuid().ToString();


                #region Need To redo
                {
                    ISchedulerFactory factory = new StdSchedulerFactory();
                    IScheduler scheduler = factory.GetScheduler();
                    JobDataMap dataMap = new JobDataMap();

                    dataMap["registermodel"] = model;
                    dataMap["userId"] = user.Id;
                    dataMap["OperationType"] = "Registration";

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
                #endregion


                unitOfWork.UserRepository.Insert(user);
                await unitOfWork.SaveAsync();

                ApplicationUserRole userRole = new ApplicationUserRole();
                userRole.RoleId = DataLayer.DbConstants.CustomerUserRole;
                userRole.UserId = user.Id;
                unitOfWork.UserRoleRepository.Insert(userRole);

                await unitOfWork.SaveAsync();

                Address address = new Address();
                address.CreatedById = user.Id;
                address.CreatedDate = DateTime.Now;

                unitOfWork.AddressRepository.Insert(address);
                await unitOfWork.SaveAsync();

                CustomerDetails customer = new CustomerDetails();
                customer.Id = Guid.NewGuid();
                customer.CreatedById = user.Id;
                customer.CreatedDate = DateTime.Now;
                customer.UserId = user.Id;
                customer.ContactPhone = model.ContactPhone;
                customer.AddressId = address.Id;

                customer.ModifiedReason = "created";
                unitOfWork.CustomerDetailsRepository.Insert(customer);

                await unitOfWork.SaveAsync();

                if (!string.IsNullOrEmpty(model.Provider) && !string.IsNullOrEmpty(model.ExternalAccessToken))
                {
                    var loginInfo = await this.SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();

                    ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(loginInfo.ExternalIdentity as ClaimsIdentity);

                    ApplicationUserLogin userLogin = new ApplicationUserLogin();
                    userLogin.UserId = user.Id;
                    userLogin.ProviderKey = loginInfo.Login.ProviderKey;
                    userLogin.LoginProvider = loginInfo.Login.LoginProvider;

                    unitOfWork.UserLoginRepository.Insert(userLogin);

                    await unitOfWork.SaveAsync();
                }

                unitOfWork.CommitTransaction();

                if (model.Provider == null)
                {
                    //Senf verification Email 
                    {
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = string.Format("{0}/#/confirm-email?userId={1}&code={2}", Url.Link("Default", new { controller = "account" }), user.Id, Uri.EscapeDataString(code));
                        await EmailNitificationHelper.Create().SendConfirmationEmail(user, UserManager, callbackUrl);
                    }
                }

                userResult.UserId = user.Id;

                return userResult;
            }

        }

        /// <summary>
        /// Save photo from uploaded File;
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        internal static async Task<HttpStatusCode> SavePhotoAvatar(string userId, string fileId, Uri requestUri)
        {
            //HttpClient client = new HttpClient();

            //var address = ConfigurationManager.AppSettings["blobEndPointAddress"].ToString();
            //var token = ConfigurationManager.AppSettings["blobToken"].ToString();

            //var from = string.Format("{0}://{1}:{2}", requestUri.Scheme, requestUri.Host, requestUri.Port);

            //HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/api/file/SaveFileAsPhoto?userId={1}&fileId={2}&from={3}&token={4}", address, userId, fileId, from, token));

            HttpStatusCode code = HttpStatusCode.NotFound;
            //try
            //{
            //    var result = await client.SendAsync(message);
            //    code = result.StatusCode;
            //}
            //catch (Exception)
            //{
            //}

            return code;
        }
    }
}
