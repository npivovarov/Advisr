using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Models;
using Advisr.Web.Quartz;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
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
                        prePolicyType = a.PrePolicyType,
                        policyTypeId = a.PolicyTypeId,
                        insurerId = a.InsurerId,
                        startDate = a.StartDate,
                        endDate = a.EndDate,
                        policyNumber = a.PolicyNumber,
                        policyPremium = a.PolicyPremium,
                        policyExcess = a.PolicyExcess,
                        policyPaymentAmount = a.PolicyPaymentAmount,
                        policyPaymentFrequency = a.PolicyPaymentFrequency,
                        lifeInsurancePremiumType = a.PolicyPolicyProperties.Where(f => f.PolicyProperty.Status != FieldStatus.Deleted && f.PolicyProperty.FieldName == "Life Insurance Premium Type")
                        .Select(f => new
                        {
                            value = f.Value
                        }).FirstOrDefault(),
                        policyGroup = a.PolicyType == null ? null : new
                        {
                            id = a.PolicyTypeId,
                            groupName = a.PolicyType.PolicyGroup.Name,
                            policyTypeName = a.PolicyType.PolicyTypeName,
                            policyTemplateId = a.PolicyType.PolicyTemplateId,
                            description = a.PolicyType.Description
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
                            description = a.Insurer.Description
                        },
                        files = a.PolicyFiles.Where(f => f.Status != FileStatus.deleted).Select(p => new
                        {
                            id = p.FileId,
                            fileName = p.File.FileName,
                            fileSize = p.File.FileSize,
                            uploadedDate = p.File.CreatedDate,
                            url = string.Concat(startLinkToPhoto, "/", p.FileId),
                        }),
                        properties = a.PolicyPolicyProperties.Where(p=>p.PolicyProperty.Status != FieldStatus.Deleted).OrderBy(p => p.PolicyProperty.OrderIndex).Select(af => new
                        {
                            id = af.Id,
                            fieldName = af.PolicyProperty.FieldName,
                            fieldType = af.PolicyProperty.FieldType,
                            propertyType = af.PolicyProperty.PropertyType,
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

                var files = policyDb.files.ToList();
                var properties = policyDb.properties.ToList();

                return Json(policyDb);
            }
        }


        /// <summary>
        /// Get coverages list for the policy;
        /// </summary>
        /// <param name="id">Policy Id</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetCoverages")]
        public IHttpActionResult GetCoverages(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var result = unitOfWork.PolicyCoverageRepository.GetAll()
                    .Where(a => a.PolicyId == id)
                    .Select(a => new
                    {
                        id = a.Id,
                        title = a.Coverage.Title,
                        type = a.Coverage.Type,
                        description = a.Coverage.Description,
                        insurerId = a.Coverage.InsurerId,
                        isActive = a.IsActive,
                        excess = a.Excess,
                        limit = a.Limit,
                    }).ToList();

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
                        insurer = a.Insurer == null ? null : new
                        {
                            id = a.InsurerId,
                            name = a.Insurer.Name,
                            url = a.Insurer.URL,
                            phone = a.Insurer.Phone,
                            phoneOverseas = a.Insurer.PhoneOverseas,
                            email = a.Insurer.Email,
                            description = a.Insurer.Description
                        },
                        description = a.PolicyType.Description,
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
        public IHttpActionResult List(PolicyStatus status = PolicyStatus.None, PolicySortType sortType = PolicySortType.All, int offset = 0, int count = 10, string groupName = null, string q = null)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var startLinkToLogo = Url.Link("Default", new { controller = "files", action = "getLogo" });
                
                IQueryable<Policy> query = unitOfWork.PolicyRepository.GetAll()
                   .Where(a => a.CreatedById == userId && a.Status != PolicyStatus.Hidden);

                if (!string.IsNullOrEmpty(groupName))
                {
                    query = query.Where(e =>
                            e.PolicyType != null && e.PolicyType.PolicyGroup.Name == groupName);
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

                if (sortType == PolicySortType.Active)
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
                                prePolicyType = a.PrePolicyType,
                                policyPaymentAmount = a.PolicyPaymentAmount,
                                policyPaymentFrequency = a.PolicyPaymentFrequency,
                                policyTotalPremiumAmount = a.PolicyPremium,
                                policyNumber = a.PolicyNumber,
                                endDate = a.EndDate,
                                fileNames = a.PolicyFiles.Where(f => f.Status != FileStatus.deleted).Select(f => f.File.FileName),
                                lifeInsurancePremiumType = a.PolicyPolicyProperties.Where(f => f.PolicyProperty.Status != FieldStatus.Deleted && f.PolicyProperty.FieldName == "Life Insurance Premium Type")
                                .Select(f => new
                                {
                                  value = f.Value
                                }).FirstOrDefault(),
                                policyGroup = a.PolicyType == null ? null : new
                                {
                                    groupName = a.PolicyType.PolicyGroup.Name,
                                    policyType = a.PolicyType.PolicyTypeName,
                                    policyTemplate = a.PolicyType.PolicyTemplate.Name,
                                },
                                insurer = a.Insurer == null ? null : new
                                {
                                    name = a.Insurer.Name,
                                    color = a.Insurer.Color,
                                    logoUrl = string.Concat(startLinkToLogo, "?id=", a.Insurer.LogoFileId),
                                    description = a.Insurer.Description
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
                    policy.PrePolicyType = model.PrePolicyType;
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

                if (policy.PrePolicyType != model.PrePolicyType)
                {
                    policy.PrePolicyType = model.PrePolicyType;
                    unitOfWork.PolicyRepository.Edit(policy);
                }

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
        [ActionName("PolicyTypes")]
        public IHttpActionResult GetPolicyTypes(int insurerId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var groups = unitOfWork.PolicyTypeRepository.GetAll().Where(g => g.InsurerId == insurerId && g.Status != PolicyTypeStatus.Hide).GroupBy(a => a.PolicyGroup.Name)
                    .Select(a => new
                    {
                        groupName = a.Key,
                        policyTypes = a.Select(g => new
                        {
                            id = g.Id,
                            policyGroupId = g.PolicyGroupId,
                            policyTemplateId = g.PolicyTemplateId,
                            groupName = g.PolicyGroup.Name,
                            name = g.PolicyTemplate.Name + " - " + g.PolicyTypeName,
                        })
                    }
                    ).ToList();

                return Json(groups);
            }
        }

        /// <summary>
        /// Get coverages for the policy type;
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Policy/Coverages/Description")]
        public IHttpActionResult PolicyCoveragesDescription(int policyTypeId, int policyId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var policyCoverages = unitOfWork.PolicyCoverageRepository.GetAll().Where(a => a.PolicyId == policyId).ToList();

                var policyTypeCoverages = unitOfWork.PolicyTypeCoverageRepository.GetAll().Where(a => a.PolicyTypeId == policyTypeId && a.Status == PolicyTypeCoverageStatus.None).ToList();

                List<dynamic> data = new List<dynamic>();

                foreach (var item in policyTypeCoverages)
                {
                    var policyCoverage = policyCoverages.FirstOrDefault(a => a.CoverageId == item.CoverageId);

                    var r = new
                    {
                        isSelected = policyCoverage != null,
                        coverageId = item.CoverageId,
                        title = item.Coverage.Title,
                        type = item.Coverage.Type,
                        description = item.Coverage.Description,
                        isActive = policyCoverage == null ? false : policyCoverage.IsActive,
                        limit = policyCoverage == null ? null : policyCoverage.Limit,
                        excess = policyCoverage == null ? null : policyCoverage.Excess,
                    };
                    data.Add(r);
                }

                return Json(data);
            }
        }


        /// <summary>
        /// Get additional properties for policy type;
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Policy/GroupFields/Description")]
        public IHttpActionResult PolicyPropertiesDescription(int policyTypeId, int policyId, PropertyType propertyType)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                List<PolicyPolicyProperty> policyPolicyProperties = new List<PolicyPolicyProperty>();

                if (policyId != 0)
                {
                    policyPolicyProperties = unitOfWork.PolicyPolicyPropertyRepository.GetAll().Where(a => a.PolicyId == policyId).ToList();
                }

                List<PolicyProperty> properties = new List<PolicyProperty>();

                if (propertyType == PropertyType.AdditionalToPolicyType || propertyType == PropertyType.None)
                {
                    var policyTypeAdditionalProperties = unitOfWork.PolicyTypePolicyPropertyRepository.GetAll()
                        .Where(a => a.PolicyTypeId == policyTypeId && a.Status != PolicyTypePolicyPropertyStatus.Deleted)
                        .Select(a => a.PolicyProperty)
                        .OrderBy(a => a.OrderIndex)
                        .ToList();

                    properties.AddRange(policyTypeAdditionalProperties);
                }

                if (propertyType == PropertyType.PolicyTemplate || propertyType == PropertyType.None)
                {
                    var templateProperties = unitOfWork.PolicyTypeRepository.GetAll().Where(a => a.Id == policyTypeId).Select(a => a.PolicyTemplate.PolicyTemplatePolicyProperties.Select(p => p.PolicyProperty)).First().ToList();
                    properties.AddRange(templateProperties);
                }

                List<dynamic> data = new List<dynamic>();

                foreach (var property in properties)
                {
                    var policyPropertyValue = policyPolicyProperties.FirstOrDefault(a => a.PolicyPropertyId == property.Id);

                    var currentValue = policyPropertyValue != null ? policyPropertyValue.Value : property.DefaultValue;

                    if (property.FieldType == PolicyTypeFieldType.Bool && (currentValue != "Yes" && currentValue != "No"))
                    {
                        currentValue = null;
                    }

                    List<dynamic> listDescription = null;
                    if (property.ListDesription != null)
                    {
                        listDescription = new List<dynamic>();

                        var list = JsonConvert.DeserializeObject<List<string>>(property.ListDesription);

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

                    if (policyPropertyValue != null && policyPropertyValue.Value != null)
                    {
                        switch (property.FieldType)
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
                                    bool result = currentValue == "Yes";
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
                            case PolicyTypeFieldType.Date:
                                {
                                    DateTime result = default(DateTime);
                                    if (DateTime.TryParse(currentValue, out result))
                                    {
                                        value = result;
                                    }
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
                        id = property.Id,
                        fieldType = property.FieldType,
                        fieldName = property.FieldName,
                        isRequired = property.IsRequired,
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
        [Authorize(Roles = "ADMIN")]
        [ActionName("Update")]
        public async Task<IHttpActionResult> Update([FromBody] UpdatePolicyModel model)
        {
            if (model.StartDate.HasValue && model.EndDate.HasValue && model.StartDate.Value.Date >= model.EndDate.Value.Date)
            {
                ModelState.AddModelError("startDate", "Start date cannot be after or equal End date");
            }

            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    unitOfWork.BeginTransaction();

                    var userId = User.Identity.GetUserId();

                    var policyPropertyModelIds = model.PolicyProperties.Select(p => p.PropertyId).ToList();

                    var policyProperties = unitOfWork.PolicyPropertyRepository.GetAll()
                                            .Where(a => policyPropertyModelIds.Contains(a.Id))
                                            .ToList();

                    var policyTemplate = unitOfWork.PolicyTypeRepository.GetAll().Where(a => a.Id == model.PolicyTypeId).Select(a => a.PolicyTemplate).First();

                    //Update policy table
                    var policy = unitOfWork.PolicyRepository.GetById(model.Id);
                    policy.InsurerId = model.InsurerId;
                    policy.PolicyTypeId = model.PolicyTypeId;
                    policy.PolicyNumber = model.PolicyNumber;
                    policy.PolicyPremium = model.PolicyPremium;
                    policy.PolicyPaymentFrequency = model.PolicyPaymentFrequency;
                    policy.PolicyPaymentAmount = model.PolicyPaymentAmount;
                    policy.PolicyExcess = model.PolicyExcess;
                    policy.StartDate = model.StartDate;
                    policy.EndDate = model.EndDate;
                    //policy.Description = model.Description;
                    policy.ModifiedById = userId;
                    policy.ModifiedDate = DateTime.Now;

                    //Get title and subtitle from properties
                    
                    var titleParts = (from templateProperty in policyProperties
                                      join modelProperty in model.PolicyProperties on templateProperty.Id equals modelProperty.PropertyId
                                      where templateProperty.IsTitle == true
                                      select modelProperty.GetValueAsString(templateProperty.FieldType)).ToList();

                    var subtitleParts = (from templateProperty in policyProperties
                                         join modelProperty in model.PolicyProperties on templateProperty.Id equals modelProperty.PropertyId
                                         where templateProperty.IsSubtitle == true
                                         select modelProperty.GetValueAsString(templateProperty.FieldType)).ToList();
                    
                    titleParts.Insert(0, policyTemplate.Title);
                    subtitleParts.Insert(0, policyTemplate.Subtitle);

                    policy.Title = string.Join(" ", titleParts);
                    policy.SubTitle = string.Join(" ", subtitleParts);

                    //Update Policy
                    unitOfWork.PolicyRepository.Edit(policy);

                    #region Autopilot

                    if (model.IsConfirmed == true)
                    {
                        var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == policy.CreatedById);

                        if (user.AutopilotTrack)
                        {
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
                        }

                        policy.Status = PolicyStatus.Confirmed;
                    }
                    #endregion

                    //Delete Current Properties
                    foreach (var item in policy.PolicyPolicyProperties.ToList())
                    {
                        unitOfWork.PolicyPolicyPropertyRepository.Delete(item);
                    }

                    await unitOfWork.SaveAsync();

                    ///----------------------------------
                    ///Add new Properties 
                    ///----------------------------------
                    {
                        List<PolicyPropertyModel> propertiesFromModel = new List<PolicyPropertyModel>();

                        propertiesFromModel.AddRange(model.PolicyProperties);
                        if (model.AdditionalProperties != null)
                        {
                            propertiesFromModel.AddRange(model.AdditionalProperties);
                        }

                        foreach (var modelProperty in propertiesFromModel)
                        {
                            var propertyType = unitOfWork.PolicyPropertyRepository.GetAll().Where(f => f.Id == modelProperty.PropertyId)
                                .Select(a => new
                                {
                                    fieldType = a.FieldType
                                }).FirstOrDefault();

                            PolicyPolicyProperty property = new PolicyPolicyProperty();
                            property.PolicyId = policy.Id;
                            property.PolicyPropertyId = modelProperty.PropertyId;
                            property.Value = modelProperty.GetValueAsString(propertyType.fieldType);

                            unitOfWork.PolicyPolicyPropertyRepository.Insert(property);
                        }

                        unitOfWork.PolicyRepository.Edit(policy);
                    }

                    ///----------------------------------
                    ///Coverages for the policy
                    ///----------------------------------
                    {
                        ///Delete Existing Coverages
                        foreach (var item in policy.PolicyCoverages.ToList())
                        {
                            unitOfWork.PolicyCoverageRepository.Delete(item);
                        }

                        await unitOfWork.SaveAsync();

                        if (model.Coverages != null)
                        {
                            //Add new coverages
                            foreach (var modelCoverage in model.Coverages.ToList())
                            {
                                PolicyCoverage policyCoverage = new PolicyCoverage();
                                policyCoverage.PolicyId = policy.Id;
                                policyCoverage.CoverageId = modelCoverage.CoverageId;
                                policyCoverage.IsActive = modelCoverage.IsActive.Value;
                                policyCoverage.Excess = modelCoverage.Excess;
                                policyCoverage.Limit = modelCoverage.Limit;
                                policyCoverage.CreatedById = userId;
                                policyCoverage.CreatedDate = DateTime.Now;

                                unitOfWork.PolicyCoverageRepository.Insert(policyCoverage);
                            }
                        }
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
        /// Hide the policy for customer
        /// </summary>
        /// <param name="id">policy id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ActionName("HideForCustomer")]
        public async Task<IHttpActionResult> HideForCustomer(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var policy = unitOfWork.PolicyRepository.GetById(id);

                if(policy.Status != PolicyStatus.Confirmed)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Policy must have status Confirmed", null);
                }

                policy.Status = PolicyStatus.Hidden;
                policy.ModifiedById = User.Identity.GetUserId();
                policy.ModifiedDate = DateTime.Now;

                unitOfWork.PolicyRepository.Edit(policy);
                await unitOfWork.SaveAsync();
            }

            return Ok();
        }


        /// <summary>
        /// Show the policy for customer
        /// </summary>
        /// <param name="id">policy id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ActionName("ShowForCustomer")]
        public async Task<IHttpActionResult> ShowForCustomer(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var policy = unitOfWork.PolicyRepository.GetById(id);

                if (policy.Status != PolicyStatus.Hidden)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Policy must have status Hidden", null);
                }

                policy.Status = PolicyStatus.Confirmed;
                policy.ModifiedById = User.Identity.GetUserId();
                policy.ModifiedDate = DateTime.Now;

                unitOfWork.PolicyRepository.Edit(policy);
                await unitOfWork.SaveAsync();
            }

            return Ok();
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

