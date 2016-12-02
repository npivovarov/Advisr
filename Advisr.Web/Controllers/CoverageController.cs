using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Models;
using Microsoft.AspNet.Identity;
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
    [Authorize(Roles = "ADMIN")]
    public class CoverageController : BaseApiController
    {
        /// <summary>
        /// Get coverage info by id;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult Get(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var result = unitOfWork.CoverageRepository.GetAll().Where(a => a.Id == id)
                    .Select(a => new
                    {
                        id = a.Id,
                        title = a.Title,
                        type = a.Type,
                        description = a.Description,
                        insurerId = a.InsurerId
                    }).FirstOrDefault();

                return Json(result);
            }
        }


        /// <summary>
        /// Get list of coverages by insurer id or policy type id
        /// </summary>
        /// <param name="status"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="groupName">For exmp: Me, Vehicle, Property, etc.
        /// <returns></returns>
        [HttpGet]
        [ActionName("List")]
        public IHttpActionResult List(int? insurerId = null, int? policyTypeId = null)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                if (insurerId == null && policyTypeId == null)
                {
                    throw new ArgumentException();
                }
                
                dynamic list = null;

                if (insurerId != null)
                {
                    list = unitOfWork.CoverageRepository.GetAll().Where(a => a.InsurerId == insurerId && a.Status != CoverageStatus.Hidden)
                        .Select(a => new
                        {
                            id = a.Id,
                            title = a.Title,
                            type = a.Type,
                            description = a.Description,
                            insurerId = a.InsurerId
                        }).ToList();
                }
                else if(policyTypeId != null)
                {
                    list = unitOfWork.PolicyTypeCoverageRepository.GetAll().Where(a => a.PolicyTypeId == policyTypeId && a.Coverage.Status != CoverageStatus.Hidden)
                         .Select(a => new
                         {
                             id = a.Coverage.Id,
                             title = a.Coverage.Title,
                             type = a.Coverage.Type,
                             description = a.Coverage.Description,
                             insurerId = a.Coverage.InsurerId
                         }).ToList();
                }
                
                return Json(list);
            }
        }





        /// <summary>
        /// Add new Coverage for Insurer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Add")]
        public async Task<IHttpActionResult> Add([FromBody] NewCoverageModel model)
        {
            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();

                    unitOfWork.BeginTransaction();

                    Coverage coverage = new Coverage();
                    coverage.InsurerId = model.InsurerId;
                    coverage.Title = model.Title;
                    coverage.Description = model.Description;
                    coverage.Type = model.Type;
                    coverage.CreatedById = userId;
                    coverage.CreatedDate = DateTime.Now;

                    unitOfWork.CoverageRepository.Insert(coverage);
                    await unitOfWork.SaveAsync();

                    if (model.PolicyTypeId.HasValue)
                    {
                        PolicyTypeCoverage policyTypeCoverage = new PolicyTypeCoverage();
                        policyTypeCoverage.PolicyTypeId = model.PolicyTypeId.Value;
                        policyTypeCoverage.CoverageId = coverage.Id;
                        policyTypeCoverage.CreatedById = userId;
                        policyTypeCoverage.CreatedDate = DateTime.Now;

                        unitOfWork.PolicyTypeCoverageRepository.Insert(policyTypeCoverage);
                        await unitOfWork.SaveAsync();
                    }

                    unitOfWork.CommitTransaction();

                    var result = new
                    {
                        id = coverage.Id,
                        title = coverage.Title,
                        type = coverage.Type,
                        description = coverage.Description,
                        insurerId = coverage.InsurerId
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
        /// Edit Coverage for Insurer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Edit")]
        public async Task<IHttpActionResult> Edit([FromBody] EditCoverageModel model)
        {
            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();

                    Coverage coverage = unitOfWork.CoverageRepository.GetById(model.Id);

                    coverage.Title = model.Title;
                    coverage.Description = model.Description;
                    coverage.Type = model.Type;
                    coverage.ModifiedById = userId;
                    coverage.ModifiedDate = DateTime.Now;

                    unitOfWork.CoverageRepository.Edit(coverage);
                    await unitOfWork.SaveAsync();

                    return Ok();
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

        /// <summary>
        /// Assign insurer coverage to insurer policy type 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Assign")]
        public async Task<IHttpActionResult> Assign([FromBody] AssignCoverageModel model)
        {
            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();

                    Coverage coverage = unitOfWork.CoverageRepository.GetById(model.CoverageId);
                    PolicyType policyType = unitOfWork.PolicyTypeRepository.GetById(model.PolicyTypeId);

                    if (coverage.InsurerId != policyType.InsurerId)
                    {
                        ModelState.AddModelError("coverageId", "The coverage is missing in coverage list of the insurer");
                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }

                    PolicyTypeCoverage policyTypeCoverage = unitOfWork.PolicyTypeCoverageRepository.GetAll().FirstOrDefault(a => a.CoverageId == model.CoverageId && a.PolicyTypeId == model.PolicyTypeId);

                    if (policyTypeCoverage != null)
                    {
                        ModelState.AddModelError("coverageId", "The coverage is present in the policy type");
                        return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
                    }

                    policyTypeCoverage = new PolicyTypeCoverage();
                    policyTypeCoverage.PolicyTypeId = model.PolicyTypeId;
                    policyTypeCoverage.CoverageId = coverage.Id;
                    policyTypeCoverage.CreatedById = userId;
                    policyTypeCoverage.CreatedDate = DateTime.Now;

                    unitOfWork.PolicyTypeCoverageRepository.Insert(policyTypeCoverage);
                    await unitOfWork.SaveAsync();

                    return Ok();
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }


        /// <summary>
        /// Delete coverage from policy type 
        /// </summary>
        /// <param name="coverageId"></param>
        /// <param name="policyTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DeleteFromPolicyType")]
        public async Task<IHttpActionResult> DeleteFromPolicyType(int coverageId, int policyTypeId)
        {
            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();

                    PolicyTypeCoverage policyTypeCoverage = unitOfWork.PolicyTypeCoverageRepository.GetAll()
                        .FirstOrDefault(a => a.CoverageId == coverageId && a.PolicyTypeId == policyTypeId);

                    unitOfWork.PolicyTypeCoverageRepository.Delete(policyTypeCoverage);
                    await unitOfWork.SaveAsync();

                    return Ok();
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }


        /// <summary>
        /// Hide the coverage for insurer coverage list
        /// </summary>
        /// <param name="coverageId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("HideForInsurer")]
        public async Task<IHttpActionResult> HideForInsurer(int coverageId)
        {
            if (ModelState.IsValid)
            {
                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var userId = User.Identity.GetUserId();

                    Coverage coverage = unitOfWork.CoverageRepository.GetById(coverageId);

                    coverage.Status = CoverageStatus.Hidden;
                    coverage.ModifiedById = userId;
                    coverage.ModifiedDate = DateTime.Now;

                    unitOfWork.CoverageRepository.Edit(coverage);
                    await unitOfWork.SaveAsync();

                    return Ok();
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 10, "Warning", ModelState);
            }
        }

    }
}
