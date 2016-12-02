using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using System.Drawing.Imaging;

namespace Advisr.Web.Controllers
{
    /// <summary>
    /// Awailable only for Admin role.
    /// </summary>
    [Authorize(Roles = "ADMIN")]
    public class InsurerController : BaseApiController
    {
        /// <summary>
        /// Creates an insurer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Add")]
        public async Task<IHttpActionResult> Add([FromBody] InsurerModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Need to fill all required fields.", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == userId);
                var file = unitOfWork.FileRepository.GetAll().FirstOrDefault(f => f.Id == model.LogoId);

                if (user == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "User doesn't exist.", ModelState);
                }

                if (file == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "File doesn't exist.", ModelState);
                }

                SaveFileAsLogo(unitOfWork, file);

                unitOfWork.BeginTransaction();

                var insurer = new Insurer();
                insurer.Name = model.Name;
                insurer.URL = model.URL;
                insurer.Phone = model.Phone;
                insurer.PhoneOverseas = model.PhoneOverseas;
                insurer.LogoFileId = model.LogoId;
                insurer.Email = model.Email;
                insurer.Color = model.Color;
                insurer.CreatedBy = user;
                insurer.CreatedById = userId;
                insurer.CreatedDate = DateTime.Now;
                insurer.Status = InsurerStatus.Active;

                unitOfWork.InsurerRepository.Insert(insurer);
                await unitOfWork.SaveAsync();

                PolicyType vehicleCar = new PolicyType();
                vehicleCar.GroupName = "Vehicle";
                vehicleCar.PolicyTypeName = "Car";
                vehicleCar.PolicyGroupType = PolicyGroupType.Vehicle;
                vehicleCar.CreatedById = DbConstants.SystemUserId;
                vehicleCar.CreatedDate = DateTime.Now;
                vehicleCar.InsurerId = insurer.Id;
                vehicleCar.Status = Status.Active;
                unitOfWork.PolicyTypeRepository.Insert(vehicleCar);

                PolicyType vehicleMotorbike = new PolicyType();
                vehicleMotorbike.GroupName = "Vehicle";
                vehicleMotorbike.PolicyTypeName = "Motorbike";
                vehicleMotorbike.PolicyGroupType = PolicyGroupType.Vehicle;
                vehicleMotorbike.CreatedById = DbConstants.SystemUserId;
                vehicleMotorbike.CreatedDate = DateTime.Now;
                vehicleMotorbike.InsurerId = insurer.Id;
                vehicleMotorbike.Status = Status.Active;
                unitOfWork.PolicyTypeRepository.Insert(vehicleMotorbike);

                PolicyType meMedical = new PolicyType();
                meMedical.GroupName = "Me";
                meMedical.PolicyTypeName = "Medical";
                meMedical.PolicyGroupType = PolicyGroupType.Life;
                meMedical.CreatedById = DbConstants.SystemUserId;
                meMedical.CreatedDate = DateTime.Now;
                meMedical.InsurerId = insurer.Id;
                meMedical.Status = Status.Active;
                unitOfWork.PolicyTypeRepository.Insert(meMedical);

                PolicyType meTravel = new PolicyType();
                meTravel.GroupName = "Me";
                meTravel.PolicyTypeName = "Travel";
                meTravel.PolicyGroupType = PolicyGroupType.Life;
                meTravel.CreatedById = DbConstants.SystemUserId;
                meTravel.CreatedDate = DateTime.Now;
                meTravel.InsurerId = insurer.Id;
                meTravel.Status = Status.Active;
                unitOfWork.PolicyTypeRepository.Insert(meTravel);

                await unitOfWork.SaveAsync();

