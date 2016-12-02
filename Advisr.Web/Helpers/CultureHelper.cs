using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Advisr.Web.Helpers
{
    public static class CultureHelper
    {
        public const string CookieCultureKey = "culture";

        static CultureHelper()
        {
            SupportedLanguages.Add(new CultureInfo("en"));
        }

        public static readonly IList<CultureInfo> SupportedLanguages = new List<CultureInfo>();

        private static CultureInfo currentCulture;

        public static CultureInfo CurrentCulture
        {
            get
            {
                if (currentCulture == null)
                {
                    currentCulture = GetDefaultCultureInfo();
                    ApplyCulture(currentCulture);

                }
                return currentCulture;
            }
        }


        public static bool NeedUseCurrent { get; set; }

        public static bool TrySetCulture(string name)
        {
            bool success = false;
            var current = CurrentCulture;

            //if (!current.TwoLetterISOLanguageName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                var newCulture = SupportedLanguages.FirstOrDefault(a => a.TwoLetterISOLanguageName.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (newCulture != null) // new language is supported
                {
                    currentCulture = newCulture;
                    ApplyCulture(currentCulture);
                    success = true;
                }
            }
            return success;
        }

        private static void ApplyCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture.Name);

            HttpCookie cookie = new HttpCookie(CookieCultureKey, culture.TwoLetterISOLanguageName);
            cookie.Expires = DateTime.Now.AddYears(1);

            //HttpContext.Current.Response.Cookies.Remove(CookieCultureKey);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        private static CultureInfo GetDefaultCultureInfo()
        {
            return SupportedLanguages[0];
        }
    }
}
