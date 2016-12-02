using Advisr.DataLayer;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace Advisr.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class FilesController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [System.Web.Http.Authorize]
        public ActionResult Get(string id, string type = null)
        {
            string folder = System.Web.HttpContext.Current.Server.MapPath(Advisr.Core.Constants.UploadFilesDirectory);

            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var dbFile = unitOfWork.FileRepository.GetAll().FirstOrDefault(a => a.Id == new Guid(id));

                if (dbFile != null)
                {
                    string path = Path.Combine(folder, id);

                    if (System.IO.File.Exists(path))
                    {
                        var cd = new System.Net.Mime.ContentDisposition
                        {
                            FileName = dbFile.FileName,
                            Inline = (type == "inline"),
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(path, dbFile.ContentType /* "application/octet-stream"*/);
                    }
                    else
                    {
                        throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetLogo(string id)
        {
            string folder = System.Web.HttpContext.Current.Server.MapPath(Advisr.Core.Constants.LogoFilesDirectory);

            string path = Path.Combine(folder, id + ".jpg");

            if (System.IO.File.Exists(path))
            {
                return File(path, "image/jpeg");
            }
            else
            {
                string contentFolder = System.Web.HttpContext.Current.Server.MapPath("~/Content/images");
                path = Path.Combine(contentFolder, "logo_insurer.jpg");
                return File(path, "image/jpeg");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult Photo(string id, string type)
        {
            string folder = System.Web.HttpContext.Current.Server.MapPath(Core.Constants.PhotosFilesDirectory);
            
            string path = Path.Combine(folder, string.Format("{0}_{1}.jpg", id, type));

            if (System.IO.File.Exists(path))
            {
                return File(path, "image/jpeg");
            }
            else
            {
                string contentFolder = System.Web.HttpContext.Current.Server.MapPath("~/Content/images");
                path = Path.Combine(contentFolder, string.Format("empty_{1}.jpg", id, type));
                return File(path, "image/jpeg");
            }
        }
    }
}