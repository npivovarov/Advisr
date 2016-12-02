using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Models;
using Advisr.Web.Quartz;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class PolicyController : BaseApiController
    {
        /// <summary>
        /// Get policy info by id;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult Get(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var startLinkToPhoto = Url.Link("Default", new { controller = "files", action = "get" });

                var userId = User.Identity.GetUserId();

                var query = unitOfWork.PolicyRepository.GetAll();

                if (User.IsInRole("ADMIN") == false)
                {
                    //Get only own policy
                    query = query.Where(a => a.CreatedById == userId);
                }

                var policyDb = query
                    .Where(a => a.Id == id)
                    .Select(a => new
                    {
                        id = a.Id,
                        status = a.Status,
                        title = a.Title,
                        subTitle = a.SubTitle,
                        policyGroupId = a.PolicyTypeId,
                        insurerId = a.InsurerId,
                        startDate = a.StartDate,
                        endDate = a.EndDate,
                        policyNumber = a.PolicyNumber,
                        description = a.Description,
                        //expiryDate = a.PolicyExpiryDate,
                        policyPremium = a.PolicyPremium,
                        policyExcess = a.PolicyExcess,
                        policyPaymentAmount = a.PolicyPaymentAmount,
                        policyPaymentFrequency = a.PolicyPaymentFrequency,

                        policyGroup = a.PolicyType == null ? null : new
                        {
                            id = a.PolicyTypeId,
                            groupName = a.PolicyType.GroupName,
                            policyTypeName = a.PolicyType.PolicyTypeName
                        },
                        insurer = a.Insurer == null ? null : new
                        {
                            id = a.InsurerId,
                            name = a.Insurer.Name,
                            color = a.Insurer.Color,
                            url = a.Insurer.URL,
                            phone = a.Insurer.Phone,
                            phoneOverseas = a.Insurer.PhoneOverseas,
                            email = a.Insurer.Email,
                        },
                        //TODO: change field names to lower case
                        vehiclePolicyModel = a.VehicleItems.Select(v => new
                        {
                            Id = v.Id,
                            PolicyId = a.Id,
                            Year = v.Year,
                            Colour = v.Colour,
                            RegistredDriverName = v.RegistredDriverName,
                            RegistrationNumber = v.RegistrationNumber,
                            RegistrationState = v.RegistrationState,
                            Make = v.Make.MakeName,
                            Model = v.Model.ModelName,
                            Mileage = v.Mileage,
                            IsCommercial = new
                            {
                                id = v.IsCommercial,
                                name = v.IsCommercial == true ? "Yes" : "No"
                            } 
                        }),
                        //TODO: change field names to lower case
                        lifePolicyModel = a.LifeItems.Select(l => new
                        {
                            Id = l.Id,
                            PolicyId = l.PolicyId,
                            Medication = l.Medication,
                            MedicationCondition = l.MedicationCondition
                        }),
                        //TODO: change field names to lower case
                        homePolicyModel = a.HomeItems.Select(h => new
                        {
                            Id = h.Id,
                            PolicyId = h.PolicyId,
                            Address = h.Address,
                            BuildDate = h.BuildDate,
                        }),
                        files = a.PolicyFiles.Where(f => f.Status != FileStatus.deleted).Select(p => new
                        {
                            id = p.FileId,
                            fileName = p.File.FileName,
                            fileSize = p.File.FileSize,
                            uploadedDate = p.File.CreatedDate,
                            fileUrl = string.Concat(startLinkToPhoto, "?id=", p.FileId, "&type=inline"),
                        }),
                        additionalProperties = a.AdditionalPolicyFields.Select(af => new
                        {
                            groupFileId = af.PolicyTypeFieldId,
                            value = af.Value
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

                if (policyDb == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the policy");
                }
                
                var result = new
                {
                    id = policyDb.id,
                    status = policyDb.status,
                    title = policyDb.title,
                    subTitle = policyDb.subTitle,
                    policyGroupId = policyDb.policyGroupId,
                    insurerId = policyDb.insurerId,
                    startDate = policyDb.startDate,
                    endDate = policyDb.endDate,
                    policyNumber = policyDb.policyNumber,
                    description = policyDb.description,
                    //expiryDate = policyDb.expiryDate,
                    policyPremium = policyDb.policyPremium,
                    policyExcess = policyDb.policyExcess,
                    policyPaymentAmount = policyDb.policyPaymentAmount,
                    policyPaymentFrequency = policyDb.policyPaymentFrequency,
                    policyGroup = policyDb.policyGroup,
                    insurer = policyDb.insurer,

                    vehiclePolicyModel = policyDb.vehiclePolicyModel.FirstOrDefault(),
                    lifePolicyModel = policyDb.lifePolicyModel.FirstOrDefault(),
                    homePolicyModel = policyDb.homePolicyModel.FirstOrDefault(),

                    files = policyDb.files.ToList(),
                    additionalProperties = policyDb.additionalProperties.ToList(),
                    createdDate = policyDb.createdDate,
                    createdBy = policyDb.createdBy,
                };


                return Json(result);
            }
        }

        /// <summary>
        /// Get a short policy details by id for policy listing;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("ShortDetails")]
        public IHttpActionResult ShortDetails(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var startLinkToPhoto = Url.Link("Default", new { controller = "files", action = "get" });

                var userId = User.Identity.GetUserId();

                var query = unitOfWork.PolicyRepository.GetAll();

                if (User.IsInRole("ADMIN") == false)
                {
                    //Get only own policy
                    query = query.Where(a => a.CreatedById == userId);
                }

                var policyDb = query
                    .Where(a => a.Id == id)
                    .Select(a => new
                    {
                        id = a.Id,
                        descrition = a.Description,
                        insurer = a.Insurer == null ? null : new
                        {
                            id = a.InsurerId,
                            name = a.Insurer.Name,
                            url = a.Insurer.URL,
                            phone = a.Insurer.Phone,
                            phoneOverseas = a.Insurer.PhoneOverseas,
                            email = a.Insurer.Email,
                        },
                    })
                    .FirstOrDefault();

                if (policyDb == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the policy");
                }
                
                return Json(policyDb);
            }
        }
        
        /// <summary>
        /// Get list of all user policy, allow only for Admin role;
        /// </summary>
        /// <param name="status"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="groupName">For exmp: Me, Vehicle, Property, etc.
        /// <returns></returns>
        [HttpGet]
        [ActionName("List")]
        public IHttpActionResult List(PolicyStatus status = PolicyStatus.None, PolicySortType sortType = PolicySortType.All, int offset = 0, int count = 10, string groupName = null,  string q = null)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var startLinkToLogo = Url.Link("Default", new { controller = "files", action = "getLogo" });


                IQueryable<Policy> query = unitOfWork.PolicyRepository.GetAll()
                   .Where(a => a.CreatedById == userId);

                if(!string.IsNullOrEmpty(groupName))
                {
                    query = query.Where(e =>
                            e.PolicyType != null && e.PolicyType.GroupName == groupName);
                }
                
                if (status != PolicyStatus.None)
                {
                    query = query.Where(a => a.Status == status);
                }
                
                if (!string.IsNullOrEmpty(q))
                {
                    query = query.Where(e =>
                               e.PolicyNumber == q);
                }
                
                if(sortType == PolicySortType.Active)
                {
                    query = query.Where(a => a.Status == PolicyStatus.Unconfirmed || a.EndDate > DateTime.Now);
                }
                else if (sortType == PolicySortType.Expired)
                {
                    query = query.Where(a => a.EndDate < DateTime.Now);
                }
                
                var countOfUsers = query.Count();
                var countOfProcessing = query.Where(a => a.Status == PolicyStatus.Unconfirmed).Count();
                
                var policyList = query
                            .OrderByDescending(a => a.CreatedDate)
                            .Skip(offset)
                            .Take(count)
                            .ToList()
                            .Select(a => new
                            {
                                id = a.Id,
                                status = a.Status,
                                title = a.Title,
                                subTitle = a.SubTitle,
                                policyPaymentAmount = a.PolicyPaymentAmount,
                                policyPaymentFrequency = a.PolicyPaymentFrequency,
                                policyNumber = a.PolicyNumber,
                                endDate = a.EndDate,
                                fileNames = a.PolicyFiles.Where(f => f.Status != FileStatus.deleted).Select(f => f.File.FileName),
                                policyGroup = a.PolicyType == null ? null : new
                                {
                                    groupName = a.PolicyType.GroupName,
                                    policyType = a.PolicyType.PolicyTypeName
                                },
                                insurer = a.Insurer == null ? null : new
                                {
                                    name = a.Insurer.Name,
                                    color = a.Insurer.Color,
                                    logoUrl = string.Concat(startLinkToLogo, "?id=", a.Insurer.LogoFileId),
                                },
                                createdDate = a.CreatedDate,
                                createdBy = new
                                {
                                    id = a.CreatedBy.Id,
                                    firstName = a.CreatedBy.FirstName,
                                    lastName = a.CreatedBy.LastName,
                                    email = a.CreatedBy.Email
                                },
                            }).ToList();

                foreach (var policy in policyList)
                {
                    policy.fileNames.ToList();
                }

                var result = new
                {
                    count = countOfUsers,
                    countOfProcessing = countOfProcessing,
                    data = policyList
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Get all policies. Available for admin;
        /// </summary>
        /// <param name="status"> Unconfirmed = 0, Confirmed = 1, Rejected = 2 </param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [ActionName("ViewAll")]
        public IHttpActionResult ViewAll(PolicyStatus status, int offset = 0, int count = 10, string q = null)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                IQueryable<Policy> query = unitOfWork.PolicyRepository.GetAll();

                if (status != PolicyStatus.None)
                {
                    query = query.Where(a => a.Status == status);
                }

                if (!string.IsNullOrEmpty(q))
                {
                    query = query.Where(e =>
                               e.PolicyNumber == q
                            || e.CreatedBy.FirstName.StartsWith(q)
                            || e.CreatedBy.LastName.StartsWith(q)
                            || e.CreatedBy.Email == q);
                }

                var countOfUsers = query.Count();

                var policyList = query
                            .OrderByDescending(a => a.CreatedDate)
                            .Skip(offset)
                            .Take(count)
                            .ToList()
                            .Select(a => new
                            {
                                id = a.Id,
                                status = a.Status,
                                policyNumber = a.PolicyNumber,
                                insurer = a.Insurer == null ? null : new
                                {
                                    name = a.Insurer.Name
                                },
                                //fileNames = a.PolicyFiles.Where(f => f.Status != FileStatus.deleted).Select(f => f.File.FileName),
                                createdDate = a.CreatedDate,
                                createdBy = new
                                {
                                    id = a.CreatedBy.Id,
                                    firstName = a.CreatedBy.FirstName,
                                    lastName = a.CreatedBy.LastName,
                                    email = a.CreatedBy.Email
                                }
                            }).ToList();

                //foreach (var policy in policyList)
                //{
                //    policy.fileNames.ToList();
                //}

                var result = new
                {
                    count = countOfUsers,
                    data = policyList
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Create Policy by Customer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Create")]
        public async Task<IHttpActionResult> Create([FromBody] CreatePolicyModel model)
        {
            if (model.FileIds != null && model.FileIds.Count == 0)
            {
                ModelState.AddModelError("fileIds", "files is required");
            }

            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();

                    var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == userId);

                    var files = unitOfWork.FileRepository.GetAll().Where(f => model.FileIds.Contains(f.Id)).ToList();

                    if (files.Count != model.FileIds.Count)
                    {
                        ModelState.AddModelError("fileIds", "files contains wrong file id");
                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }

                    unitOfWork.BeginTransaction();

                    foreach (var file in files)
                    {
                        file.IsTemp = false;
                        unitOfWork.FileRepository.Edit(file);
                    }

                    Policy policy = new Policy();
                    policy.CreatedDate = DateTime.Now;
                    policy.CreatedById = userId;
                    policy.Status = PolicyStatus.Unconfirmed;

                    unitOfWork.PolicyRepository.Insert(policy);
                    await unitOfWork.SaveAsync();

                    ISchedulerFactory factory = new StdSchedulerFactory();
                    IScheduler scheduler = factory.GetScheduler();
                    JobDataMap dataMap = new JobDataMap();

                    if (!string.IsNullOrEmpty(user.AutopilotContactId))
                    {
                        dataMap["autopilotContactId"] = user.AutopilotContactId;
                        dataMap["policyauthorId"] = userId;
                        dataMap["OperationType"] = "PolicyLoad";

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

                    var policyFiles = files.Select(file => new PolicyFile
                    {
                        PolicyId = policy.Id,
                        FileId = file.Id
                    }).ToList();

                    unitOfWork.PolicyFileRepository.InsertRange(policyFiles);
                    await unitOfWork.SaveAsync();

                    unitOfWork.CommitTransaction();

                    var result = new
                    {
                        id = policy.Id
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
        /// Adds new files to existing policy.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddNewFiles")]
        public async Task<IHttpActionResult> AddNewFiles([FromBody] AddFilesModel model)
        {
            if (model.FileIds != null && model.FileIds.Count == 0)
            {
                ModelState.AddModelError("fileIds", "files is required");
            }

            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var files = unitOfWork.FileRepository.GetAll().Where(f => model.FileIds.Contains(f.Id)).ToList();

                if (files.Count != model.FileIds.Count)
                {
                    ModelState.AddModelError("fileIds", "files contains wrong file id");
                    return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                }

                var policy = unitOfWork.PolicyRepository.GetAll().FirstOrDefault(p => p.Id == model.Id);

                if (policy == null)
                {
                    ModelState.AddModelError("policy", "wrong policy id");
                    return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                }

                unitOfWork.BeginTransaction();

                foreach (var file in files)
                {
                    file.IsTemp = false;
                    unitOfWork.FileRepository.Edit(file);
                }

                var policyFiles = files.Select(file => new PolicyFile
                {
                    PolicyId = policy.Id,
                    FileId = file.Id
                }).ToList();

                unitOfWork.PolicyFileRepository.InsertRange(policyFiles);

                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                return Ok();
            }
        }

        /// <summary>
        /// Deletes file from policy. Accessible only for ADMIN.
        /// </summary>
        /// <param name="id">policy id</param>
        /// <param name="fileId">file to delete id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ActionName("DeleteFileFromPolicy")]
        public async Task<IHttpActionResult> DeleteFileFromPolicy(string id, string fileId)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                int policyId = int.Parse(id);
                Guid policyFileId = Guid.Parse(fileId);

                var policy = unitOfWork.PolicyRepository.GetAll().FirstOrDefault(p => p.Id == policyId);
                var policyfile = unitOfWork.PolicyFileRepository.GetAll().FirstOrDefault(f => f.FileId == policyFileId && f.PolicyId == policyId);

                if (policy == null)
                {
                    ModelState.AddModelError("policy", "policy was not found");
                    return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                }

                if (policyfile == null)
                {
                    ModelState.AddModelError("file", "policy file was not found");
                    return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                }

                unitOfWork.BeginTransaction();

                var file = unitOfWork.FileRepository.GetAll().FirstOrDefault(f => f.Id == policyfile.FileId);
                file.IsTemp = true;

                unitOfWork.FileRepository.Edit(file);
                unitOfWork.PolicyFileRepository.Delete(policyfile);

                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                return Ok();
            }
        }


        /// <summary>
        /// Get policy types with template type by insurer id;
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Groups")]
        public IHttpActionResult GetPolicyTypes(int insurerId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var groups = unitOfWork.PolicyTypeRepository.GetAll().Where(g=>g.InsurerId == insurerId && g.Status != Status.Hide).GroupBy(a => a.GroupName)
                    .Select(a => new
                    {
                        groupName = a.Key,
                        categories = a.Select(g => new
                        {
                            id = g.Id,
                            name = g.PolicyTypeName,
                            policyTemplate = g.PolicyGroupType
                        })
                    }
                    ).ToList();

                return Json(groups);
            }
        }


        /// <summary>
        /// Get additional properties for policy group;
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Policy/GroupFields/Description")]
        public IHttpActionResult PolicyGroupFieldsDescription(int policyGroupId, int policyId = 0)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                List<AdditionalPolicyProperty> policyGroupFields = new List<AdditionalPolicyProperty>();

                if (policyId != 0)
                {
                    policyGroupFields = unitOfWork.AdditionalPolicyPropertyRepository.GetAll().Where(a => a.PolicyId == policyId).ToList();
                }

                var groupFields = unitOfWork.PolicyTypeFieldRepository.GetAll().Where(a => a.PolicyTypeId == policyGroupId && a.Status != FieldStatus.Deleted).ToList();

                List<dynamic> data = new List<dynamic>();

                foreach (var item in groupFields)
                {
                    var policyField = policyGroupFields.FirstOrDefault(a => a.PolicyTypeFieldId == item.Id);

                    var currentValue = policyField != null ? policyField.Value : item.DefaultValue;


                    List<dynamic> listDescription = null;
                    if (item.ListDesription != null)
                    {
                        listDescription = new List<dynamic>();

                        var list = JsonConvert.DeserializeObject<List<string>>(item.ListDesription);

                        int s = 0;
                        foreach (var listItem in list)
                        {
                            var rl = new
                            {
                                id = s,
                                value = listItem
                            };
                            s++;
                            listDescription.Add(rl);
                        }
                    }

                    dynamic value = currentValue;
                    
                    if (policyField != null && policyField.Value != null)
                    {
                        switch (item.FieldType)
                        {
                            case PolicyTypeFieldType.String:
                                break;
                            case PolicyTypeFieldType.Int:
                                {
                                    int result = 0;
                                    int.TryParse(currentValue, out result);
                                    value = result;
                                }
                                break;
                            case PolicyTypeFieldType.Bool:
                                {
                                    bool result = false;
                                    bool.TryParse(currentValue, out result);

                                    value = new
                                    {
                                        id = result,
                                        name = result == true ? "Yes" : "No"
                                    };

                                }
                                break;
                            case PolicyTypeFieldType.Float:
                                {
                                    double result = 0;
                                    double.TryParse(currentValue, out result);
                                    value = result;
                                }
                                break;
                            case PolicyTypeFieldType.List:

                                value = listDescription.FirstOrDefault(o => o.value == currentValue);
                                break;
                            default:
                                throw new NotImplementedException("This type is absent");
                        }
                    }
                    
                    var r = new
                    {
                        id = item.Id,
                        fieldType = item.FieldType,
                        fieldName = item.FieldName,
                        listDescription = listDescription,
                        value = value
                    };
                    data.Add(r);
                }

                return Json(data);
            }
        }


        /// <summary>
        /// Update policy. For Admin Role Only
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Update")]
        public async Task<IHttpActionResult> Update([FromBody] UpdatePolicyModel model)
        {
            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    unitOfWork.BeginTransaction();

                    var userId = User.Identity.GetUserId();

                    //Update policy table
                    var policy = unitOfWork.PolicyRepository.GetById(model.Id);
                    policy.InsurerId = model.InsurerId;
                    policy.PolicyTypeId = model.PolicyGroupId;
                    policy.PolicyNumber = model.PolicyNumber;
                    policy.PolicyPremium = model.PolicyPremium;
                    policy.PolicyPaymentFrequency = model.PolicyPaymentFrequency;
                    policy.PolicyPaymentAmount = model.PolicyPaymentAmount;
                    policy.PolicyExcess = model.PolicyExcess;
                    policy.StartDate = model.StartDate;
                    policy.EndDate = model.EndDate;
                    policy.Description = model.Description;
                    policy.ModifiedById = userId;
                    policy.ModifiedDate = DateTime.Now;

                    if (model.IsConfirmed == true)
                    {
                        var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == policy.CreatedById);

                        ISchedulerFactory factory = new StdSchedulerFactory();
                        IScheduler scheduler = factory.GetScheduler();
                        JobDataMap dataMap = new JobDataMap();

                        dataMap["autopilotContactId"] = user.AutopilotContactId;
                        dataMap["OperationType"] = "TriggerJourney";
                        dataMap["journeyName"] = ConfigurationManager.AppSettings["PolicyConfirmedJourney"];
                        dataMap["userId"] = user.Id;

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

                        policy.Status = PolicyStatus.Confirmed;
                    }

                    var vehicle = unitOfWork.Vehicle_PRepository.GetAll().Where(a => a.PolicyId == policy.Id).SingleOrDefault();
                    var home = unitOfWork.Home_PRepository.GetAll().Where(a => a.PolicyId == policy.Id).SingleOrDefault();
                    var life = unitOfWork.Life_PRepository.GetAll().Where(a => a.PolicyId == policy.Id).SingleOrDefault();
                    
                    //Update policy type tables
                    if (model.VehiclePolicyModel != null)
                    {
                        //Delete old templates
                        if (home != null) unitOfWork.Home_PRepository.Delete(home);
                        if (life != null) unitOfWork.Life_PRepository.Delete(life);


                        bool needToCreate = false;
                        
                        if (vehicle == null)
                        {
                            needToCreate = true;
                            vehicle = new Vehicle_P();
                            vehicle.PolicyId = policy.Id;
                            vehicle.CreatedById = userId;
                            vehicle.CreatedDate = DateTime.Now;
                        }

                        var manufacturer = unitOfWork.VehicleMakeRepository.GetAll().FirstOrDefault(m => m.MakeName == model.VehiclePolicyModel.Make);

                        if (manufacturer == null)
                        {
                            VehicleMake maker = new VehicleMake();
                            maker.MakeName = model.VehiclePolicyModel.Make;
                            maker.Status = 0;
                            maker.Description = "";

                            unitOfWork.VehicleMakeRepository.Insert(maker);
                            await unitOfWork.SaveAsync();

                            vehicle.Make = maker;
                            vehicle.MakeId = maker.Id;
                        }
                        else
                        {
                            vehicle.Make = manufacturer;
                            vehicle.MakeId = manufacturer.Id;
                        }

                        var dbModel = unitOfWork.VehicleModelRepository.GetAll().FirstOrDefault(m => m.ModelName == model.VehiclePolicyModel.Model && m.VehicleMakeId == vehicle.Make.Id);

                        if (dbModel == null)
                        {
                            VehicleModel vm = new VehicleModel();
                            vm.ModelName = model.VehiclePolicyModel.Model;
                            vm.VehicleMakeId = vehicle.MakeId;

                            unitOfWork.VehicleModelRepository.Insert(vm);
                            await unitOfWork.SaveAsync();

                            vehicle.Model = vm;
                            vehicle.ModelId = vm.Id;
                        }
                        else
                        {
                            vehicle.Model = dbModel;
                            vehicle.ModelId = dbModel.Id;
                        }

                        //Update Policy Title
                        policy.Title = vehicle.RegistrationNumber;
                        policy.SubTitle = string.Format("{0}, {1}", model.VehiclePolicyModel.Make, model.VehiclePolicyModel.Model);



                        vehicle.Year = model.VehiclePolicyModel.Year.Value;
                        vehicle.Mileage = model.VehiclePolicyModel.Mileage.Value;
                        vehicle.RegistredDriverName = model.VehiclePolicyModel.RegistredDriverName;
                        vehicle.RegistrationNumber = model.VehiclePolicyModel.RegistrationNumber;
                        vehicle.RegistrationState = model.VehiclePolicyModel.RegistrationState;
                        vehicle.IsCommercial = model.VehiclePolicyModel.IsCommercial.Value;


                        if (needToCreate)
                        {
                            unitOfWork.Vehicle_PRepository.Insert(vehicle);
                        }
                        else
                        {
                            vehicle.ModifiedById = userId;
                            vehicle.ModifiedDate = DateTime.Now;
                            unitOfWork.Vehicle_PRepository.Edit(vehicle);
                        }
                    }
                    else if (model.HomePolicyModel != null)
                    {
                        //Delete old templates
                        if (vehicle != null) unitOfWork.Vehicle_PRepository.Delete(vehicle);
                        if (life != null) unitOfWork.Life_PRepository.Delete(life);

                        bool needToCreate = false;
                        bool needToCreateAddress = false;
                       
                        if (home == null)
                        {
                            needToCreate = true;
                            home = new Home_P();
                            home.PolicyId = policy.Id;
                            home.CreatedById = userId;
                            home.CreatedDate = DateTime.Now;
                        }

                        if (home.Address == null)
                        {
                            needToCreateAddress = true;
                            home.Address = new Address();
                            home.Address.CreatedById = userId;
                            home.Address.CreatedDate = DateTime.Now;
                        }

                        //Update Policy Title
                        policy.Title = "Home";
                        policy.SubTitle = model.HomePolicyModel.Address;
                      

                        home.BuildDate = model.HomePolicyModel.BuildDate.Value;
                        home.Address.Address_1 = model.HomePolicyModel.Address;

                        if (needToCreateAddress)
                        {
                            unitOfWork.AddressRepository.Insert(home.Address);
                        }
                        else
                        {
                            home.Address.ModifiedById = userId;
                            home.Address.ModifiedDate = DateTime.Now;
                            unitOfWork.AddressRepository.Edit(home.Address);
                        }
                        
                        if (needToCreate)
                        {
                            home.AddressId = home.Address.Id;
                            unitOfWork.Home_PRepository.Insert(home);
                        }
                        else
                        {
                            home.ModifiedById = userId;
                            home.ModifiedDate = DateTime.Now;
                            unitOfWork.Home_PRepository.Edit(home);
                        }
                    }
                    else if (model.LifePolicyModel != null)
                    {
                        //Delete old templates
                        if (home != null) unitOfWork.Home_PRepository.Delete(home);
                        if (vehicle != null) unitOfWork.Vehicle_PRepository.Delete(vehicle);

                        bool needToCreate = false;
                       
                        if (life == null)
                        {
                            needToCreate = true;
                            life = new Life_P();
                            life.PolicyId = policy.Id;
                            life.CreatedById = userId;
                            life.CreatedDate = DateTime.Now;
                        }
                        
                        //Update Policy Title
                        policy.Title = life.Medication;
                        policy.SubTitle = life.MedicationCondition;

                        life.Medication = model.LifePolicyModel.Medication;
                        life.MedicationCondition = model.LifePolicyModel.MedicationCondition;
                      
                        if (needToCreate)
                        {
                            unitOfWork.Life_PRepository.Insert(life);
                        }
                        else
                        {
                            life.ModifiedById = userId;
                            life.ModifiedDate = DateTime.Now;
                            unitOfWork.Life_PRepository.Edit(life);
                        }
                    }

                    //Update Policy
                    unitOfWork.PolicyRepository.Edit(policy);
                    
                    //Delete Additional Fields
                    foreach (var item in policy.AdditionalPolicyFields.ToList())
                    {
                        unitOfWork.AdditionalPolicyPropertyRepository.Delete(item);
                    }

                    await unitOfWork.SaveAsync();

                    //Add Additional Fields
                    foreach (var modelProperty in model.AdditionalProperties.ToList())
                    {
                        var field = unitOfWork.PolicyTypeFieldRepository.GetAll().FirstOrDefault(f => f.Id == modelProperty.GroupFieldId);

                        AdditionalPolicyProperty property = new AdditionalPolicyProperty();
                        property.PolicyId = policy.Id;
                        property.PolicyTypeFieldId = modelProperty.GroupFieldId;
                        
                        if (field != null && field.FieldType == PolicyTypeFieldType.List)
                        {
                            property.Value = string.Format("{0}", modelProperty.Value.value);
                        }
                        else
                        {
                            property.Value = string.Format("{0}", modelProperty.Value);
                        }

                        unitOfWork.AdditionalPolicyPropertyRepository.Insert(property);
                    }

                    try
                    {
                        await unitOfWork.SaveAsync();
                    }
                    catch (Exception e)
                    {
                        return JsonError(HttpStatusCode.BadRequest, 10, "Error " + e.Message, null);
                    }    
                
                    unitOfWork.CommitTransaction();

                    return Ok();
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

        /// <summary>
        /// Gets all manufacturers of vehicles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Policy/Vehicle/GetMakes")]
        [ActionName("GetMakes")]
        public IHttpActionResult GetMakes()
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var list = unitOfWork.VehicleMakeRepository.GetAll().
                    Select(vm => new
                    {
                        id = vm.Id,
                        name = vm.MakeName,
                        status = vm.Status,
                        description = vm.Description
                    }).ToList();

                var result = new
                {
                    count = list.Count,
                    data = list
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Gets all models for vehicle manufacturer.
        /// </summary>
        /// <param name="id">manufacturer id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Policy/Vehicle/GetModels")]
        [ActionName("GetModels")]
        public IHttpActionResult GetModels(string id)
        {
            int manufacturerId = int.Parse(id);
            using (var unitOfWork = UnitOfWork.Create())
            {
                var list = unitOfWork.VehicleModelRepository.GetAll().Where(vm => vm.VehicleMakeId == manufacturerId).
                    Select(vm => new
                    {
                        id = vm.Id,
                        name = vm.ModelName,
                        status = vm.Status,
                        manudacturerId = vm.VehicleMakeId
                    }).ToList();

                var result = new
                {
                    count = list.Count,
                    data = list
                };

                return Json(result);
            }
        }

    }

    public enum PolicySortType
    {
        All = 0,
        Active = 1,
        Expired = 2
    }
}

