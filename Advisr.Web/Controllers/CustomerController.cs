using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Models;
using Advisr.Web.Providers;
using Advisr.Web.Quartz;
using Microsoft.AspNet.Identity;
using Quartz;
using Quartz.Impl;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Advisr.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class CustomerController : BaseApiController
    {
        AutopilotAPIClient autopilot;
        public CustomerController()
        {
            if (autopilot == null)
            {
                autopilot = AutopilotAPIClient.Create();
            }
        }

        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult Get(string userId = null)
        {
            if(string.IsNullOrEmpty(userId))
            {
                userId = User.Identity.GetUserId();
            }
            
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var startLinkToPhoto = Url.Link("Default", new { controller = "files", action = "photo" });

                var userDb = unitOfWork.CustomerDetailsRepository.GetAll()
                    .Where(a => a.UserId == userId)
                    .Select(a => new
                    {
                        id = a.Id,
                        title = a.Title,
                        firstName = a.User.FirstName,
                        lastName = a.User.LastName,
                        roles = a.User.Roles.Select(r => r.Role.Name),
                        email =  a.User.Email.EndsWith("@tempadvisr.email") ? null : a.User.Email,
                        status = a.Status,
                        dateOfBirth = a.DateOfBirth,
                        avatarFileId = a.User.AvatarFileId,
                        contactPhone = a.ContactPhone,
                        homePhone = a.HomePhone,
                        maritalStatus = a.MaritalStatus,
                        gender = a.Gender,
                        locked = a.User.LockedByAdmin,
                        modifiedReason = a.ModifiedReason,
                        address = new
                        {
                            id = a.Address.Id,
                            customerId = a.Id,
                            addressLine1 = a.Address.Address_1,
                            addressLine2 = a.Address.Address_2,
                            city = a.Address.City,
                            state = a.Address.State,
                            postCode = a.Address.PostCode,
                        },
                        photo = new
                        {
                            smallPhoto = string.Concat(startLinkToPhoto, "?id=", a.User.AvatarFileId, "&type=s"),
                            bigPhoto = string.Concat(startLinkToPhoto, "?id=", a.User.AvatarFileId, "&type=b"),
                        },
                    })
                    .FirstOrDefault();

                if (userDb == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the user");
                }
                
                return Json(userDb);
            }
        }

        /// <summary>
        /// Get list of all customers, allow only for Admin role;
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

                var query = unitOfWork.CustomerDetailsRepository.GetAll()
                   .Where(a => a.User.Hidden == false);

                if (!string.IsNullOrEmpty(q))
                {
                    query = query.Where(e => 
                               e.User.FirstName.StartsWith(q)
                            || e.User.LastName.StartsWith(q)
                            || e.User.Email == q);
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
                                customerId = a.Id,
                                userId = a.UserId,
                                firstName = a.User.FirstName,
                                lastName = a.User.LastName,
                                roles = a.User.Roles.Select(r => r.Role.Name),
                                email = a.User.Email,
                                locked = a.User.LockedByAdmin,
                                photo = new
                                {
                                    smallPhoto = string.Concat(startLinkToPhoto, "?id=", a.User.AvatarFileId, "&type=s"),
                                    bigPhoto = string.Concat(startLinkToPhoto, "?id=", a.User.AvatarFileId, "&type=b"),
                                },
                                joinedDate = a.CreatedDate,
                                status = a.Status
                            }).ToList();

                foreach (var item in users)
                {
                    item.roles.ToList();
                }

                var result = new
                {
                    count = countOfUsers,
                    data = users
                };

                return Json(result);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Edit")]
        public async Task<IHttpActionResult> Edit([FromBody] CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();
                    var isAdmin = User.IsInRole("ADMIN");
                    var customer = unitOfWork.CustomerDetailsRepository.GetAll().FirstOrDefault(c => c.Id == model.Id);

                    if (customer == null)
                    {
                        return JsonError(HttpStatusCode.BadRequest, 10, "Customer doesn't exist.", ModelState);
                    }

                    if (customer.UserId != userId && !isAdmin) 
                    {
                        return JsonError(HttpStatusCode.BadRequest, 10, "You have not enough rights to perform this operation.", ModelState);
                    }

                    model.ModifiedReason = model.ModifiedReason;
                    
                    var checkUserEmail = unitOfWork.UserRepository.GetAll().FirstOrDefault(a => (a.Email == model.Email || a.UserName == model.Email) && a.Id != customer.UserId);

                    if (checkUserEmail != null)
                    {
                        ModelState.AddModelError("email", "This email is already taken ***");
                    }

                    if (model.Gender == null)
                    {
                        ModelState.AddModelError("gender", "Gender cannot be empty.");
                    }

                    if (!ModelState.IsValid)
                    {
                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }

                    ISchedulerFactory factory = new StdSchedulerFactory();
                    IScheduler scheduler = factory.GetScheduler();
                    JobDataMap dataMap = new JobDataMap();

                    dataMap["autopilotContactId"] = customer.User.AutopilotContactId;
                    dataMap["model"] = model;
                    dataMap["userId"] = customer.UserId;
                    dataMap["OperationType"] = "Edit";

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
                    
                    unitOfWork.BeginTransaction();
                    customer.User.FirstName = model.FirstName;
                    customer.User.LastName = model.LastName;
                    customer.User.UserName = model.Email;
                    customer.Title = model.Title;
                    customer.User.Email = model.Email;
                    customer.ContactPhone = model.ContactPhone;
                    customer.HomePhone = model.HomePhone;
                    customer.MaritalStatus = model.MaritalStatus;
                    customer.DateOfBirth = model.DateOfBirth;
                    customer.Gender = model.Gender;
                    customer.ModifiedById = userId;
                    customer.ModifiedDate = DateTime.Now;
                    customer.ModifiedReason = model.ModifiedReason;
                    customer.Status = CustomerStatus.Completed;

                    if (isAdmin)
                    {
                        if (model.Roles != null)
                        {
                            if (model.Roles.Count == 0)
                            {
                                ModelState.AddModelError("roles", "Must contain at least one role.");
                                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                            }

                            var modelRoles = unitOfWork.RoleRepository.GetAll().Where(r => model.Roles.Contains(r.Name)).ToList();

                            var customerRoles = customer.User.Roles.Select(r=>r.Role).ToList();

                            foreach (var modelRole in modelRoles)
                            {
                                if (customerRoles.FirstOrDefault(r => r.Name == "ADMIN") != null
                                    && modelRoles.FirstOrDefault(r => r.Name == "ADMIN") == null && userId == customer.UserId)
                                {
                                    ModelState.AddModelError("roles", "Cannot delete ADMIN role.");

                                    return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                                }

                                foreach (var role in customerRoles)
                                {
                                    if (!modelRoles.Any(r => r.Name == role.Name))
                                    {
                                        var uRole = unitOfWork.UserRoleRepository.GetAll().Where(a => a.UserId == customer.UserId && a.RoleId == role.Id).FirstOrDefault();
                                        if (uRole != null)
                                        {
                                            unitOfWork.UserRoleRepository.Delete(uRole);
                                        }
                                    }
                                }

                                if (!customerRoles.Any(r => r.Name == modelRole.Name))
                                {
                                    ApplicationUserRole role = new ApplicationUserRole();
                                    role.RoleId = modelRole.Id;
                                    role.Role = modelRole;
                                    role.UserId = customer.UserId;
                                    role.User = customer.User;

                                    unitOfWork.UserRoleRepository.Insert(role);
                                } 
                            }
                            await unitOfWork.SaveAsync();
                        }
                    }

                    var customerLog = new CustomerLog();
                    customerLog.CustomerId = customer.Id;
                    customerLog.ModifiedDate = DateTime.Now;
                    customerLog.ModifiedBy = User.Identity.GetUserId();
                    customerLog.ModifiedReason = customer.ModifiedReason;
                    customerLog.UserId = customer.UserId;
                    unitOfWork.CustomerLogRepository.Insert(customerLog);

                    await unitOfWork.SaveAsync();

                    var customerAddress = customer.Address;
                    customerAddress.Address_1 = model.Address.AddressLine1;
                    customerAddress.Address_2 = model.Address.AddressLine2;
                    customerAddress.City = model.Address.City;
                    customerAddress.State = model.Address.State;
                    customerAddress.PostCode = model.Address.PostCode;
                    customerAddress.ModifiedById = userId;
                    customerAddress.ModifiedDate = DateTime.Now;
                    
                    unitOfWork.AddressRepository.Edit(customerAddress);

                    unitOfWork.CustomerDetailsRepository.Edit(customer);

                    await unitOfWork.SaveAsync();

                    unitOfWork.CommitTransaction();

                    return Ok();
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ActionName("Lock")]
        public async Task<IHttpActionResult> Lock(string id)
        {
            return await LockOrUnlockUser(id, "Lock");
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ActionName("Unlock")]
        public async Task<IHttpActionResult> Unlock(string id)
        {
            return await LockOrUnlockUser(id, "Unlock");
        }

        private async Task<IHttpActionResult> LockOrUnlockUser(string id, string actionName)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var customerId = Guid.Parse(id);
                var user = unitOfWork.CustomerDetailsRepository.GetAll().Where(c => c.Id == customerId).Select(c => c.User).FirstOrDefault();

                if (user == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Customer was not found", ModelState);
                }
                if (actionName == "Lock")
                {
                    user.LockedByAdmin = true;
                    user.LockedByAdminId = User.Identity.GetUserId();
                }
                else
                {
                    user.LockedByAdmin = false;
                    user.LockedByAdminId = null;
                }
                

                unitOfWork.UserRepository.Edit(user);
                await unitOfWork.SaveAsync();

                return Ok();
            }
        }
    }
}
