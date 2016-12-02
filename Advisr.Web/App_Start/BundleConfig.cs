using System.Web;
using System.Web.Optimization;

namespace Advisr.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {


#if DEBUG
            BundleTable.EnableOptimizations = false; // false
#else
            BundleTable.EnableOptimizations = false;
#endif



            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                       "~/Scripts/angular.js",
                       "~/Scripts/angular-sanitize.js",
                       "~/Scripts/angular-cookies.js",
                       "~/Scripts/angular-ui-router.js",
                       "~/Scripts/angular-animate.min.js",
                       "~/Scripts/angular-button-spinner.min.js",
                       "~/Scripts/angular-bootstrap-checkbox.js",
                       "~/Scripts/anim-in-out.js",
                       "~/Scripts/ui-bootstrap-tpls-2.2.0.js",
                       "~/Scripts/ngMask.js",
                       "~/Scripts/ng-file-upload.js",
                       "~/Scripts/ngDialog.min.js"

           ));
            bundles.Add(new ScriptBundle("~/bundles/angularDashboard").Include(
                       "~/Scripts/select.js"
                       
           ));
            //Account
            bundles.Add(new ScriptBundle("~/bundles/angularLoginApp").Include(
                "~/Angular/Login/app.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/angularLoginControllers").IncludeDirectory(
                 "~/Angular/Login/controllers", "*.js", true
            ));

            bundles.Add(new ScriptBundle("~/bundles/angularLoginServices").IncludeDirectory(
                 "~/Angular/Login/services", "*.js", true
            ));

            //Dashboard
            bundles.Add(new ScriptBundle("~/bundles/angularDashboardApp").Include(
                        "~/Angular/Dashboard/app.js"
            ));
            
            bundles.Add(new ScriptBundle("~/bundles/angularDashboardControllers").IncludeDirectory(
                "~/Angular/Dashboard/controllers", "*.js", true
            ));

            bundles.Add(new ScriptBundle("~/bundles/angularDashboardServices").IncludeDirectory(
                "~/Angular/Dashboard/services", "*.js", true
            ));

            bundles.Add(new ScriptBundle("~/bundles/angularDashboardFilters").IncludeDirectory(
                "~/Angular/Dashboard/filters", "*.js", true
            ));

            bundles.Add(new ScriptBundle("~/bundles/angularDashboardDirectives").IncludeDirectory(
                "~/Angular/Dashboard/directives", "*.js", true
            ));

            bundles.Add(new ScriptBundle("~/bundles/libraries").Include(
                "~/Scripts/lodash.underscore.js"

            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/webflow").Include(
                        "~/Scripts/webflow.js"
            ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/fonts/font-awesome/css/font-awesome.min.css",
                      "~/Content/sb-admin.css",
                      "~/Content/main.css",
                      "~/Content/ui.css",
                      "~/Content/advisr-ui-element-kit.css",
                      "~/Content/anim-in-out.css",
                      "~/Content/ui-bootstrap-custom-2.2.0-csp.css",
                      "~/Content/ngDialog.min.css",
                      "~/Content/ngDialog-theme-default.css"
            ));
            //Dashboard
            bundles.Add(new StyleBundle("~/Content/cssDashboard").Include(
                      "~/Content/select.css"
            ));
        }
    }
}
