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
using Newtonsoft.Json;

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
            if (model.LogoId == null)
            {
                ModelState.AddModelError("logoId", "Logo is required");
            }

            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var file = unitOfWork.FileRepository.GetAll().FirstOrDefault(f => f.Id == model.LogoId);

                if (file == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "File doesn't exist.", ModelState);
                }

                unitOfWork.BeginTransaction();

                SaveFileAsLogo(unitOfWork, file);

                var insurer = new Insurer();
                insurer.Name = model.Name;
                insurer.URL = model.URL;
                insurer.Phone = model.Phone;
                insurer.PhoneOverseas = model.PhoneOverseas;
                insurer.LogoFileId = model.LogoId;
                insurer.Email = model.Email;
                insurer.Color = model.Color;
                insurer.CreatedById = userId;
                insurer.CreatedDate = DateTime.Now;
                insurer.Status = InsurerStatus.Active;
                insurer.Description = model.Description;

                unitOfWork.InsurerRepository.Insert(insurer);
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
        [ActionName("PolicyTypes")]
        public IHttpActionResult GetInsurerPolicyTypes(int insurerId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var policyTypes = unitOfWork.PolicyTypeRepository.GetAll().Where(g => g.InsurerId == insurerId)
                    .Select(a => new
                    {
                        policyTypeId = a.Id,
                        policyGroupName = a.PolicyGroup.Name,
                        //TODO: Kate
                        policyTemplate = a.PolicyTemplate.Name,
                        policyTypeName = a.PolicyTypeName,
                        createdDate = a.CreatedDate,
                        status = a.Status,
                        description = a.Description
                    }
                    ).ToList();

                return Json(policyTypes);
            }
        }

        /// <summary>
        /// Gets all policy groups.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetPolicyGroups")]
        public IHttpActionResult GetPolicyGroups()
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var groupsDb = unitOfWork.PolicyGroupRepository.GetAll()
                    .Select(g => new
                    {
                        id = g.Id,
                        name = g.Name
                    }).ToList();

                return Json(groupsDb);
            }
        }

        /// <summary>
        /// Gets policy group's templates
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetPolicyGroupTemplates")]
        public IHttpActionResult GetPolicyGroupTemplates(int groupId)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var templatesDb = unitOfWork.PolicyTemplateRepository.GetAll().Where(t=>t.PolicyGroupId == groupId)
                    .Select(t => new
                    {
                        id = t.Id,
                        name = t.Name
                    }).ToList();

                return Json(templatesDb);
            }
        }

        /// <summary>
        /// Gets policy type by policy type id.
        /// </summary>
        /// <param name="policyTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("PolicyType")]
        public IHttpActionResult GetPolicyType(int policyTypeId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var typeDb = unitOfWork.PolicyTypeRepository.GetAll().Where(g => g.Id == policyTypeId).Select(g => new
                {
                    policyTypeId = g.Id,
                    policyGroupName = g.PolicyGroup.Name,
                    //TODO: Kate
                    policyTemplate = g.PolicyTemplate.Name,
                    policyTypeName = g.PolicyTypeName,
                    createdDate = g.CreatedDate,
                    status = g.Status,
                    insurerId = g.InsurerId,
                    description = g.Description,
                    additionalProperties = g.PolicyTypePolicyProperties.Where(af=>af.Status != PolicyTypePolicyPropertyStatus.Deleted && af.PolicyProperty.Status != FieldStatus.Deleted).
                    Select(f => new
                    {
                        fieldId = f.Id,
                        policyPropertyId = f.PolicyPropertyId,
                        fieldName = f.PolicyProperty.FieldName,
                        fieldType = f.PolicyProperty.FieldType,
                        defaultValue = f.PolicyProperty.DefaultValue,
                        listDescription = f.PolicyProperty.ListDesription
                    })
                }).FirstOrDefault();

                if (typeDb == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the group", ModelState);
                }

                typeDb.additionalProperties.ToList();

                return Json(typeDb);
            }
        }

        /// <summary>
        /// Edits insurers policy type.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> EditPolicyType([FromBody] PolicyTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var policyType = unitOfWork.PolicyTypeRepository.GetAll().FirstOrDefault(i => i.Id == model.PolicyTypeId);

                if (policyType == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Such group doesn't exist.", ModelState);
                }

                policyType.PolicyGroupId = model.PolicyGroupId;
                policyType.PolicyTemplateId = model.PolicyTemplateId;
                policyType.PolicyTypeName = model.PolicyTypeName; 
                policyType.ModifiedById = userId;
                policyType.ModifiedDate = DateTime.Now;
                policyType.InsurerId = model.InsurerId;
                policyType.Status = model.Status;
                policyType.Description = model.Description;
                unitOfWork.PolicyTypeRepository.Edit(policyType);
                await unitOfWork.SaveAsync();

                return Ok();
            }

        }

        /// <summary>
        /// Adds new policy type to insurer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddPolicyType([FromBody] PolicyTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var policyType = new PolicyType();
                //TODO check if insurer already has group with such groupName, type and type name
                var repeats = unitOfWork.PolicyTypeRepository.GetAll().FirstOrDefault(g => g.InsurerId == model.InsurerId && g.PolicyGroupId == model.PolicyGroupId && g.PolicyTemplateId == model.PolicyTemplateId && g.PolicyTypeName == model.PolicyTypeName);

                if (repeats != null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Such policy type have been already created for this insurer", ModelState);
                }

                //TODO: Kate
                policyType.PolicyGroupId = model.PolicyGroupId;
                policyType.PolicyTemplateId = model.PolicyTemplateId;
                policyType.PolicyTypeName = model.PolicyTypeName;
                policyType.CreatedById = userId;
                policyType.CreatedDate = DateTime.Now;
                policyType.InsurerId = model.InsurerId;
                policyType.Status = model.Status;
                policyType.Description = model.Description;
                unitOfWork.PolicyTypeRepository.Insert(policyType);
                await unitOfWork.SaveAsync();

                var result = new
                {
                    id = policyType.Id
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
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var property = new PolicyProperty();

                unitOfWork.BeginTransaction();

                property.FieldName = model.FieldName;
                property.FieldType = model.FieldType;
                property.Status = FieldStatus.Active;
                property.DefaultValue = model.DefaultValue;
                property.ListDesription = model.ListDescription;
                property.CreatedById = userId;
                property.CreatedDate = DateTime.Now;

                if (model.FieldType == PolicyTypeFieldType.List)
                {
                    try
                    {
                        var list = JsonConvert.DeserializeObject<List<string>>(property.ListDesription);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("listDescription", "Please use correct format, like [\"item1\", \"item2\"]");
                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }
                }

                unitOfWork.PolicyPropertyRepository.Insert(property);
                await unitOfWork.SaveAsync();

                PolicyTypePolicyProperty policyTypePolicyProperty = new PolicyTypePolicyProperty();
                policyTypePolicyProperty.PolicyPropertyId = property.Id;
                policyTypePolicyProperty.PolicyTypeId =   model.PolicyTypeId;
                policyTypePolicyProperty.CreatedById = userId;
                policyTypePolicyProperty.CreatedDate = DateTime.Now;
                
                unitOfWork.PolicyTypePolicyPropertyRepository.Insert(policyTypePolicyProperty);
                await unitOfWork.SaveAsync();

                unitOfWork.CommitTransaction();

                var result = new
                {
                    id = property.Id
                };

                return Json(result);
            }
        }

        /// <summary>
        /// Deletes policy type additional property.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeleteField")]
        public async Task<IHttpActionResult> DeleteField(int id)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var property = unitOfWork.PolicyPropertyRepository.GetAll().FirstOrDefault(i => i.Id == id);

                if (property == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Field was not found or already deleted.", ModelState);
                }

                var policyTypeProperty = unitOfWork.PolicyTypePolicyPropertyRepository.GetAll().FirstOrDefault(f => f.PolicyPropertyId == property.Id);

                if (policyTypeProperty != null)
                {
                    policyTypeProperty.ModifiedById = User.Identity.GetUserId();
                    policyTypeProperty.ModifiedDate = DateTime.Now;
                    policyTypeProperty.Status = PolicyTypePolicyPropertyStatus.Deleted;
                    unitOfWork.PolicyTypePolicyPropertyRepository.Edit(policyTypeProperty);
                }

                property.ModifiedById = User.Identity.GetUserId();
                property.ModifiedDate = DateTime.Now;
                property.Status = FieldStatus.Deleted;

                unitOfWork.PolicyPropertyRepository.Edit(property);
                
                await unitOfWork.SaveAsync();

                return Ok();
            }
        }

        /// <summary>
        /// Gets policy type field.
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetField")]
        public IHttpActionResult GetField(int fieldId)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var fieldDb = unitOfWork.PolicyPropertyRepository.GetAll().Where(g => g.Id == fieldId).Select(g => new
                {
                    fieldId = g.Id,
                    fieldName = g.FieldName,
                    fieldType = g.FieldType,
                    defaultValue = g.DefaultValue,
                    listDescription = g.ListDesription,
                    createdDate = g.CreatedDate,
                    status = g.Status,
                }).FirstOrDefault();

                if (fieldDb == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the policy", ModelState);
                }

                return Json(fieldDb);
            }
        }

        /// <summary>
        /// Saves the additional property edited by user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SaveField")]
        public async Task<IHttpActionResult> SaveField([FromBody] PolicyGroupFieldModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var property = unitOfWork.PolicyPropertyRepository.GetAll().FirstOrDefault(f => f.Id == model.FieldId);

                if (property == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Field was not found.", ModelState);
                }

                unitOfWork.BeginTransaction();
                
                property.FieldName = model.FieldName;
                property.FieldType = model.FieldType;
                property.Status = FieldStatus.Active;
                property.ModifiedById = userId;
                property.ModifiedDate = DateTime.Now;
                property.DefaultValue = model.DefaultValue;
                property.ListDesription = model.ListDescription;

                if (model.FieldType == PolicyTypeFieldType.List)
                {
                    try
                    {
                        var list = JsonConvert.DeserializeObject<List<string>>(property.ListDesription);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("listDescription", "Please use correct format, like [\"item1\", \"item2\"]");
                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }
                }

                unitOfWork.PolicyPropertyRepository.Edit(property);
                await unitOfWork.SaveAsync();

                PolicyTypePolicyProperty policyTypePolicyProperty = unitOfWork.PolicyTypePolicyPropertyRepository.GetAll()
                    .FirstOrDefault(p => p.PolicyPropertyId == model.FieldId);
                if (policyTypePolicyProperty == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Field was not found.", ModelState);
                }
                policyTypePolicyProperty.PolicyPropertyId = property.Id;
                policyTypePolicyProperty.PolicyTypeId = model.PolicyTypeId;
                policyTypePolicyProperty.CreatedById = userId;
                policyTypePolicyProperty.CreatedDate = DateTime.Now;

                unitOfWork.PolicyTypePolicyPropertyRepository.Edit(policyTypePolicyProperty);
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
            if (model.LogoId == null)
            {
                ModelState.AddModelError("logoId", "Logo is required");
            }

            if (!ModelState.IsValid)
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var insurer = unitOfWork.InsurerRepository.GetAll().FirstOrDefault(i => i.Id == model.Id);
                var file = unitOfWork.FileRepository.GetAll().FirstOrDefault(f => f.Id == model.LogoId);
                
                if (insurer == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Such insurer doesn't exist.", ModelState);
                }

                if (file == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Selected logo file is invalid.", ModelState);
                }
                unitOfWork.BeginTransaction();

                if (model.LogoId != insurer.LogoFileId)
                {
                    SaveFileAsLogo(unitOfWork, file);
                }

                insurer.Name = model.Name;
                insurer.Phone = model.Phone;
                insurer.PhoneOverseas = model.PhoneOverseas;
                insurer.URL = model.URL;
                insurer.ModifiedById = userId;
                insurer.ModifiedDate = DateTime.Now;
                insurer.LogoFileId = model.LogoId;
                insurer.Color = model.Color;
                insurer.Description = model.Description;
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

                var dataCount = insurers.Count();

                var startLinkToLogo = Url.Link("Default", new { controller = "files", action = "getlogo" });

                var insurersList = insurers.Where(insurer=>insurer.PolicyGroups.Count > 0)
                            .OrderBy(i => i.Name)
                            .Skip(offset)
                            .Take(count)
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
                    count = dataCount,
                    data = insurersList
                };

                return Json(result);
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
        [ActionName("ListAdmin")]
        public IHttpActionResult ListAdmin(int offset = 0, int count = 10, string q = null)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var insurers = unitOfWork.InsurerRepository.GetAll().Where(i => i.Status != InsurerStatus.Deleted);

                if (!string.IsNullOrEmpty(q))
                {
                    insurers = insurers.Where(i => i.Name.StartsWith(q) || i.Name.Contains(q));
                }

                var dataCount = insurers.Count();

                var startLinkToLogo = Url.Link("Default", new { controller = "files", action = "getlogo" });

                var insurersList = insurers
                            .OrderBy(i => i.Name)
                            .Skip(offset)
                            .Take(count)
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
                    count = dataCount,
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
        public IHttpActionResult Get(int id)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var startLinkToLogo = Url.Link("Default", new { controller = "files", action = "getlogo" });

                var insurer = unitOfWork.InsurerRepository.GetAll().Where(i => i.Id == id).Select(
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
                        description = i.Description
                    }).FirstOrDefault();

                if (insurer == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Insurer was not found", ModelState);
                }

                return Json(insurer);
            }
        }

        /// <summary>
        /// Deletes an insurer by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var insurer = unitOfWork.InsurerRepository.GetAll().FirstOrDefault(i => i.Id == id && i.Status == InsurerStatus.Active);

                if (insurer == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Insurer was not found or already deleted.", ModelState);
                }

                insurer.ModifiedById = User.Identity.GetUserId();
                insurer.ModifiedDate = DateTime.Now;
                insurer.Status = InsurerStatus.Deleted;

                unitOfWork.InsurerRepository.Edit(insurer);

                await unitOfWork.SaveAsync();

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

                    Bitmap image = new Bitmap(cropped);

                    Color backColor = image.GetPixel(1, 1);

                    if (backColor.Name == "0")
                    {
                        image.MakeTransparent(backColor);

                        using (var b = new Bitmap(image.Width, image.Height))
                        {
                            b.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                            using (var g = Graphics.FromImage(b))
                            {
                                g.Clear(Color.White);
                                g.DrawImageUnscaled(image, 0, 0);
                            }

                            b.Save(logoPath, ImageFormat.Jpeg);
                            

                            b.Dispose();
                        }
                    }
                    else
                    {
                        cropped.Save(logoPath, ImageFormat.Jpeg);
                        cropped.Dispose();
                    }

                    file.LocationPath = logoPath;
                    file.FileName = fileName;
                    file.ContentType = "image/jpeg";
                    file.IsTemp = false;

                    unitOfWork.FileRepository.Edit(file);
                    unitOfWork.Save();
                }

                //System.IO.File.Delete(path);           
            }
        }
    }
}
