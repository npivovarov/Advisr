using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Advisr.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class PortfolioController : BaseApiController
    {
        /// <summary>
        /// Get Pie Chart Details for Portfolio page 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("PieChartDetails")]
        public IHttpActionResult PieChartDetails()
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var currentDate = DateTime.Now.Date;

                var policies = unitOfWork.PolicyRepository.GetAll()
                            .Where(a => a.Status == PolicyStatus.Confirmed && a.CreatedById == userId && a.EndDate > currentDate)
                            .Select(a => new
                            {
                                groupName = a.PolicyType.PolicyGroup.Name,
                                amount = a.PolicyPremium
                            }).ToList();

                var countOfPendingPolicies = unitOfWork.PolicyRepository.GetAll()
                            .Where(a => a.Status == PolicyStatus.Unconfirmed && a.CreatedById == userId).Count();
                
                var result = new
                {
                    countOfPendingPolicies = countOfPendingPolicies,
                    vehicle = policies.Where(a=> a.groupName == "Vehicle").Select(a=>a.amount).DefaultIfEmpty(0.0m).Sum(),
                    personal = policies.Where(a => a.groupName == "Personal").Select(a => a.amount).DefaultIfEmpty(0.0m).Sum(),
                    property = policies.Where(a => a.groupName == "Property").Select(a => a.amount).DefaultIfEmpty(0.0m).Sum(),
                    commertial = policies.Where(a => a.groupName == "Commercial").Select(a => a.amount).DefaultIfEmpty(0.0m).Sum(),
                };

                return Json(result);
            }
        }
    }
}
