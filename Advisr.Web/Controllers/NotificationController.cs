using Advisr.DataLayer;
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
    public class NotificationController : BaseApiController
    {
        ///// <summary>
        ///// Get notification by id;
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[ActionName("Get")]
        //public IHttpActionResult Get(int id)
        //{
        //    using (IUnitOfWork unitOfWork = UnitOfWork.Create())
        //    {
        //        var userId = User.Identity.GetUserId();
                
        //        var startLinkToPhoto = Url.Link("Default", new { controller = "files", action = "photo" });

        //        var userDb = unitOfWork.UserRepository.GetAll()
        //            .Where(a => a.Id == userId)
        //            .Select(a => new
        //            {
        //                id = a.Id,
        //                username = a.UserName,
        //                firstName = a.FirstName,
        //                lastName = a.LastName,
        //                email = a.Email,
        //                status = a.Status,
        //                avatarFileId = a.AvatarFileId,
        //                photo = new
        //                {
        //                    smallPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=s"),
        //                    bigPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=b"),
        //                },
        //                roles = a.Roles.Select(p => new
        //                {
        //                    id = p.Role.Name
        //                }),
        //                createdDate = a.CreatedDate,
        //                createdBy = new
        //                {
        //                    id = a.CreatedBy.Id,
        //                    firstName = a.CreatedBy.FirstName,
        //                    lastName = a.CreatedBy.LastName,
        //                },
        //            })
        //            .First();

        //        if (userDb == null)
        //        {
        //            return this.JsonError(HttpStatusCode.NotFound, 0, "not found the user");
        //        }

        //        var result = new
        //        {
        //            id = userDb.id,
        //            username = userDb.username,
        //            firstName = userDb.firstName,
        //            lastName = userDb.lastName,
        //            email = userDb.email,
        //            status = userDb.status,
        //            avatarFileId = userDb.avatarFileId,
        //            photo = userDb.photo,
        //            roles = userDb.roles.ToList(),
        //            createdDate = userDb.createdDate,
        //            createdBy = userDb.createdBy
        //        };

        //        return Json(result);
        //    }
        //}

        ///// <summary>
        ///// Get list of all users, allow only for Admin role;
        ///// </summary>
        ///// <param name="offset"></param>
        ///// <param name="count"></param>
        ///// <param name="q"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[ActionName("List")]
        //[Authorize(Roles = "ADMIN")]
        //public IHttpActionResult List(int offset = 0, int count = 10, string q = null)
        //{
        //    using (IUnitOfWork unitOfWork = UnitOfWork.Create())
        //    {
        //        var userId = User.Identity.GetUserId();

        //        IQueryable<ApplicationUser> query = unitOfWork.UserRepository.GetAll()
        //           .Where(a => /*a.Status == status && a.Roles.Any(r => r.Role.Id == DataLayer.DbConstants.PrivateUserRoleId) &&*/ a.Hidden == false);

        //        if (!string.IsNullOrEmpty(q))
        //        {
        //            query = query.Where(e => e.FirstName.StartsWith(q)
        //                    || e.Id == q
        //                    || e.LastName.StartsWith(q)
        //                    || e.Email == q);
        //        }

        //        var countOfUsers = query.Count();

        //        var startLinkToPhoto = Url.Link("Default", new { controller = "files", action = "photo" });

        //        var users = query
        //                    .OrderByDescending(a => a.CreatedDate)
        //                    .Skip(offset)
        //                    .Take(count)
        //                    .ToList()
        //                    .Select(a => new
        //                    {
        //                        id = a.Id,
        //                        firstName = a.FirstName,
        //                        lastName = a.LastName,
        //                        photo = new
        //                        {
        //                            smallPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=s"),
        //                            bigPhoto = string.Concat(startLinkToPhoto, "?id=", a.AvatarFileId, "&type=b"),
        //                        },
        //                        joinedDate = a.CreatedDate,
        //                        status = a.Status
        //                    }).ToList();


        //        var result = new
        //        {
        //            myUserId = userId,
        //            count = countOfUsers,
        //            data = users
        //        };

        //        return Json(result);
        //    }
        //}




    }
}
