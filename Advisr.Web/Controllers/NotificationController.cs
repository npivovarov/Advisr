using Advisr.DataLayer;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Advisr.Web.Controllers
{
    /// <summary>
    /// Notification api controller
    /// </summary>
    [Authorize]
    public class NotificationController : BaseApiController
    {
        /// <summary>
        /// Get information about number of unread notifications 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetCounter")]
        public IHttpActionResult GetCounter()
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var count = unitOfWork.UserNotificationRepository.GetAll().Where(e => 
                        e.RecipientId == userId && e.Status == Domain.DbModels.UserNotificationStatus.Unread).Count();
                
                return Json(count);
            }
        }

        /// <summary>
        /// Get details of notification by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult Get(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var notification = unitOfWork.UserNotificationRepository.GetAll().Where(e => e.Id == id && e.RecipientId == userId && e.Status != Domain.DbModels.UserNotificationStatus.Deleted)
                    .Select(a => new
                    {
                        id = a.Id,
                        title = a.Title,
                        subjectTitleFirst = a.SubjectTitleFirst,
                        subjectTitleSecond = a.SubjectTitleSecond,
                        body = a.Body,
                        notificationType = a.NotificationType,
                        targetObjectType = a.TargetObjectType,
                        targetObjectId = a.TargetObjectId,
                        targetUrl = a.TargetUrl,
                        isUnread = a.Status == Domain.DbModels.UserNotificationStatus.Unread,
                        createdDate = a.CreatedDate
                    }).FirstOrDefault();

                if (notification == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the notification");
                }

                return Json(notification);
            }
        }

        /// <summary>
        /// Get list of all user notification 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("List")]
        public IHttpActionResult List(int offset = 0, int count = 10)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();
                
                var notificationsQuery = unitOfWork.UserNotificationRepository.GetAll().Where(e => e.RecipientId == userId && e.Status != Domain.DbModels.UserNotificationStatus.Deleted);
                
                var countOfData = notificationsQuery.Count();
                var countOfRead = notificationsQuery.Count(a=>a.Status == Domain.DbModels.UserNotificationStatus.Read);
                var countOfUnread = countOfData - countOfRead;

                var notifications = notificationsQuery.OrderByDescending(a => a.CreatedDate)
                                    .Skip(offset)
                                    .Take(count)
                                    .Select(a => new
                                    {
                                        id = a.Id,
                                        //title = a.Title,
                                        subjectTitleFirst = a.SubjectTitleFirst,
                                        subjectTitleSecond = a.SubjectTitleSecond,
                                        notificationType = a.NotificationType,
                                        targetObjectType = a.TargetObjectType,
                                        targetObjectId = a.TargetObjectId,
                                        isUnread = a.Status == Domain.DbModels.UserNotificationStatus.Unread,
                                        createdDate = a.CreatedDate
                                    }).ToList();


                var value = new
                {
                    count = countOfData,
                    countOfRead = countOfRead,
                    countOfUnread = countOfUnread,
                    data = notifications
                };


                return Json(value);
            }
        }
        
        /// <summary>
        /// Mark notification as read
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("MarkAsRead")]
        public async Task<IHttpActionResult> MarkAsRead(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var notification = unitOfWork.UserNotificationRepository.GetAll().Where(e => e.Id == id && e.RecipientId == userId).FirstOrDefault();

                if (notification == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the notification");
                }
                else
                {
                    notification.Status = Domain.DbModels.UserNotificationStatus.Read;
                    notification.ReadDate = DateTime.Now;
                    unitOfWork.UserNotificationRepository.Edit(notification);
                    await unitOfWork.SaveAsync();
                }

                return this.Ok();
            }
        }

        /// <summary>
        /// Delete notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Create())
            {
                var userId = User.Identity.GetUserId();

                var notification = unitOfWork.UserNotificationRepository.GetAll().Where(e => e.Id == id && e.RecipientId == userId).FirstOrDefault();

                if (notification == null)
                {
                    return this.JsonError(HttpStatusCode.NotFound, 0, "not found the notification");
                }
                else
                {
                    notification.Status = Domain.DbModels.UserNotificationStatus.Deleted;
                    unitOfWork.UserNotificationRepository.Edit(notification);
                    await unitOfWork.SaveAsync();
                }

                return this.Ok();
            }
        }
    }
}
