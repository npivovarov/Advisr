using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Advisr.Web.Filters
{
    public class LogMvcErrorAttribute: HandleErrorAttribute
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(ExceptionContext filterContext)
        {
            log.Fatal(filterContext.Exception.Message, filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}