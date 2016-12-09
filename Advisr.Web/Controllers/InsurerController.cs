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
                        policyGroupName = a.GroupName,
                        policyGroupType = a.PolicyGroupType,
                        policyTypeName = a.PolicyTypeName,
                        createdDate = a.CreatedDate,
                        status = a.Status
                    }
                    ).ToList();

                return Json(policyTypes);
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
                    policyGroupName = g.GroupName,
                    policyGroupType = g.PolicyGroupType,
                    policyTypeName = g.PolicyTypeName,
                    createdDate = g.CreatedDate,
                    status = g.Status,
                    insurerId = g.InsurerId,
                    additionalProperties = g.PolicyTypeFields.Where(af=>af.Status != FieldStatus.Deleted).
                    Select(f => new
                    {
                        fieldId = f.Id,
                        fieldName = f.FieldName,
                        fieldType = f.FieldType,
                        defaultValue = f.DefaultValue,
                        listDescription = f.ListDesription
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
                var group = unitOfWork.PolicyTypeRepository.GetAll().FirstOrDefault(i => i.Id == model.PolicyTypeId);

                if (group == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Such group doesn't exist.", ModelState);
                }

                group.GroupName = model.PolicyGroupName; // TODO: change to policy group name
                group.PolicyGroupType = model.PolicyGroupType;
                group.PolicyTypeName = model.PolicyTypeName; //TODO: change to policy type name
                group.ModifiedById = userId;
                group.ModifiedDate = DateTime.Now;
                group.InsurerId = model.InsurerId;
                group.Status = model.Status;
                unitOfWork.PolicyTypeRepository.Edit(group);
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
                var group = new PolicyType();
                //TODO check if insurer already has group with such groupName, type and type name
                var repeats = unitOfWork.PolicyTypeRepository.GetAll().FirstOrDefault(g => g.InsurerId == model.InsurerId && g.PolicyTypeName == model.PolicyTypeName);

                if (repeats != null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Such policy type have been already created for this insurer", ModelState);
                }

                group.GroupName = model.PolicyGroupName;
                group.PolicyGroupType = model.PolicyGroupType;
                group.PolicyTypeName = model.PolicyTypeName;
                group.CreatedById = userId;
                group.CreatedDate = DateTime.Now;
                group.InsurerId = model.InsurerId;
                group.Status = model.Status;
                
                unitOfWork.PolicyTypeRepository.Insert(group);
                await unitOfWork.SaveAsync();

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
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                var field = new PolicyTypeField();

                field.FieldName = model.FieldName;
                field.FieldType = model.FieldType;
                field.Status = FieldStatus.Active;
                field.CreatedById = userId;
                field.CreatedDate = DateTime.Now;
                field.DefaultValue = model.DefaultValue;
                field.ListDesription = model.ListDescription;
                field.PolicyTypeId = model.PolicyTypeId;

                unitOfWork.PolicyTypeFieldRepository.Insert(field);
                await unitOfWork.SaveAsync();

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
        /// <param name="id"></param>
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
                    return JsonError(HttpStatusCode.BadRequest, 10, "Field was not found or already deleted.", ModelState);
                }


                field.ModifiedById = User.Identity.GetUserId();
                field.ModifiedDate = DateTime.Now;
                field.Status = FieldStatus.Deleted;

                unitOfWork.PolicyTypeFieldRepository.Edit(field);
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
                var fieldDb = unitOfWork.PolicyTypeFieldRepository.GetAll().Where(g => g.Id == fieldId).Select(g => new
                {
                    fieldId = g.Id,
                    fieldName = g.FieldName,
                    fieldType = g.FieldType,
                    defaultValue = g.DefaultValue,
                    listDescription = g.ListDesription,
                    createdDate = g.CreatedDate,
                    status = g.Status,
                    policyTypeId = g.PolicyTypeId
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
                var field = unitOfWork.PolicyTypeFieldRepository.GetAll().FirstOrDefault(f => f.Id == model.FieldId);

                if (field == null)
                {
                    return JsonError(HttpStatusCode.BadRequest, 10, "Field was not found.", ModelState);
                }


                field.FieldName = model.FieldName;
                field.FieldType = model.FieldType;
                field.Status = FieldStatus.Active;
                field.ModifiedById = userId;
                field.ModifiedDate = DateTime.Now;
                field.DefaultValue = model.DefaultValue;
                field.ListDesription = model.ListDescription;

                unitOfWork.PolicyTypeFieldRepository.Edit(field);
                await unitOfWork.SaveAsync();

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
