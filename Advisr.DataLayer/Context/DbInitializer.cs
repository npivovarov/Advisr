using Advisr.Domain.DbModels;
using Microsoft.AspNet.Identity;
using System;

namespace Advisr.DataLayer.Context
{
    public class DbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            #region UserRoles

            ApplicationRole adminUserRole = new ApplicationRole();
            adminUserRole.Id = DbConstants.AdminUserRole;
            adminUserRole.Name = DbConstants.AdminUserRole;

            ApplicationRole customerUserRole = new ApplicationRole();
            customerUserRole.Id = DbConstants.CustomerUserRole;
            customerUserRole.Name = DbConstants.CustomerUserRole;

            context.Roles.Add(adminUserRole);
            context.Roles.Add(customerUserRole);
           
            #endregion

            #region Application Users

            PasswordHasher passwordHasher = new PasswordHasher();
            var passwordHash = passwordHasher.HashPassword("Qa123456+");
  
            var userSystem = new ApplicationUser
            {
                Id = DbConstants.SystemUserId,
                UserName = "System_User",
                Email = "noemail@advisr.com",
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "",
                PasswordHash = "--",
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                Hidden = true,
                CreatedDate = DateTime.Now,
                CreatedById = DbConstants.SystemUserId
            };
            
            var userAdmin = new ApplicationUser
            {
                Id = DbConstants.AdminUserId,
                UserName = "Admin",
                Email = "advisr@qarea.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Advisr",
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                AutopilotTrack = false,
                CreatedDate = DateTime.Now,
                CreatedById = DbConstants.SystemUserId
            };

