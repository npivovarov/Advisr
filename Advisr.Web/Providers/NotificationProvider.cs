using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using System;
using System.Threading.Tasks;

namespace Advisr.Web.Providers
{
    public class NotificationProvider
    {
        private static NotificationProvider instance;
        private static object lockPoint = new object();

        private NotificationProvider()
        { }

        public static NotificationProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockPoint)
                    {
                        if (instance == null)
                        {
                            instance = new NotificationProvider();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Send notification abount expire policy
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="title"></param>
        /// <param name="subjectTitleFirst"></param>
        /// <param name="subjectTitleSecond"></param>
        /// <param name="bodyText"></param>
        /// <param name="policyId"></param>
        /// <param name="recipientUserId"></param>
        /// <returns></returns>
        public async Task<UserNotification> SendNotificationExpirePolicy(
                                    IUnitOfWork unitOfWork,
                                    string title,
                                    string subjectTitleFirst,
                                    string subjectTitleSecond,
                                    string bodyText,
                                    int policyId,
                                    NotificationTargetType type,
                                    string recipientUserId
                                    )
        {
            var notification = await this.SendNotification(
                                unitOfWork,
                                title, 
                                subjectTitleFirst, 
                                subjectTitleSecond, 
                                bodyText, 
                                NotificationType.Information,
                                type, 
                                policyId, 
                                null, 
                                recipientUserId);
            
            return notification;
        }


        /// <summary>
        /// Create notification 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="title"></param>
        /// <param name="subjectTitleFirst"></param>
        /// <param name="subjectTitleSecond"></param>
        /// <param name="bodyText"></param>
        /// <param name="notificationType"></param>
        /// <param name="targetObjectType"></param>
        /// <param name="targetObjectId"></param>
        /// <param name="targetUrl"></param>
        /// <param name="recipientUserId"></param>
        /// <returns></returns>
        public async Task<UserNotification> SendNotification(
                                            IUnitOfWork unitOfWork, 
                                            string title, 
                                            string subjectTitleFirst,
                                            string subjectTitleSecond,
                                            string bodyText,
                                            NotificationType notificationType,
                                            NotificationTargetType targetObjectType,
                                            int? targetObjectId,
                                            string targetUrl,
                                            string recipientUserId
                                            )
        {
            UserNotification userNotification = new UserNotification();
            userNotification.Title = title;
            userNotification.SubjectTitleFirst = subjectTitleFirst;
            userNotification.SubjectTitleSecond = subjectTitleSecond;
            userNotification.Body = bodyText;
            userNotification.NotificationType = notificationType;
            userNotification.TargetObjectType = targetObjectType;
            userNotification.TargetObjectId = targetObjectId;
            userNotification.TargetUrl = targetUrl;
            userNotification.RecipientId = recipientUserId;
            userNotification.CreatedDate = DateTime.Now;

            unitOfWork.UserNotificationRepository.Insert(userNotification);

            await unitOfWork.SaveAsync();

            return userNotification;
        }
    }
}