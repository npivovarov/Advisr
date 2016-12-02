using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class UserNotification
    {
        public int Id { get; set; }

        public string RecipientId { get; set; }

        public string SubjectTitleFirst  { get; set; }

        public string SubjectTitleSecond { get; set; }
        
        public string Title { get; set; }

        public string Body { get; set; }
        
        public UserNotificationStatus Status { get; set; }

        public NotificationType NotificationType { get; set; }

        public NotificationTargetType TargetObjectType { get; set; }

        public int? TargetObjectId { get; set; }

        public string TargetUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ReadDate { get; set; }
        
        public string CreatedById { get; set; }

        //-----------------------------------
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }
        
        [ForeignKey("RecipientId")]
        public virtual ApplicationUser Recipient { get; set; }
    }

    public enum UserNotificationStatus
    {
        None = 0,
        Read = 1,
        Deleted = 2
    }
    
    public enum NotificationType
    {
        Information = 0,
        Warning = 1,
    }

    public enum NotificationTargetType
    {
        None = 0,
        Policy = 1,
        Profile = 2,
    }


}
