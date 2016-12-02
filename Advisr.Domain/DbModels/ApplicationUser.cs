using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Advisr.Domain.DbModels
{
    [Table("Users")]
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUser, IUser<string>, IBaseEntity
    {
        [MaxLength(65)]
        public string FirstName { get; set; }

        [MaxLength(65)]
        public string LastName { get; set; }
        
        public Guid? AvatarFileId { get; set; }

        public bool HasTempPassword { get; set; }

        public string CultureName { get; set; }
        
        public ApplicationUserStatus Status { get; set; }

        public bool Hidden { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }

        public bool LockedByAdmin { get; set; }

        public string LockedByAdminId { get; set; }

        public string AutopilotContactId { get; set; }

        public string AutopilotData { get; set; }

        public bool AutopilotTrack { get; set; }
        //-----------------------------------
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        //[ForeignKey("ModifiedById")]
        //public virtual ApplicationUser ModifiedBy { get; set; }


        public virtual File AvatarFile { get; set; }

        public virtual ICollection<UserPolicy> UserPolicies { get; set; }

        public virtual ICollection<ApplicationUser> CreatedUsers { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            
            // Add custom user claims here
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, this.Email));
            
            return userIdentity;
        }

        public string GetFullName()
        {
            return string.Concat(this.FirstName, " ", this.LastName);
        }
    }

    [Table("Roles")]
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public bool Hidden { get; set; }

    }

    [Table("UserRoles")]
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }

    [Table("UserClaims")]
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {

    }

    [Table("UserLogins")]
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        
    }

    public enum ApplicationUserStatus
    {
        Active = 0,
        Past = 1
    }
}