                PolicyTypeField vehicleCarColor = new PolicyTypeField();
                vehicleCarColor.PolicyTypeId = vehicleCar.Id;
                vehicleCarColor.FieldName = "Color";
                vehicleCarColor.FieldType = PolicyTypeFieldType.List;
                vehicleCarColor.ListDesription = "[\"red\",\"black\",\"white\",\"blue\",\"green\"]";
                vehicleCarColor.CreatedById = DbConstants.SystemUserId;
                vehicleCarColor.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleCarColor);

                PolicyTypeField vehicleBodyType = new PolicyTypeField();
                vehicleBodyType.PolicyTypeId = vehicleCar.Id;
                vehicleBodyType.FieldName = "Body Type";
                vehicleBodyType.FieldType = PolicyTypeFieldType.List;
                vehicleBodyType.ListDesription = "[\"off-road\",\"sedan\",\"station wagon\"]";
                vehicleBodyType.CreatedById = DbConstants.SystemUserId;
                vehicleBodyType.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleBodyType);

                PolicyTypeField vehicleCarImmobilaser = new PolicyTypeField();
                vehicleCarImmobilaser.PolicyTypeId = vehicleCar.Id;
                vehicleCarImmobilaser.FieldName = "Immobilaser";
                vehicleCarImmobilaser.FieldType = PolicyTypeFieldType.Bool;
                vehicleCarImmobilaser.DefaultValue = "0";
                vehicleCarImmobilaser.CreatedById = DbConstants.SystemUserId;
                vehicleCarImmobilaser.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleBodyType);

                PolicyTypeField vehicleCarNumberOfKeys = new PolicyTypeField();
                vehicleCarNumberOfKeys.PolicyTypeId = vehicleCar.Id;
                vehicleCarNumberOfKeys.FieldName = "Number Of Keys";
                vehicleCarNumberOfKeys.FieldType = PolicyTypeFieldType.Int;
                vehicleCarNumberOfKeys.CreatedById = DbConstants.SystemUserId;
                vehicleCarNumberOfKeys.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleCarNumberOfKeys);

                PolicyTypeField vehicleCarSecondDriver = new PolicyTypeField();
                vehicleCarSecondDriver.PolicyTypeId = vehicleCar.Id;
                vehicleCarSecondDriver.FieldName = "Second Driver";
                vehicleCarSecondDriver.FieldType = PolicyTypeFieldType.String;
                vehicleCarSecondDriver.CreatedById = DbConstants.SystemUserId;
                vehicleCarSecondDriver.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleCarSecondDriver);

                PolicyTypeField vehicleCarThirdDriver = new PolicyTypeField();
                vehicleCarThirdDriver.PolicyTypeId = vehicleCar.Id;
                vehicleCarThirdDriver.FieldName = "Third Driver";
                vehicleCarThirdDriver.FieldType = PolicyTypeFieldType.String;
                vehicleCarThirdDriver.CreatedById = DbConstants.SystemUserId;
                vehicleCarThirdDriver.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleCarThirdDriver);
                await unitOfWork.SaveAsync();

                PolicyTypeField vehicleMotorbikeColor = new PolicyTypeField();
                vehicleMotorbikeColor.PolicyTypeId = vehicleMotorbike.Id;
                vehicleMotorbikeColor.FieldName = "Color";
                vehicleMotorbikeColor.FieldType = PolicyTypeFieldType.List;
                vehicleMotorbikeColor.ListDesription = "[\"red\",\"black\",\"white\",\"blue\",\"green\"]";
                vehicleMotorbikeColor.CreatedById = DbConstants.SystemUserId;
                vehicleMotorbikeColor.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleMotorbikeColor);

                PolicyTypeField vehicleMotorbikeNumberOfKeys = new PolicyTypeField();
                vehicleMotorbikeNumberOfKeys.PolicyTypeId = vehicleMotorbike.Id;
                vehicleMotorbikeNumberOfKeys.FieldName = "Number Of Keys";
                vehicleMotorbikeNumberOfKeys.FieldType = PolicyTypeFieldType.Int;
                vehicleMotorbikeNumberOfKeys.DefaultValue = "0";
                vehicleMotorbikeNumberOfKeys.CreatedById = DbConstants.SystemUserId;
                vehicleMotorbikeNumberOfKeys.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(vehicleMotorbikeNumberOfKeys);
                await unitOfWork.SaveAsync();

                PolicyTypeField meMedicalPersone = new PolicyTypeField();
                meMedicalPersone.PolicyTypeId = meMedical.Id;
                meMedicalPersone.FieldName = "Persone Full Name";
                meMedicalPersone.FieldType = PolicyTypeFieldType.String;
                meMedicalPersone.CreatedById = DbConstants.SystemUserId;
                meMedicalPersone.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(meMedicalPersone);

                PolicyTypeField meTravelPersone = new PolicyTypeField();
                meTravelPersone.PolicyTypeId = meTravel.Id;
                meTravelPersone.FieldName = "Persone Full Name";
                meTravelPersone.FieldType = PolicyTypeFieldType.String;
                meTravelPersone.CreatedById = DbConstants.SystemUserId;
                meTravelPersone.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypeFieldRepository.Insert(meTravelPersone);
                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                var insurerResult = new
                {
                    id = insurer.Id
                };

                return Json(insurerResult);
            }
        }

        /// <summary>
        /// Get insurer policy types with template type;
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Groups")]
        public IHttpActionResult GetInsurerPolicyTypes(int insurerId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var groups = unitOfWork.PolicyTypeRepository.GetAll().Where(g => g.InsurerId == insurerId)
                    .Select(a => new
                    {
                        groupid = a.Id,
                        groupName = a.GroupName,
                        groupType = a.PolicyGroupType.ToString(),
                        subGroupName = a.PolicyTypeName,
                        createdDate = a.CreatedDate,
                        status = a.Status.ToString()
                    }
                    ).ToList();

                return Json(groups);
            }
        }

        /// <summary>
        /// Gets policy type by policy type id.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetGroup")]
        public IHttpActionResult GetPolicyType(int groupId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var group = unitOfWork.PolicyTypeRepository.GetAll().FirstOrDefault(g => g.Id == groupId);
                var groupFields = unitOfWork.PolicyTypeFieldRepository.GetAll().Where(gf => gf.PolicyTypeId == groupId && gf.Status != FieldStatus.Deleted);
                List<dynamic> types = new List<dynamic>();
                List<dynamic> statuses = new List<dynamic>();
                List<dynamic> fieldTypes = new List<dynamic>();
                int counter = 0;

                foreach (var item in Enum.GetNames(typeof(PolicyGroupType)).ToList())
                {
                    types.Add(new { id = counter, name = item });
                    counter++;
                }
                counter = 0;

                foreach (var item in Enum.GetNames(typeof(Status)).ToList())
                {
                    statuses.Add(new { id = counter, name = item });
                    counter++;
                }

                counter = 0;
                foreach (var item in Enum.GetNames(typeof(PolicyTypeFieldType)).ToList())
                {
                    fieldTypes.Add(new { id = counter, name = item });
                    counter++;
                }

                var groupDb = unitOfWork.PolicyTypeRepository.GetAll().Where(g => g.Id == groupId).Select(g => new
                {
                    groupId = g.Id,
                    groupName = g.GroupName,
                    groupType = g.PolicyGroupType.ToString(),
                    subGroupName = g.PolicyTypeName,
                    createdDate = g.CreatedDate,
                    status = g.Status.ToString(),
                    insurerId = g.InsurerId,
                    additionalProperties = groupFields.Select(f => new
                    {
                        fieldId = f.Id,
                        fieldName = f.FieldName,
                        fieldType = f.FieldType.ToString(),
                        defaultValue = f.DefaultValue,
                        listDescription = f.ListDesription
                    })
                }).FirstOrDefault();

                if (group == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the policy", ModelState);
                }
                var groupType = types.FirstOrDefault(t => t.name == groupDb.groupType);
                var status = statuses.FirstOrDefault(s => s.name == groupDb.status);
                groupDb.additionalProperties.ToList();

                var result = new
                {
                    groupId = groupDb.groupId,
                    groupName = groupDb.groupName,
                    groupType = groupType,
                    subGroupName = groupDb.subGroupName,
                    status = status,
                    additionalProperties = groupDb.additionalProperties,
                    insurerId = groupDb.insurerId,
                    groupTypes = types,
                    statuses = statuses,
                    fieldTypes = fieldTypes
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Edits insurers policy type.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> EditGroup([FromBody] PolicyGroupModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var group = unitOfWork.PolicyTypeRepository.GetAll().FirstOrDefault(i => i.Id == model.GroupId);
                var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == userId);

                if (group == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Such group doesn't exist.", ModelState);
                }

                unitOfWork.BeginTransaction();

                group.GroupName = model.GroupName;
                group.PolicyGroupType = model.GroupType;
                group.PolicyTypeName = model.SubgroupName;
                group.ModifiedBy = user;
                group.ModifiedById = userId;
                group.ModifiedDate = DateTime.Now;
                group.InsurerId = model.InsurerId;
                group.Status = model.Status;
                unitOfWork.PolicyTypeRepository.Edit(group);
                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                return Ok();
            }

        }

        /// <summary>
        /// Adds new policy type to insurer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddGroup")]
        public async Task<IHttpActionResult> AddGroup([FromBody] PolicyGroupModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Need to fill all required fields.", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == userId);
                var group = new PolicyType();

                unitOfWork.BeginTransaction();

                group.GroupName = model.GroupName;
                group.PolicyGroupType = model.GroupType;
                group.PolicyTypeName = model.SubgroupName;
                group.CreatedById = userId;
                group.CreatedDate = DateTime.Now;
                group.CreatedBy = user;
                group.InsurerId = model.InsurerId;
                group.Status = model.Status;
                
                unitOfWork.PolicyTypeRepository.Insert(group);
                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                var result = new
                {
                    id = group.Id
                };

                return Json(result);
            }
        }


        /// <summary>
        /// Adds additional property to policy type.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddField")]
        public async Task<IHttpActionResult> AddField([FromBody] PolicyGroupFieldModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Need to fill all required fields.", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == userId);
                var field = new PolicyTypeField();

                unitOfWork.BeginTransaction();

                field.FieldName = model.FieldName;
                field.FieldType = model.FieldType;
                field.Status = FieldStatus.Active;
                field.CreatedById = userId;
                field.CreatedDate = DateTime.Now;
                field.DefaultValue = model.DefaultValue;
                field.ListDesription = model.ListDescription;
                field.PolicyTypeId = model.PolicyGroupId;

                unitOfWork.PolicyTypeFieldRepository.Insert(field);
                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                var result = new
                {
                    id = field.Id
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Deletes policy type additional property.
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeleteField")]
        public async Task<IHttpActionResult> DeleteField(int id)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var field = unitOfWork.PolicyTypeFieldRepository.GetAll().FirstOrDefault(i => i.Id == id);

                if (field == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Insurer was not found or already deleted.", ModelState);
                }

                unitOfWork.BeginTransaction();

                field.Status = FieldStatus.Deleted;

                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                return Ok();
            }
        }

        /// <summary>
        /// Edits existing insurer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Edit")]
        public async Task<IHttpActionResult> Edit([FromBody] InsurerModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Need to fill all required fields.", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var insurer = unitOfWork.InsurerRepository.GetAll().FirstOrDefault(i => i.Id == model.Id);
                var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == userId);
                var file = unitOfWork.FileRepository.GetAll().FirstOrDefault(f => f.Id == model.LogoId);
                
                if (insurer == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Such insurer doesn't exist.", ModelState);
                }

                if (file == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Selected logo file is invalid.", ModelState);
                }

                if (model.LogoId != insurer.LogoFileId)
                {
                    SaveFileAsLogo(unitOfWork, file);
                }

                unitOfWork.BeginTransaction();

                insurer.Name = model.Name;
                insurer.Phone = model.Phone;
                insurer.PhoneOverseas = model.PhoneOverseas;
                insurer.URL = model.URL;
                insurer.ModifiedBy = user;
                insurer.ModifiedById = userId;
                insurer.ModifiedDate = DateTime.Now;
                insurer.LogoFileId = model.LogoId;
                insurer.Color = model.Color;
                unitOfWork.InsurerRepository.Edit(insurer);
                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                return Ok();
            }
        }

        /// <summary>
        /// Gets list of insurers with pagination and search term(optional).
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("List")]
        public IHttpActionResult List(int offset = 0, int count = 10, string q = null)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var insurers = unitOfWork.InsurerRepository.GetAll().Where(i => i.Status != InsurerStatus.Deleted);

                if (!string.IsNullOrEmpty(q))
                {
                    insurers = insurers.Where(i => i.Name.StartsWith(q) || i.Name.Contains(q));
                }

                var insurersCount = insurers.Count();

                var startLinkToLogo = Url.Link("Default", new { controller = "files", action = "getlogo" });
                if (insurers.Count() == 0)
                {
                    return Json("");
                }

                var insurersList = insurers
                            .OrderBy(i => i.Name)
                            .Skip(offset)
                            .Take(count)
                            .ToList()
                            .Select(insurer => new
                            {
                                insurerId = insurer.Id,
                                name = insurer.Name,
                                url = insurer.URL,
                                email = insurer.Email,
                                phone = insurer.Phone,
                                phoneOverseas = insurer.PhoneOverseas,
                                logo = new
                                {
                                    id = insurer.LogoFile.Id,
                                    filename = insurer.LogoFile.FileName,
                                    url = string.Concat(startLinkToLogo, "?id=", insurer.LogoFileId),
                                },
                                color = insurer.Color,
                                joinedDate = insurer.CreatedDate,
                            }).ToList();

                var result = new
                {
                    count = insurersCount,
                    data = insurersList
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Gets one insurer by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult Get(string id)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var startLinkToLogo = Url.Link("Default", new { controller = "files", action = "getlogo" });
                int result;
                List<dynamic> types = new List<dynamic>();
                List<dynamic> statuses = new List<dynamic>();
                List<dynamic> fieldTypes = new List<dynamic>();
                int counter = 0;

                if (!int.TryParse(id, out result))
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Insurer is invalid.", ModelState);
                }

                var insurerId = Int32.Parse(id);
                var insurerDb = unitOfWork.InsurerRepository.GetAll().FirstOrDefault(i => i.Id == insurerId && i.Status != InsurerStatus.Deleted);

                if (insurerDb == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Insurer was not found", ModelState);
                }

                foreach (var item in Enum.GetNames(typeof(PolicyGroupType)).ToList())
                {
                    types.Add(new { id = counter, name = item });
                    counter++;
                }
                counter = 0;

                foreach (var item in Enum.GetNames(typeof(Status)).ToList())
                {
                    statuses.Add(new { id = counter, name = item });
                    counter++;
                }

                counter = 0;
                foreach (var item in Enum.GetNames(typeof(PolicyTypeFieldType)).ToList())
                {
                    fieldTypes.Add(new { id = counter, name = item });
                    counter++;
                }

                var insurer = unitOfWork.InsurerRepository.GetAll().Where(i => i.Id == insurerId).Select(
                    i => new
                    {
                        id = i.Id,
                        name = i.Name,
                        url = i.URL,
                        email = i.Email,
                        phone = i.Phone,
                        phoneOverseas = i.PhoneOverseas,
                        logo = new
                        {
                            id = i.LogoFile.Id,
                            name = i.LogoFile.FileName,
                            url = string.Concat(startLinkToLogo, "?id=", i.LogoFileId),
                        },
                        color = i.Color,
                        joinedDate = i.CreatedDate,
                    }).First();

                var value = new
                {
                    id = insurer.id,
                    name = insurer.name,
                    url = insurer.url,
                    email = insurer.email,
                    phone = insurer.phone,
                    phoneOverseas = insurer.phoneOverseas,
                    logo = new
                    {
                        id = insurer.logo.id,
                        name = insurer.logo.name,
                        url = insurer.logo.url
                    },
                    color = insurer.color,
                    joinedDate = insurer.joinedDate,
                    groupTypes = types,
                    statuses = statuses,
                    fieldTypes = fieldTypes
                };

                return Json(value);
            }
        }

        /// <summary>
        /// Deletes an insurer by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                int result;
                if (!int.TryParse(id, out result))
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Insurer is invalid.", ModelState);
                }

                var insurerId = int.Parse(id);
                var insurer = unitOfWork.InsurerRepository.GetAll().FirstOrDefault(i => i.Id == insurerId);

                if (insurer == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Insurer was not found or already deleted.", ModelState);
                }

                unitOfWork.BeginTransaction();

                insurer.Status = InsurerStatus.Deleted;

                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                return Ok();
            }
        }

        private void SaveFileAsLogo(IUnitOfWork unitOfWork, Domain.DbModels.File file)
        {
            string folder = HttpContext.Current.Server.MapPath(Core.Constants.UploadFilesDirectory);

            string path = Path.Combine(folder, file.Id.ToString());

            if (System.IO.File.Exists(path))
            {
                using (Image original = Image.FromFile(path))
                {
                    string logosfolder = HttpContext.Current.Server.MapPath(Core.Constants.LogoFilesDirectory);

                    if (!Directory.Exists(logosfolder))
                    {
                        Directory.CreateDirectory(logosfolder);
                    }

                    var fileName = string.Format("{0}.jpg", file.Id);

                    var logoPath = Path.Combine(logosfolder, fileName);
                    
                    double scaleSmall = Math.Max(original.Height, original.Width) / 150d;

                    Size s = new Size((int)(original.Width / scaleSmall), (int)(original.Height / scaleSmall));

                    Image cropped = new Bitmap(original, s);

                    cropped.Save(logoPath, ImageFormat.Jpeg);

                    cropped.Dispose();

                    file.LocationPath = logoPath;
                    file.FileName = fileName;
                    file.ContentType = "jpg";
                    file.IsTemp = false;

                    unitOfWork.FileRepository.Edit(file);
                    unitOfWork.Save();
                }

                //System.IO.File.Delete(path);           
            }
        }
    }
}
