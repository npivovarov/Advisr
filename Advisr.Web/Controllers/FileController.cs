
using Advisr.Core;
using Advisr.DataLayer;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Advisr.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class FileController : BaseApiController
    {
        /// <summary>
        /// The method need to get upload file and store to local folder.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("upload")]
        public async Task<IHttpActionResult> Upload()
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                return JsonError(HttpStatusCode.BadRequest, 201, "Mime Multipart Content missing");
            }

            var userId = User.Identity.GetUserId();

            MultipartMemoryStreamProvider provider = await request.Content.ReadAsMultipartAsync();

            Advisr.Domain.DbModels.File dbFile = null;

            var content = provider.Contents.SingleOrDefault();
            if (content is StreamContent)
            {
                string fileName = content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                string mediaType = content.Headers.ContentType.MediaType;


                string folder = System.Web.HttpContext.Current.Server.MapPath(Core.Constants.UploadFilesDirectory);

                long fileSize = 0;

                StreamContent streamContent = (StreamContent)content;

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                try
                {
                    using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                    {
                        dbFile = new Domain.DbModels.File();
                        dbFile.FileName = fileName;
                        dbFile.ContentType = mediaType;
                        dbFile.IsTemp = true;
                        dbFile.CreatedDate = DateTime.Now;
                        dbFile.CreatedById = userId;

                        unitOfWork.FileRepository.Insert(dbFile);
                        await unitOfWork.SaveAsync();

                        string filePath = Path.Combine(folder, dbFile.Id.ToString());

                        using (var fileStream = await streamContent.ReadAsStreamAsync())
                        {
                            using (var file = File.Create(filePath))
                            {
                                await fileStream.CopyToAsync(file);

                                fileSize = fileStream.Length;
                            }
                            streamContent.Dispose();
                        }

                        dbFile.FileSize = fileSize;
                        unitOfWork.FileRepository.Edit(dbFile);

                        await unitOfWork.SaveAsync();
                    }
                }
                catch (Exception e)
                {
                    return JsonError(HttpStatusCode.BadRequest, 202, e.Message + " " + e.StackTrace);
                }
            }
            else
            {
                return JsonError(HttpStatusCode.BadRequest, 203, "Content missing");
            }

            var result = new
            {
                id = dbFile.Id,
                fileName = dbFile.FileName,
                fileSize = dbFile.FileSize,
                contentType = dbFile.ContentType,
                url = string.Format("{0}/get/{1}", Url.Link("Default", new { controller = "files" }), dbFile.Id)
            };

            return this.Ok(result);
        }
    }
}
