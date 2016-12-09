using Advisr.DataLayer;
using Advisr.Web.Providers;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Advisr.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public async Task<ActionResult> Index()
        {
            //Retrieve email for 'autopilot' service
            var claimsPrincipal = User as ClaimsPrincipal;
            var claimEmail = claimsPrincipal.FindFirst(ClaimTypes.Email);
            this.ViewBag.UserEmail = claimEmail.Value;
            
            return View();
        }
    }
}