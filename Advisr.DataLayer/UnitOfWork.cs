using Advisr.DataLayer.Context;
using Advisr.Domain.DbModels;
using System;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Advisr.DataLayer
{
    public partial class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ApplicationDbContext dbContext;
        private DbContextTransaction dbContextTransaction = null;
        
        private IRepository<File> fileRepository;
        public IRepository<File> FileRepository
        {
            get
            {
                if (this.fileRepository == null)
                {
                    this.fileRepository = new Repository<File>(this.dbContext);
                }
                return this.fileRepository;
            }
        }

        private IRepository<UserNotification> userNotificationRepository;
        public IRepository<UserNotification> UserNotificationRepository
        {
            get
            {
                if (this.userNotificationRepository == null)
                {
                    this.userNotificationRepository = new Repository<UserNotification>(this.dbContext);
                }
                return this.userNotificationRepository;
            }
        }

        private IRepository<ApplicationUser> userRepository;
        public IRepository<ApplicationUser> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new Repository<ApplicationUser>(this.dbContext);
                }
                return this.userRepository;
            }
        }

        private IRepository<ApplicationUserRole> userRoleRepository;
        public IRepository<ApplicationUserRole> UserRoleRepository
        {
            get
            {
                if (this.userRoleRepository == null)
                {
                    this.userRoleRepository = new Repository<ApplicationUserRole>(this.dbContext);
                }
                return this.userRoleRepository;
            }
        }


        private IRepository<ApplicationUserLogin> userLoginRepository;
        public IRepository<ApplicationUserLogin> UserLoginRepository
        {
            get
            {
                if (this.userLoginRepository == null)
                {
                    this.userLoginRepository = new Repository<ApplicationUserLogin>(this.dbContext);
                }
                return this.userLoginRepository;
            }
        }

        private IRepository<ApplicationRole> roleRepository;
        public IRepository<ApplicationRole> RoleRepository
        {
            get
            {
                if (this.roleRepository == null)
                {
                    this.roleRepository = new Repository<ApplicationRole>(this.dbContext);
                }
                return this.roleRepository;
            }
        }

        private IRepository<AutopilotErrorBuffer> autopilotErrorsRepository { get; set; }
        public IRepository<AutopilotErrorBuffer> AutopilotErrorRepository
        {
            get
            {
                if (this.autopilotErrorsRepository == null)
                {
                    this.autopilotErrorsRepository = new Repository<AutopilotErrorBuffer>(this.dbContext);
                }
                return this.autopilotErrorsRepository;
            }
        }

        private IRepository<Address> addressRepository;
        public IRepository<Address> AddressRepository
        {
            get
            {
                if (this.addressRepository == null)
                {
                    this.addressRepository = new Repository<Address>(this.dbContext);
                }
                return this.addressRepository;
            }
        }
        
        private IRepository<Coverage> coverageRepository;
        public IRepository<Coverage> CoverageRepository
        {
            get
            {
                if (this.coverageRepository == null)
                {
                    this.coverageRepository = new Repository<Coverage>(this.dbContext);
                }
                return this.coverageRepository;
            }
        }

        private IRepository<CustomerDetails> customerDetailsRepository;
        public IRepository<CustomerDetails> CustomerDetailsRepository
        {
            get
            {
                if (this.customerDetailsRepository == null)
                {
                    this.customerDetailsRepository = new Repository<CustomerDetails>(this.dbContext);
                }
                return this.customerDetailsRepository;
            }
        }

        private IRepository<CustomerLog> customerLogRepository;
        public IRepository<CustomerLog> CustomerLogRepository
        {
            get
            {
                if (this.customerLogRepository == null)
                {
                    this.customerLogRepository = new Repository<CustomerLog>(this.dbContext);
                }
                return this.customerLogRepository;
            }
        }

        //private IRepository<CustomerAddress> customerAddressRepository;
        //public IRepository<CustomerAddress> CustomerAddressRepository
        //{
        //    get
        //    {
        //        if (this.customerAddressRepository == null)
        //        {
        //            this.customerAddressRepository = new Repository<CustomerAddress>(this.dbContext);
        //        }
        //        return this.customerAddressRepository;
        //    }
        //}

        private IRepository<Insurer> insurerRepository;
        public IRepository<Insurer> InsurerRepository
        {
            get
            {
                if (this.insurerRepository == null)
                {
                    this.insurerRepository = new Repository<Insurer>(this.dbContext);
                }
                return this.insurerRepository;
            }
        }

        private IRepository<Policy> policyRepository;
        public IRepository<Policy> PolicyRepository
        {
            get
            {
                if (this.policyRepository == null)
                {
                    this.policyRepository = new Repository<Policy>(this.dbContext);
                }
                return this.policyRepository;
            }
        }

        private IRepository<PolicyCoverage> policyCoverageRepository;
        public IRepository<PolicyCoverage> PolicyCoverageRepository
        {
            get
            {
                if (this.policyCoverageRepository == null)
                {
                    this.policyCoverageRepository = new Repository<PolicyCoverage>(this.dbContext);
                }
                return this.policyCoverageRepository;
            }
        }

        private IRepository<PolicyFile> policyFileRepository;
        public IRepository<PolicyFile> PolicyFileRepository
        {
            get
            {
                if (this.policyFileRepository == null)
                {
                    this.policyFileRepository = new Repository<PolicyFile>(this.dbContext);
                }
                return this.policyFileRepository;
            }
        }

        private IRepository<PolicyType> policyTypeRepository;
        public IRepository<PolicyType> PolicyTypeRepository
        {
            get
            {
                if (this.policyTypeRepository == null)
                {
                    this.policyTypeRepository = new Repository<PolicyType>(this.dbContext);
                }
                return this.policyTypeRepository;
            }
        }

        private IRepository<PolicyTypeField> policyTypeFieldRepository;
        public IRepository<PolicyTypeField> PolicyTypeFieldRepository
        {
            get
            {
                if (this.policyTypeFieldRepository == null)
                {
                    this.policyTypeFieldRepository = new Repository<PolicyTypeField>(this.dbContext);
                }
                return this.policyTypeFieldRepository;
            }
        }
        
        private IRepository<PolicyTypeCoverage> policyTypeCoverageRepository;
        public IRepository<PolicyTypeCoverage> PolicyTypeCoverageRepository
        {
            get
            {
                if (this.policyTypeCoverageRepository == null)
                {
                    this.policyTypeCoverageRepository = new Repository<PolicyTypeCoverage>(this.dbContext);
                }
                return this.policyTypeCoverageRepository;
            }
        }

        private IRepository<AdditionalPolicyProperty> additionalPolicyPropertyRepository;
        public IRepository<AdditionalPolicyProperty> AdditionalPolicyPropertyRepository
        {
            get
            {
                if (this.additionalPolicyPropertyRepository == null)
                {
                    this.additionalPolicyPropertyRepository = new Repository<AdditionalPolicyProperty>(this.dbContext);
                }
                return this.additionalPolicyPropertyRepository;
            }
        }

        private IRepository<UserPolicy> userPolicyRepository;
        public IRepository<UserPolicy> UserPolicyRepository
        {
            get
            {
                if (this.userPolicyRepository == null)
                {
                    this.userPolicyRepository = new Repository<UserPolicy>(this.dbContext);
                }
                return this.userPolicyRepository;
            }
        }

        private IRepository<Home_P> home_PRepository;
        public IRepository<Home_P> Home_PRepository
        {
            get
            {
                if (this.home_PRepository == null)
                {
                    this.home_PRepository = new Repository<Home_P>(this.dbContext);
                }
                return this.home_PRepository;
            }
        }

        private IRepository<Life_P> life_PRepository;
        public IRepository<Life_P> Life_PRepository
        {
            get
            {
                if (this.life_PRepository == null)
                {
                    this.life_PRepository = new Repository<Life_P>(this.dbContext);
                }
                return this.life_PRepository;
            }
        }

        private IRepository<Vehicle_P> vehicle_PRepository;
        public IRepository<Vehicle_P> Vehicle_PRepository
        {
            get
            {
                if (this.vehicle_PRepository == null)
                {
                    this.vehicle_PRepository = new Repository<Vehicle_P>(this.dbContext);
                }
                return this.vehicle_PRepository;
            }
        }

        private IRepository<VehicleModel> vehicleModelRepository;
        public IRepository<VehicleModel> VehicleModelRepository
        {
            get
            {
                if (this.vehicleModelRepository == null)
                {
                    this.vehicleModelRepository = new Repository<VehicleModel>(this.dbContext);
                }
                return this.vehicleModelRepository;
            }
        }

        private IRepository<VehicleMake> vehicleMakeRepository;
        public IRepository<VehicleMake> VehicleMakeRepository
        {
            get
            {
                if (this.vehicleMakeRepository == null)
                {
                    this.vehicleMakeRepository = new Repository<VehicleMake>(this.dbContext);
                }
                return this.vehicleMakeRepository;
            }
        }
        

        public DbContext CurrentDbContext
        {
            get
            {
                return this.dbContext;
            }
        }

        private UnitOfWork()
        {
            this.dbContext = new ApplicationDbContext();
        }

        public int Save()
        {
            try
            {
                return this.dbContext.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ve)
            {
                foreach (var item in ve.EntityValidationErrors)
                {
                    foreach (var error in item.ValidationErrors)
                    {
                        Debug.WriteLine(string.Format("ValidationError: {0}-{1}", error.PropertyName, error.ErrorMessage));
                    }
                }
                throw;
            }
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await this.dbContext.SaveChangesAsync();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ve)
            {
                foreach (var item in ve.EntityValidationErrors)
                {
                    foreach (var error in item.ValidationErrors)
                    {
                        Debug.WriteLine(string.Format("ValidationError: {0}-{1}", error.PropertyName, error.ErrorMessage));
                    }
                }
                throw;
            }
        }


        public void BeginTransaction()
        {
            if (this.dbContextTransaction != null)
            {
                throw new FieldAccessException("Need to commit or rollback previously transation");
            }

            this.dbContextTransaction = this.dbContext.Database.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (this.dbContextTransaction != null)
            {
                throw new FieldAccessException("Need to commit or rollback previously transation");
            }

            this.dbContextTransaction = this.dbContext.Database.BeginTransaction(isolationLevel);
        }

        public void RollbackTransaction()
        {
            if (this.dbContextTransaction == null)
            {
                throw new FieldAccessException("Transaction was not begining");
            }
            this.dbContextTransaction.Rollback();
            this.dbContextTransaction.Dispose();
            this.dbContextTransaction = null;
        }

        public void CommitTransaction()
        {
            if (this.dbContextTransaction == null)
            {
                throw new FieldAccessException("Transaction was not begining");
            }
            this.dbContextTransaction.Commit();
            this.dbContextTransaction.Dispose();
            this.dbContextTransaction = null;
        }


        public void Detached(object entity)
        {
            this.dbContext.Entry(entity).State = EntityState.Detached;
        }

        public static IUnitOfWork Create()
        {
            return new UnitOfWork();
        }

        public void Dispose()
        {
            if (this.dbContextTransaction != null)
            {
                this.RollbackTransaction();
            }

            this.dbContext.Dispose();
        }



    }
}