            var userTestCustomer = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Customer",
                Email = "customer@advisr.com",
                FirstName = "Test",
                LastName = "Customer",
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                CreatedDate = DateTime.Now,
                CreatedById = DbConstants.SystemUserId
            };

            context.Users.Add(userSystem);
            context.Users.Add(userAdmin);
            context.Users.Add(userTestCustomer);

            context.SaveChanges();
            
            ApplicationUserRole userRole1 = new ApplicationUserRole();
            userRole1.RoleId = adminUserRole.Id;
            userRole1.UserId = userSystem.Id;

            ApplicationUserRole userRole2 = new ApplicationUserRole();
            userRole2.RoleId = adminUserRole.Id;
            userRole2.UserId = userAdmin.Id;
            
            ApplicationUserRole userRole3 = new ApplicationUserRole();
            userRole3.RoleId = customerUserRole.Id;
            userRole3.UserId = userTestCustomer.Id;

            var userRoleSet = context.Set<ApplicationUserRole>();
            userRoleSet.Add(userRole1);
            userRoleSet.Add(userRole2);
            userRoleSet.Add(userRole3);

            context.SaveChanges();

            #endregion


            #region Customer Details For Admin
            
            Address address = new Address();
            address.CreatedById = userAdmin.Id;
            address.CreatedDate = DateTime.Now;

            context.Addresses.Add(address);
            context.SaveChanges();

            CustomerDetails customer = new CustomerDetails();
            customer.Id = Guid.NewGuid();
            customer.CreatedById = userAdmin.Id;
            customer.CreatedDate = DateTime.Now;
            customer.UserId = userAdmin.Id;
            customer.ContactPhone = "";
            customer.AddressId = address.Id;
            customer.DateOfBirth = new DateTime(1999, 1, 1);
            customer.Gender = Gender.Male;
            customer.Status = CustomerStatus.Completed;

            customer.ModifiedReason = "created";
            context.CustomerDetails.Add(customer);

            #endregion

            #region Insurers
            File f1 = new File();
            f1.FileName = "AAMI Logo";
            f1.ContentType = "image/jpeg";
            f1.IsTemp = false;
            f1.CreatedDate = DateTime.Now;
            f1.CreatedById = DbConstants.SystemUserId;
           
            File f2 = new File();
            f2.FileName = "AAMI Logo";
            f2.ContentType = "image/jpeg";
            f2.IsTemp = false;
            f2.CreatedDate = DateTime.Now;
            f2.CreatedById = DbConstants.SystemUserId;

            context.Files.Add(f1);

            context.Files.Add(f2);

            context.SaveChanges();

            Insurer insurer1 = new Insurer();
            insurer1.Name = "AAMI";
            insurer1.URL = "www.aami.com.au";
            insurer1.Phone = "13 12 44";
            insurer1.PhoneOverseas = "13 12 44";
            insurer1.LogoFileId = f1.Id;
            insurer1.Color = "#ff0000";
            insurer1.CreatedDate = DateTime.Now;
            insurer1.CreatedById = DbConstants.SystemUserId;

            Insurer insurer2 = new Insurer();
            insurer2.Name = "Bindle";
            insurer2.URL = "www.bindle.com.au";
            insurer2.Phone = "13 12 44";
            insurer2.PhoneOverseas = "13 12 44";
            insurer2.LogoFileId = f2.Id;
            insurer2.Color = "#ff0000";
            insurer2.CreatedDate = DateTime.Now;
            insurer2.CreatedById = DbConstants.SystemUserId;


            context.Insurers.Add(insurer1);

            context.Insurers.Add(insurer2);

            context.SaveChanges();
            #endregion

            #region PolicyGroup data

            PolicyType vehicleCar = new PolicyType();
            vehicleCar.GroupName = "Vehicle";
            vehicleCar.PolicyTypeName = "Car";
            vehicleCar.PolicyGroupType = PolicyGroupType.Vehicle;
            vehicleCar.CreatedById = userSystem.Id;
            vehicleCar.CreatedDate = DateTime.Now;
            vehicleCar.InsurerId = insurer1.Id;
            vehicleCar.Status = Status.Active;

            PolicyType vehicleMotorbike = new PolicyType();
            vehicleMotorbike.GroupName = "Vehicle";
            vehicleMotorbike.PolicyTypeName = "Motorbike";
            vehicleMotorbike.PolicyGroupType = PolicyGroupType.Vehicle;
            vehicleMotorbike.CreatedById = userSystem.Id;
            vehicleMotorbike.CreatedDate = DateTime.Now;
            vehicleMotorbike.InsurerId = insurer1.Id;
            vehicleMotorbike.Status = Status.Active;

            PolicyType vehicleCar1 = new PolicyType();
            vehicleCar1.GroupName = "Vehicle";
            vehicleCar1.PolicyTypeName = "Car";
            vehicleCar1.PolicyGroupType = PolicyGroupType.Vehicle;
            vehicleCar1.CreatedById = userSystem.Id;
            vehicleCar1.CreatedDate = DateTime.Now;
            vehicleCar1.InsurerId = insurer2.Id;
            vehicleCar1.Status = Status.Active;

            PolicyType vehicleMotorbike1 = new PolicyType();
            vehicleMotorbike1.GroupName = "Vehicle";
            vehicleMotorbike1.PolicyTypeName = "Motorbike";
            vehicleMotorbike1.PolicyGroupType = PolicyGroupType.Vehicle;
            vehicleMotorbike1.CreatedById = userSystem.Id;
            vehicleMotorbike1.CreatedDate = DateTime.Now;
            vehicleMotorbike1.InsurerId = insurer2.Id;
            vehicleMotorbike1.Status = Status.Active;

            context.PolicyTypes.Add(vehicleCar);
            context.PolicyTypes.Add(vehicleMotorbike);
            context.PolicyTypes.Add(vehicleCar1);
            context.PolicyTypes.Add(vehicleMotorbike1);

            PolicyType meMedical = new PolicyType();
            meMedical.GroupName = "Me";
            meMedical.PolicyTypeName = "Medical";
            meMedical.PolicyGroupType = PolicyGroupType.Life;
            meMedical.CreatedById = userSystem.Id;
            meMedical.CreatedDate = DateTime.Now;
            meMedical.InsurerId = insurer1.Id;
            meMedical.Status = Status.Active;

            PolicyType meTravel = new PolicyType();
            meTravel.GroupName = "Me";
            meTravel.PolicyTypeName = "Travel";
            meTravel.PolicyGroupType = PolicyGroupType.Life;
            meTravel.CreatedById = userSystem.Id;
            meTravel.CreatedDate = DateTime.Now;
            meTravel.InsurerId = insurer1.Id;
            meTravel.Status = Status.Active;

            PolicyType meMedical1 = new PolicyType();
            meMedical1.GroupName = "Me";
            meMedical1.PolicyTypeName = "Medical";
            meMedical1.PolicyGroupType = PolicyGroupType.Life;
            meMedical1.CreatedById = userSystem.Id;
            meMedical1.CreatedDate = DateTime.Now;
            meMedical1.InsurerId = insurer2.Id;
            meMedical1.Status = Status.Active;

            PolicyType meTravel1 = new PolicyType();
            meTravel1.GroupName = "Me";
            meTravel1.PolicyTypeName = "Travel";
            meTravel1.PolicyGroupType = PolicyGroupType.Life;
            meTravel1.CreatedById = userSystem.Id;
            meTravel1.CreatedDate = DateTime.Now;
            meTravel1.InsurerId = insurer2.Id;
            meTravel1.Status = Status.Active;

            context.PolicyTypes.Add(vehicleCar);
            context.PolicyTypes.Add(vehicleMotorbike);
            context.PolicyTypes.Add(vehicleCar1);
            context.PolicyTypes.Add(vehicleMotorbike1);
            context.PolicyTypes.Add(meMedical);
            context.PolicyTypes.Add(meTravel);
            context.PolicyTypes.Add(meMedical1);
            context.PolicyTypes.Add(meTravel1);
            context.SaveChanges();

            PolicyTypeField vehicleCarColor = new PolicyTypeField();
            vehicleCarColor.PolicyTypeId = vehicleCar.Id;
            vehicleCarColor.FieldName = "Color";
            vehicleCarColor.FieldType = PolicyTypeFieldType.List;
            vehicleCarColor.ListDesription = "[\"red\",\"black\",\"white\",\"blue\",\"green\"]";
            vehicleCarColor.CreatedById = userSystem.Id;
            vehicleCarColor.CreatedDate = DateTime.Now;
            vehicleCarColor.Status = FieldStatus.Active;

            PolicyTypeField vehicleBodyType = new PolicyTypeField();
            vehicleBodyType.PolicyTypeId = vehicleCar.Id;
            vehicleBodyType.FieldName = "Body Type";
            vehicleBodyType.FieldType = PolicyTypeFieldType.List;
            vehicleBodyType.ListDesription = "[\"off-road\",\"sedan\",\"station wagon\"]";
            vehicleBodyType.CreatedById = userSystem.Id;
            vehicleBodyType.CreatedDate = DateTime.Now;
            vehicleBodyType.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarImmobilaser = new PolicyTypeField();
            vehicleCarImmobilaser.PolicyTypeId = vehicleCar.Id;
            vehicleCarImmobilaser.FieldName = "Immobilaser";
            vehicleCarImmobilaser.FieldType = PolicyTypeFieldType.Bool;
            vehicleCarImmobilaser.DefaultValue = "0";
            vehicleCarImmobilaser.CreatedById = userSystem.Id;
            vehicleCarImmobilaser.CreatedDate = DateTime.Now;
            vehicleCarImmobilaser.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarNumberOfKeys = new PolicyTypeField();
            vehicleCarNumberOfKeys.PolicyTypeId = vehicleCar.Id;
            vehicleCarNumberOfKeys.FieldName = "Number Of Keys";
            vehicleCarNumberOfKeys.FieldType = PolicyTypeFieldType.Int;
            vehicleCarNumberOfKeys.CreatedById = userSystem.Id;
            vehicleCarNumberOfKeys.CreatedDate = DateTime.Now;
            vehicleCarNumberOfKeys.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarSecondDriver = new PolicyTypeField();
            vehicleCarSecondDriver.PolicyTypeId = vehicleCar.Id;
            vehicleCarSecondDriver.FieldName = "Second Driver";
            vehicleCarSecondDriver.FieldType = PolicyTypeFieldType.String;
            vehicleCarSecondDriver.CreatedById = userSystem.Id;
            vehicleCarSecondDriver.CreatedDate = DateTime.Now;
            vehicleCarSecondDriver.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarThirdDriver = new PolicyTypeField();
            vehicleCarThirdDriver.PolicyTypeId = vehicleCar.Id;
            vehicleCarThirdDriver.FieldName = "Third Driver";
            vehicleCarThirdDriver.FieldType = PolicyTypeFieldType.String;
            vehicleCarThirdDriver.CreatedById = userSystem.Id;
            vehicleCarThirdDriver.CreatedDate = DateTime.Now;
            vehicleCarThirdDriver.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarColor1 = new PolicyTypeField();
            vehicleCarColor1.PolicyTypeId = vehicleCar1.Id;
            vehicleCarColor1.FieldName = "Color";
            vehicleCarColor1.FieldType = PolicyTypeFieldType.List;
            vehicleCarColor1.ListDesription = "[\"red\",\"black\",\"white\",\"blue\",\"green\"]";
            vehicleCarColor1.CreatedById = userSystem.Id;
            vehicleCarColor1.CreatedDate = DateTime.Now;
            vehicleCarColor1.Status = FieldStatus.Active;

            PolicyTypeField vehicleBodyType1 = new PolicyTypeField();
            vehicleBodyType1.PolicyTypeId = vehicleCar1.Id;
            vehicleBodyType1.FieldName = "Body Type";
            vehicleBodyType1.FieldType = PolicyTypeFieldType.List;
            vehicleBodyType1.ListDesription = "[\"off-road\",\"sedan\",\"station wagon\"]";
            vehicleBodyType1.CreatedById = userSystem.Id;
            vehicleBodyType1.CreatedDate = DateTime.Now;
            vehicleBodyType1.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarImmobilaser1 = new PolicyTypeField();
            vehicleCarImmobilaser1.PolicyTypeId = vehicleCar1.Id;
            vehicleCarImmobilaser1.FieldName = "Immobilaser";
            vehicleCarImmobilaser1.FieldType = PolicyTypeFieldType.Bool;
            vehicleCarImmobilaser1.DefaultValue = "0";
            vehicleCarImmobilaser1.CreatedById = userSystem.Id;
            vehicleCarImmobilaser1.CreatedDate = DateTime.Now;
            vehicleCarImmobilaser1.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarNumberOfKeys1 = new PolicyTypeField();
            vehicleCarNumberOfKeys1.PolicyTypeId = vehicleCar1.Id;
            vehicleCarNumberOfKeys1.FieldName = "Number Of Keys";
            vehicleCarNumberOfKeys1.FieldType = PolicyTypeFieldType.Int;
            vehicleCarNumberOfKeys1.CreatedById = userSystem.Id;
            vehicleCarNumberOfKeys1.CreatedDate = DateTime.Now;
            vehicleCarNumberOfKeys1.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarSecondDriver1 = new PolicyTypeField();
            vehicleCarSecondDriver1.PolicyTypeId = vehicleCar1.Id;
            vehicleCarSecondDriver1.FieldName = "Second Driver";
            vehicleCarSecondDriver1.FieldType = PolicyTypeFieldType.String;
            vehicleCarSecondDriver1.CreatedById = userSystem.Id;
            vehicleCarSecondDriver1.CreatedDate = DateTime.Now;
            vehicleCarSecondDriver1.Status = FieldStatus.Active;

            PolicyTypeField vehicleCarThirdDriver1 = new PolicyTypeField();
            vehicleCarThirdDriver1.PolicyTypeId = vehicleCar1.Id;
            vehicleCarThirdDriver1.FieldName = "Third Driver";
            vehicleCarThirdDriver1.FieldType = PolicyTypeFieldType.String;
            vehicleCarThirdDriver1.CreatedById = userSystem.Id;
            vehicleCarThirdDriver1.CreatedDate = DateTime.Now;
            vehicleCarThirdDriver1.Status = FieldStatus.Active;

            context.PolicyTypeFields.Add(vehicleCarColor);
            context.PolicyTypeFields.Add(vehicleCarImmobilaser);
            context.PolicyTypeFields.Add(vehicleCarNumberOfKeys);
            context.PolicyTypeFields.Add(vehicleBodyType);
            context.PolicyTypeFields.Add(vehicleCarSecondDriver);
            context.PolicyTypeFields.Add(vehicleCarThirdDriver);
            context.PolicyTypeFields.Add(vehicleCarColor1);
            context.PolicyTypeFields.Add(vehicleCarImmobilaser1);
            context.PolicyTypeFields.Add(vehicleCarNumberOfKeys1);
            context.PolicyTypeFields.Add(vehicleBodyType1);
            context.PolicyTypeFields.Add(vehicleCarSecondDriver1);
            context.PolicyTypeFields.Add(vehicleCarThirdDriver1);

            PolicyTypeField vehicleMotorbikeColor = new PolicyTypeField();
            vehicleMotorbikeColor.PolicyTypeId = vehicleMotorbike.Id;
            vehicleMotorbikeColor.FieldName = "Color";
            vehicleMotorbikeColor.FieldType = PolicyTypeFieldType.List;
            vehicleMotorbikeColor.ListDesription = "[\"red\",\"black\",\"white\",\"blue\",\"green\"]";
            vehicleMotorbikeColor.CreatedById = userSystem.Id;
            vehicleMotorbikeColor.CreatedDate = DateTime.Now;
            vehicleMotorbikeColor.Status = FieldStatus.Active;

            PolicyTypeField vehicleMotorbikeNumberOfKeys = new PolicyTypeField();
            vehicleMotorbikeNumberOfKeys.PolicyTypeId = vehicleMotorbike.Id;
            vehicleMotorbikeNumberOfKeys.FieldName = "Number Of Keys";
            vehicleMotorbikeNumberOfKeys.FieldType = PolicyTypeFieldType.Int;
            vehicleMotorbikeNumberOfKeys.DefaultValue = "0";
            vehicleMotorbikeNumberOfKeys.CreatedById = userSystem.Id;
            vehicleMotorbikeNumberOfKeys.CreatedDate = DateTime.Now;
            vehicleMotorbikeNumberOfKeys.Status = FieldStatus.Active;

            PolicyTypeField vehicleMotorbikeColor1 = new PolicyTypeField();
            vehicleMotorbikeColor1.PolicyTypeId = vehicleMotorbike1.Id;
            vehicleMotorbikeColor1.FieldName = "Color";
            vehicleMotorbikeColor1.FieldType = PolicyTypeFieldType.List;
            vehicleMotorbikeColor1.ListDesription = "[\"red\",\"black\",\"white\",\"blue\",\"green\"]";
            vehicleMotorbikeColor1.CreatedById = userSystem.Id;
            vehicleMotorbikeColor1.CreatedDate = DateTime.Now;
            vehicleMotorbikeColor1.Status = FieldStatus.Active;

            PolicyTypeField vehicleMotorbikeNumberOfKeys1 = new PolicyTypeField();
            vehicleMotorbikeNumberOfKeys1.PolicyTypeId = vehicleMotorbike1.Id;
            vehicleMotorbikeNumberOfKeys1.FieldName = "Number Of Keys";
            vehicleMotorbikeNumberOfKeys1.FieldType = PolicyTypeFieldType.Int;
            vehicleMotorbikeNumberOfKeys1.DefaultValue = "0";
            vehicleMotorbikeNumberOfKeys1.CreatedById = userSystem.Id;
            vehicleMotorbikeNumberOfKeys1.CreatedDate = DateTime.Now;
            vehicleMotorbikeNumberOfKeys1.Status = FieldStatus.Active;

            context.PolicyTypeFields.Add(vehicleMotorbikeColor);
            context.PolicyTypeFields.Add(vehicleMotorbikeNumberOfKeys);
            context.PolicyTypeFields.Add(vehicleMotorbikeColor1);
            context.PolicyTypeFields.Add(vehicleMotorbikeNumberOfKeys1);

            PolicyTypeField meMedicalPersone = new PolicyTypeField();
            meMedicalPersone.PolicyTypeId = meMedical.Id;
            meMedicalPersone.FieldName = "Persone Full Name";
            meMedicalPersone.FieldType = PolicyTypeFieldType.String;
            meMedicalPersone.CreatedById = userSystem.Id;
            meMedicalPersone.CreatedDate = DateTime.Now;
            meMedicalPersone.Status = FieldStatus.Active;


            PolicyTypeField meTravelPersone = new PolicyTypeField();
            meTravelPersone.PolicyTypeId = meTravel.Id;
            meTravelPersone.FieldName = "Persone Full Name";
            meTravelPersone.FieldType = PolicyTypeFieldType.String;
            meTravelPersone.CreatedById = userSystem.Id;
            meTravelPersone.CreatedDate = DateTime.Now;
            meTravelPersone.Status = FieldStatus.Active;

            PolicyTypeField meMedicalPersone1 = new PolicyTypeField();
            meMedicalPersone1.PolicyTypeId = meMedical1.Id;
            meMedicalPersone1.FieldName = "Persone Full Name";
            meMedicalPersone1.FieldType = PolicyTypeFieldType.String;
            meMedicalPersone1.CreatedById = userSystem.Id;
            meMedicalPersone1.CreatedDate = DateTime.Now;
            meMedicalPersone1.Status = FieldStatus.Active;


            PolicyTypeField meTravelPersone1 = new PolicyTypeField();
            meTravelPersone1.PolicyTypeId = meTravel1.Id;
            meTravelPersone1.FieldName = "Persone Full Name";
            meTravelPersone1.FieldType = PolicyTypeFieldType.String;
            meTravelPersone1.CreatedById = userSystem.Id;
            meTravelPersone1.CreatedDate = DateTime.Now;
            meTravelPersone1.Status = FieldStatus.Active;

            context.PolicyTypeFields.Add(meMedicalPersone);
            context.PolicyTypeFields.Add(meTravelPersone);
            context.PolicyTypeFields.Add(meMedicalPersone1);
            context.PolicyTypeFields.Add(meTravelPersone1);
            #endregion

            context.SaveChanges();
        }
    }
}
