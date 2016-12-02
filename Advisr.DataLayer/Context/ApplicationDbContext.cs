using Advisr.Domain.DbModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Advisr.DataLayer.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext()
            : base(DbConstants.DataBaseConnectionName/*, throwIfV1Schema: false*/)
        {
        }
        
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<AutopilotErrorBuffer> AutopilotFailedOperations { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Coverage> Coverages { get; set; }
        public virtual DbSet<CustomerLog> CustomerChanges { get; set; }
        //public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public virtual DbSet<CustomerDetails> CustomerDetails { get; set; }
        public virtual DbSet<Home_P> Home_Ps { get; set; }
        public virtual DbSet<Life_P> Life_Ps { get; set; }
        public virtual DbSet<Vehicle_P> Vehicle_Ps { get; set; }
        public virtual DbSet<Insurer> Insurers { get; set; }
        public virtual DbSet<Policy> Policies { get; set; }
        public virtual DbSet<PolicyCoverage> PolicyCoverages { get; set; }
        public virtual DbSet<PolicyFile> PolicyFiles { get; set; }
        public virtual DbSet<PolicyType> PolicyTypes { get; set; }
        public virtual DbSet<PolicyTypeField> PolicyTypeFields { get; set; }
        public virtual DbSet<PolicyTypeCoverage> PolicyTypeCoverage { get; set; }
        public virtual DbSet<AdditionalPolicyProperty> AdditionalPolicyProperties { get; set; }
        public virtual DbSet<UserPolicy> UserPolicies { get; set; }
        public virtual DbSet<VehicleModel> VehicleModels { get; set; }
        public virtual DbSet<VehicleMake> VehicleMakes { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            //modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            //modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles");
            //modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
            //modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogins");
            
            //modelBuilder.Entity<ApplicationUser>()
            //            .HasKey(c => c.Id);

            modelBuilder.Entity<ApplicationUser>()
                        .HasMany(x => x.CreatedUsers)
                        .WithOptional()
                        .HasForeignKey(g => g.CreatedById)
                        .WillCascadeOnDelete(false);

            //modelBuilder.Entity<VehicleMake>()
            //            .HasMany(x => x.VehicleModels)
            //            .WithOptional()
            //            .HasForeignKey(g => g.VehicleMakeId)
            //            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vehicle_P>()
                        .HasRequired(c => c.Make)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<VehicleModel>()
                        .HasRequired(s => s.VehicleMake)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<PolicyTypeCoverage>()
                        .HasRequired(s => s.PolicyType)
                        .WithMany()
                        .WillCascadeOnDelete(false);


            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
