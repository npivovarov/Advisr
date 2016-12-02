using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;

namespace Advisr.Web.Helpers
{
    public class LogWriter
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private LogWriter()
        {
                
        }

        public static LogWriter Create()
        {
            return new LogWriter();
        }

        public void LogMessage(string message, string type, Type controller, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string invokerName = "")
        {
            switch (type)
            {
                case "Error":
                    log.Error(message +" "+controller.Name+"."+invokerName + ", line number:" + lineNumber);
                    break;
                case "Fatal":
                    log.Fatal(message +" "+ controller.Name+"."+ invokerName + ", line number:" + lineNumber);
                    break;
                case "Info":
                    log.Info(message);
                    break;
                default:
                    break;
            }
        }
    }
}