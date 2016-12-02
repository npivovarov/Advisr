using log4net;
using System.Reflection;
using System.Web.Http.Filters;

namespace Advisr.Web.Filters
{
    public class LogErrorAttribute: ExceptionFilterAttribute
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnException(HttpActionExecutedContext actionexecutedContext)
        {

            log.Fatal(actionexecutedContext.Exception.Message, actionexecutedContext.Exception);
            base.OnException(actionexecutedContext);
        }
    }
}