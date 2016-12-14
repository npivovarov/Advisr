using Advisr.Domain.DbModels;
using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Advisr.DataLayer
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();

        void BeginTransaction(IsolationLevel isolationLevel);

        void RollbackTransaction();

        void CommitTransaction();

        void Detached(object entity);

        //-----------------------------------------------
        IRepository<File> FileRepository { get; }
        IRepository<UserNotification> UserNotificationRepository { get; }
        IRepository<ApplicationUser> UserRepository { get; }
        IRepository<ApplicationUserRole> UserRoleRepository { get; }
        IRepository<ApplicationRole> RoleRepository { get; }
        IRepository<ApplicationUserLogin> UserLoginRepository { get; }
        IRepository<AutopilotErrorBuffer> AutopilotErrorRepository{ get; }
        IRepository<Address> AddressRepository { get; }
        IRepository<Coverage> CoverageRepository { get; }
        IRepository<CustomerDetails> CustomerDetailsRepository { get; }
        IRepository<CustomerLog> CustomerLogRepository { get; }
        //IRepository<CustomerAddress> CustomerAddressRepository { get; }
        IRepository<Insurer> InsurerRepository { get; }
        IRepository<Policy> PolicyRepository { get; }
        IRepository<PolicyCoverage> PolicyCoverageRepository { get; }
        IRepository<PolicyFile> PolicyFileRepository { get; }
        IRepository<PolicyGroup> PolicyGroupRepository { get; }
        IRepository<PolicyType> PolicyTypeRepository { get; }
        IRepository<PolicyProperty> PolicyPropertyRepository { get; }
        IRepository<PolicyTypePolicyProperty> PolicyTypePolicyPropertyRepository { get; }
        IRepository<PolicyTypeCoverage> PolicyTypeCoverageRepository { get; }
        IRepository<PolicyPolicyProperty> PolicyPolicyPropertyRepository { get; }
        IRepository<PolicyTemplate> PolicyTemplateRepository { get; }
        IRepository<PolicyTemplatePolicyProperty> PolicyTemplatePolicyPropertyRepository { get; }
        
        IRepository<UserPolicy> UserPolicyRepository { get; }
        IRepository<VehicleModel> VehicleModelRepository { get; }
        IRepository<VehicleMake> VehicleMakeRepository { get; }
        //------------------------------------------------

        DbContext CurrentDbContext { get; }

        int Save();

        Task<int> SaveAsync();
    }
}
