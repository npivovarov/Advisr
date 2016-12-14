using Advisr.Domain.DbModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;

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

            context.Files.Add(f1);


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

            context.Insurers.Add(insurer1);

            context.SaveChanges();
            #endregion

            #region PolicyGroup data
            PolicyGroup groupVehicle = new PolicyGroup();
            groupVehicle.Name = "Vehicle";

            PolicyGroup groupProperty = new PolicyGroup();
            groupProperty.Name = "Property";

            PolicyGroup groupPersonal = new PolicyGroup();
            groupPersonal.Name = "Personal";

            PolicyGroup groupCommercial = new PolicyGroup();
            groupCommercial.Name = "Commercial";

            context.PolicyGroups.Add(groupVehicle);
            context.PolicyGroups.Add(groupProperty);
            context.PolicyGroups.Add(groupPersonal);
            context.PolicyGroups.Add(groupCommercial);

            context.SaveChanges();

            #endregion

            #region Policy Template for CAR

                List<PolicyProperty> carProperties = new List<PolicyProperty>();

                PolicyProperty carYear = new PolicyProperty();
                carYear.FieldName = "Year";
                carYear.FieldType = PolicyTypeFieldType.Int;
                carYear.PropertyType = PropertyType.PolicyTemplate;
                carYear.IsRequired = true;
                carYear.CreatedById = userSystem.Id;
                carYear.CreatedDate = DateTime.Now;
                carProperties.Add(carYear);

                PolicyProperty carMake = new PolicyProperty();
                carMake.FieldName = "Make";
                carMake.FieldType = PolicyTypeFieldType.String;
                carMake.PropertyType = PropertyType.PolicyTemplate;
                carMake.IsRequired = true;
                carMake.IsSubtitle = true;
                carMake.CreatedById = userSystem.Id;
                carMake.CreatedDate = DateTime.Now;
                carProperties.Add(carMake);

                PolicyProperty carModel = new PolicyProperty();
                carModel.FieldName = "Model";
                carModel.FieldType = PolicyTypeFieldType.String;
                carModel.PropertyType = PropertyType.PolicyTemplate;
                carModel.IsRequired = true;
                carModel.IsSubtitle = true;
                carModel.CreatedById = userSystem.Id;
                carModel.CreatedDate = DateTime.Now;
                carProperties.Add(carModel);

                PolicyProperty carRegistredDriverName = new PolicyProperty();
                carRegistredDriverName.FieldName = "Registred Driver Name";
                carRegistredDriverName.FieldType = PolicyTypeFieldType.String;
                carRegistredDriverName.PropertyType = PropertyType.PolicyTemplate;
                carRegistredDriverName.IsRequired = true;
                carRegistredDriverName.CreatedById = userSystem.Id;
                carRegistredDriverName.CreatedDate = DateTime.Now;
                carProperties.Add(carRegistredDriverName);

                PolicyProperty carRegistrationNumber = new PolicyProperty();
                carRegistrationNumber.FieldName = "Registration Number";
                carRegistrationNumber.FieldType = PolicyTypeFieldType.String;
                carRegistrationNumber.PropertyType = PropertyType.PolicyTemplate;
                carRegistrationNumber.IsRequired = true;
                carRegistrationNumber.IsTitle = true;
                carRegistrationNumber.CreatedById = userSystem.Id;
                carRegistrationNumber.CreatedDate = DateTime.Now;
                carProperties.Add(carRegistrationNumber);

                context.PolicyProperties.AddRange(carProperties);


                PolicyTemplate carPolicyTemplate = new PolicyTemplate();
                carPolicyTemplate.PolicyGroupId = groupVehicle.Id;
                carPolicyTemplate.Name = "Car";
                carPolicyTemplate.CreatedById = userSystem.Id;
                carPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(carPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in carProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = carPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();

                #endregion

            {
                #region Policy Template for Motorbike

                List<PolicyProperty> motorbikeProperties = new List<PolicyProperty>();

                PolicyProperty motorbikeYear = new PolicyProperty();
                motorbikeYear.FieldName = "Year";
                motorbikeYear.FieldType = PolicyTypeFieldType.Int;
                motorbikeYear.PropertyType = PropertyType.PolicyTemplate;
                motorbikeYear.IsRequired = true;
                motorbikeYear.CreatedById = userSystem.Id;
                motorbikeYear.CreatedDate = DateTime.Now;
                motorbikeProperties.Add(motorbikeYear);

                PolicyProperty motorbikeMake = new PolicyProperty();
                motorbikeMake.FieldName = "Make";
                motorbikeMake.FieldType = PolicyTypeFieldType.String;
                motorbikeMake.PropertyType = PropertyType.PolicyTemplate;
                motorbikeMake.IsRequired = true;
                motorbikeMake.IsSubtitle = true;
                motorbikeMake.CreatedById = userSystem.Id;
                motorbikeMake.CreatedDate = DateTime.Now;
                motorbikeProperties.Add(motorbikeMake);

                PolicyProperty motorbikeModel = new PolicyProperty();
                motorbikeModel.FieldName = "Model";
                motorbikeModel.FieldType = PolicyTypeFieldType.String;
                motorbikeModel.PropertyType = PropertyType.PolicyTemplate;
                motorbikeModel.IsRequired = true;
                motorbikeModel.IsSubtitle = true;
                motorbikeModel.CreatedById = userSystem.Id;
                motorbikeModel.CreatedDate = DateTime.Now;
                motorbikeProperties.Add(motorbikeModel);

                PolicyProperty motorbikeRegistredDriverName = new PolicyProperty();
                motorbikeRegistredDriverName.FieldName = "Registred Driver Name";
                motorbikeRegistredDriverName.FieldType = PolicyTypeFieldType.String;
                motorbikeRegistredDriverName.PropertyType = PropertyType.PolicyTemplate;
                motorbikeRegistredDriverName.IsRequired = true;
                motorbikeRegistredDriverName.CreatedById = userSystem.Id;
                motorbikeRegistredDriverName.CreatedDate = DateTime.Now;
                motorbikeProperties.Add(motorbikeRegistredDriverName);

                PolicyProperty motorbikeRegistrationNumber = new PolicyProperty();
                motorbikeRegistrationNumber.FieldName = "Registration Number";
                motorbikeRegistrationNumber.FieldType = PolicyTypeFieldType.String;
                motorbikeRegistrationNumber.PropertyType = PropertyType.PolicyTemplate;
                motorbikeRegistrationNumber.IsRequired = true;
                motorbikeRegistrationNumber.IsTitle = true;
                motorbikeRegistrationNumber.CreatedById = userSystem.Id;
                motorbikeRegistrationNumber.CreatedDate = DateTime.Now;
                motorbikeProperties.Add(motorbikeRegistrationNumber);

                context.PolicyProperties.AddRange(motorbikeProperties);


                PolicyTemplate motorbikePolicyTemplate = new PolicyTemplate();
                motorbikePolicyTemplate.PolicyGroupId = groupVehicle.Id;
                motorbikePolicyTemplate.Name = "Motorbike";
                motorbikePolicyTemplate.CreatedById = userSystem.Id;
                motorbikePolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(motorbikePolicyTemplate);
                context.SaveChanges();

                foreach (var prop in motorbikeProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = motorbikePolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }

                context.SaveChanges();

                #endregion
            }

            {
                #region Policy Template for RV Caravan Trailer

                List<PolicyProperty> сaravanTrailerProperties = new List<PolicyProperty>();

                PolicyProperty сaravanTrailerType = new PolicyProperty();
                сaravanTrailerType.FieldName = "Type";
                сaravanTrailerType.FieldType = PolicyTypeFieldType.List;
                сaravanTrailerType.ListDesription = "[\"Caravan\",\"Caravan 2\"]";
                сaravanTrailerType.PropertyType = PropertyType.PolicyTemplate;
                сaravanTrailerType.IsRequired = true;
                сaravanTrailerType.CreatedById = userSystem.Id;
                сaravanTrailerType.CreatedDate = DateTime.Now;
                сaravanTrailerProperties.Add(сaravanTrailerType);

                PolicyProperty сaravanTrailerYear = new PolicyProperty();
                сaravanTrailerYear.FieldName = "Year";
                сaravanTrailerYear.FieldType = PolicyTypeFieldType.Int;
                сaravanTrailerYear.PropertyType = PropertyType.PolicyTemplate;
                сaravanTrailerYear.IsRequired = true;
                сaravanTrailerYear.CreatedById = userSystem.Id;
                сaravanTrailerYear.CreatedDate = DateTime.Now;
                сaravanTrailerProperties.Add(сaravanTrailerYear);

                PolicyProperty сaravanTrailerMake = new PolicyProperty();
                сaravanTrailerMake.FieldName = "Make";
                сaravanTrailerMake.FieldType = PolicyTypeFieldType.String;
                сaravanTrailerMake.PropertyType = PropertyType.PolicyTemplate;
                сaravanTrailerMake.IsRequired = true;
                сaravanTrailerMake.IsSubtitle = true;
                сaravanTrailerMake.CreatedById = userSystem.Id;
                сaravanTrailerMake.CreatedDate = DateTime.Now;
                сaravanTrailerProperties.Add(сaravanTrailerMake);

                PolicyProperty сaravanTrailerModel = new PolicyProperty();
                сaravanTrailerModel.FieldName = "Model";
                сaravanTrailerModel.FieldType = PolicyTypeFieldType.String;
                сaravanTrailerModel.PropertyType = PropertyType.PolicyTemplate;
                сaravanTrailerModel.IsRequired = true;
                сaravanTrailerModel.IsSubtitle = true;
                сaravanTrailerModel.CreatedById = userSystem.Id;
                сaravanTrailerModel.CreatedDate = DateTime.Now;
                сaravanTrailerProperties.Add(сaravanTrailerModel);


                PolicyProperty сaravanTrailerRegistrationNumber = new PolicyProperty();
                сaravanTrailerRegistrationNumber.FieldName = "Registration Number";
                сaravanTrailerRegistrationNumber.FieldType = PolicyTypeFieldType.String;
                сaravanTrailerRegistrationNumber.PropertyType = PropertyType.PolicyTemplate;
                сaravanTrailerRegistrationNumber.IsRequired = true;
                сaravanTrailerRegistrationNumber.IsTitle = true;
                сaravanTrailerRegistrationNumber.CreatedById = userSystem.Id;
                сaravanTrailerRegistrationNumber.CreatedDate = DateTime.Now;
                сaravanTrailerProperties.Add(сaravanTrailerRegistrationNumber);

                context.PolicyProperties.AddRange(сaravanTrailerProperties);


                PolicyTemplate сaravanTrailerPolicyTemplate = new PolicyTemplate();
                сaravanTrailerPolicyTemplate.PolicyGroupId = groupVehicle.Id;
                сaravanTrailerPolicyTemplate.Name = "Caravan";
                сaravanTrailerPolicyTemplate.CreatedById = userSystem.Id;
                сaravanTrailerPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(сaravanTrailerPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in сaravanTrailerProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = сaravanTrailerPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();

                #endregion
            }

            {
                #region Policy Template for Motorhome

                List<PolicyProperty> motorHomeProperties = new List<PolicyProperty>();

                PolicyProperty motorHomeYear = new PolicyProperty();
                motorHomeYear.FieldName = "Year";
                motorHomeYear.FieldType = PolicyTypeFieldType.Int;
                motorHomeYear.PropertyType = PropertyType.PolicyTemplate;
                motorHomeYear.IsRequired = true;
                motorHomeYear.CreatedById = userSystem.Id;
                motorHomeYear.CreatedDate = DateTime.Now;
                motorHomeProperties.Add(motorHomeYear);

                PolicyProperty motorHomeMake = new PolicyProperty();
                motorHomeMake.FieldName = "Make";
                motorHomeMake.FieldType = PolicyTypeFieldType.String;
                motorHomeMake.PropertyType = PropertyType.PolicyTemplate;
                motorHomeMake.IsRequired = true;
                motorHomeMake.IsSubtitle = true;
                motorHomeMake.CreatedById = userSystem.Id;
                motorHomeMake.CreatedDate = DateTime.Now;
                motorHomeProperties.Add(motorHomeMake);

                PolicyProperty motorHomeModel = new PolicyProperty();
                motorHomeModel.FieldName = "Model";
                motorHomeModel.FieldType = PolicyTypeFieldType.String;
                motorHomeModel.PropertyType = PropertyType.PolicyTemplate;
                motorHomeModel.IsRequired = true;
                motorHomeModel.IsSubtitle = true;
                motorHomeModel.CreatedById = userSystem.Id;
                motorHomeModel.CreatedDate = DateTime.Now;
                motorHomeProperties.Add(motorHomeModel);

                PolicyProperty motorHomeRegistredDriverName = new PolicyProperty();
                motorHomeRegistredDriverName.FieldName = "Registred Driver Name";
                motorHomeRegistredDriverName.FieldType = PolicyTypeFieldType.String;
                motorHomeRegistredDriverName.PropertyType = PropertyType.PolicyTemplate;
                motorHomeRegistredDriverName.IsRequired = true;
                motorHomeRegistredDriverName.CreatedById = userSystem.Id;
                motorHomeRegistredDriverName.CreatedDate = DateTime.Now;
                motorHomeProperties.Add(motorHomeRegistredDriverName);

                PolicyProperty motorHomeRegistrationNumber = new PolicyProperty();
                motorHomeRegistrationNumber.FieldName = "Registration Number";
                motorHomeRegistrationNumber.FieldType = PolicyTypeFieldType.String;
                motorHomeRegistrationNumber.PropertyType = PropertyType.PolicyTemplate;
                motorHomeRegistrationNumber.IsRequired = true;
                motorHomeRegistrationNumber.IsTitle = true;
                motorHomeRegistrationNumber.CreatedById = userSystem.Id;
                motorHomeRegistrationNumber.CreatedDate = DateTime.Now;
                motorHomeProperties.Add(motorHomeRegistrationNumber);

                context.PolicyProperties.AddRange(motorHomeProperties);


                PolicyTemplate motorHomePolicyTemplate = new PolicyTemplate();
                motorHomePolicyTemplate.PolicyGroupId = groupVehicle.Id;
                motorHomePolicyTemplate.Name = "Motorhome";
                motorHomePolicyTemplate.CreatedById = userSystem.Id;
                motorHomePolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(motorHomePolicyTemplate);
                context.SaveChanges();

                foreach (var prop in motorHomeProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = motorHomePolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();

                #endregion
            }

            {
                #region Policy Template for Boat

                List<PolicyProperty> boatProperties = new List<PolicyProperty>();

                PolicyProperty boatYear = new PolicyProperty();
                boatYear.FieldName = "Year";
                boatYear.FieldType = PolicyTypeFieldType.Int;
                boatYear.PropertyType = PropertyType.PolicyTemplate;
                boatYear.IsRequired = true;
                boatYear.CreatedById = userSystem.Id;
                boatYear.CreatedDate = DateTime.Now;
                boatProperties.Add(boatYear);

                PolicyProperty boatMake = new PolicyProperty();
                boatMake.FieldName = "Make";
                boatMake.FieldType = PolicyTypeFieldType.String;
                boatMake.PropertyType = PropertyType.PolicyTemplate;
                boatMake.IsRequired = true;
                boatMake.CreatedById = userSystem.Id;
                boatMake.CreatedDate = DateTime.Now;
                boatProperties.Add(boatMake);

                PolicyProperty boatType = new PolicyProperty();
                boatType.FieldName = "Type";
                boatType.FieldType = PolicyTypeFieldType.String;
                boatType.PropertyType = PropertyType.PolicyTemplate;
                boatType.IsRequired = true;
                boatType.IsSubtitle = true;
                boatType.CreatedById = userSystem.Id;
                boatType.CreatedDate = DateTime.Now;
                boatProperties.Add(boatType);

                PolicyProperty boatModel = new PolicyProperty();
                boatModel.FieldName = "Model";
                boatModel.FieldType = PolicyTypeFieldType.String;
                boatModel.PropertyType = PropertyType.PolicyTemplate;
                boatModel.IsRequired = true;
                boatModel.CreatedById = userSystem.Id;
                boatModel.CreatedDate = DateTime.Now;
                boatProperties.Add(boatModel);

                PolicyProperty boatSpeed = new PolicyProperty();
                boatSpeed.FieldName = "Speed";
                boatSpeed.FieldType = PolicyTypeFieldType.String;
                boatSpeed.PropertyType = PropertyType.PolicyTemplate;
                boatSpeed.IsRequired = true;
                boatSpeed.CreatedById = userSystem.Id;
                boatSpeed.CreatedDate = DateTime.Now;
                boatProperties.Add(boatSpeed);

                PolicyProperty construction = new PolicyProperty();
                construction.FieldName = "Construction";
                construction.FieldType = PolicyTypeFieldType.String;
                construction.PropertyType = PropertyType.PolicyTemplate;
                construction.IsRequired = true;
                construction.CreatedById = userSystem.Id;
                construction.CreatedDate = DateTime.Now;
                boatProperties.Add(construction);

                context.PolicyProperties.AddRange(boatProperties);


                PolicyTemplate boatPolicyTemplate = new PolicyTemplate();
                boatPolicyTemplate.PolicyGroupId = groupVehicle.Id;
                boatPolicyTemplate.Name = "Boat";
                boatPolicyTemplate.CreatedById = userSystem.Id;
                boatPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(boatPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in boatProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = boatPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();

                #endregion
            }

            {
                #region Policy Template for JetSki

                List<PolicyProperty> jetSkiProperties = new List<PolicyProperty>();

                PolicyProperty jetSkiYear = new PolicyProperty();
                jetSkiYear.FieldName = "Year";
                jetSkiYear.FieldType = PolicyTypeFieldType.Int;
                jetSkiYear.PropertyType = PropertyType.PolicyTemplate;
                jetSkiYear.IsRequired = true;
                jetSkiYear.CreatedById = userSystem.Id;
                jetSkiYear.CreatedDate = DateTime.Now;
                jetSkiProperties.Add(jetSkiYear);

                PolicyProperty jetSkiMake = new PolicyProperty();
                jetSkiMake.FieldName = "Make";
                jetSkiMake.FieldType = PolicyTypeFieldType.String;
                jetSkiMake.PropertyType = PropertyType.PolicyTemplate;
                jetSkiMake.IsRequired = true;
                jetSkiMake.IsTitle = true;
                jetSkiMake.CreatedById = userSystem.Id;
                jetSkiMake.CreatedDate = DateTime.Now;
                jetSkiProperties.Add(jetSkiMake);

                PolicyProperty jsType = new PolicyProperty();
                jsType.FieldName = "Type";
                jsType.FieldType = PolicyTypeFieldType.String;
                jsType.PropertyType = PropertyType.PolicyTemplate;
                jsType.IsRequired = true;
                jsType.IsSubtitle = true;
                jsType.CreatedById = userSystem.Id;
                jsType.CreatedDate = DateTime.Now;
                jetSkiProperties.Add(jsType);

                PolicyProperty jetSkiModel = new PolicyProperty();
                jetSkiModel.FieldName = "Model";
                jetSkiModel.FieldType = PolicyTypeFieldType.String;
                jetSkiModel.PropertyType = PropertyType.PolicyTemplate;
                jetSkiModel.IsRequired = true;
                jetSkiModel.CreatedById = userSystem.Id;
                jetSkiModel.CreatedDate = DateTime.Now;
                jetSkiProperties.Add(jetSkiModel);

                PolicyProperty jetSkiSpeed = new PolicyProperty();
                jetSkiSpeed.FieldName = "Speed";
                jetSkiSpeed.FieldType = PolicyTypeFieldType.String;
                jetSkiSpeed.PropertyType = PropertyType.PolicyTemplate;
                jetSkiSpeed.IsRequired = true;
                jetSkiSpeed.CreatedById = userSystem.Id;
                jetSkiSpeed.CreatedDate = DateTime.Now;
                jetSkiProperties.Add(jetSkiSpeed);

                PolicyProperty jetSkiConstruction = new PolicyProperty();
                jetSkiConstruction.FieldName = "Construction";
                jetSkiConstruction.FieldType = PolicyTypeFieldType.String;
                jetSkiConstruction.PropertyType = PropertyType.PolicyTemplate;
                jetSkiConstruction.IsRequired = true;
                jetSkiConstruction.CreatedById = userSystem.Id;
                jetSkiConstruction.CreatedDate = DateTime.Now;
                jetSkiProperties.Add(jetSkiConstruction);

                context.PolicyProperties.AddRange(jetSkiProperties);


                PolicyTemplate jetSkiPolicyTemplate = new PolicyTemplate();
                jetSkiPolicyTemplate.PolicyGroupId = groupVehicle.Id;
                jetSkiPolicyTemplate.Name = "JetSki";
                jetSkiPolicyTemplate.CreatedById = userSystem.Id;
                jetSkiPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(jetSkiPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in jetSkiProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = jetSkiPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();

                #endregion
            }

            {
                #region Policy Template for Private Health

                List<PolicyProperty> healthProperties = new List<PolicyProperty>();

                PolicyProperty policyType = new PolicyProperty();
                policyType.FieldName = "Policy Type";
                policyType.FieldType = PolicyTypeFieldType.List;
                policyType.ListDesription = "[\"Couples\",\"Singles\",\"Family\", \"Single Parent\"]";
                policyType.PropertyType = PropertyType.PolicyTemplate;
                policyType.IsRequired = true;
                policyType.IsTitle = true;
                policyType.CreatedById = userSystem.Id;
                policyType.CreatedDate = DateTime.Now;
                healthProperties.Add(policyType);

                PolicyProperty policyOwner = new PolicyProperty();
                policyOwner.FieldName = "Policy Owner";
                policyOwner.FieldType = PolicyTypeFieldType.String;
                policyOwner.PropertyType = PropertyType.PolicyTemplate;
                policyOwner.IsRequired = true;
                policyOwner.CreatedById = userSystem.Id;
                policyOwner.CreatedDate = DateTime.Now;
                healthProperties.Add(policyOwner);

                PolicyProperty dateOfBirth = new PolicyProperty();
                dateOfBirth.FieldName = "Date of birth";
                dateOfBirth.FieldType = PolicyTypeFieldType.Date;
                dateOfBirth.PropertyType = PropertyType.PolicyTemplate;
                dateOfBirth.IsRequired = true;
                dateOfBirth.CreatedById = userSystem.Id;
                dateOfBirth.CreatedDate = DateTime.Now;
                healthProperties.Add(dateOfBirth);

                PolicyProperty relationShipToOwner = new PolicyProperty();
                relationShipToOwner.FieldName = "Relationship to owner";
                relationShipToOwner.FieldType = PolicyTypeFieldType.String;
                relationShipToOwner.PropertyType = PropertyType.PolicyTemplate;
                relationShipToOwner.IsRequired = false;
                relationShipToOwner.CreatedById = userSystem.Id;
                relationShipToOwner.CreatedDate = DateTime.Now;
                healthProperties.Add(relationShipToOwner);

                context.PolicyProperties.AddRange(healthProperties);


                PolicyTemplate healthPolicyTemplate = new PolicyTemplate();
                healthPolicyTemplate.PolicyGroupId = groupPersonal.Id;
                healthPolicyTemplate.Name = "Private Health";
                healthPolicyTemplate.CreatedById = userSystem.Id;
                healthPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(healthPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in healthProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = healthPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();

                #endregion
            }

            {
                #region Policy Template for Life

                List<PolicyProperty> lifeProperties = new List<PolicyProperty>();

                PolicyProperty lifeInsurancePremiumType = new PolicyProperty();
                lifeInsurancePremiumType.FieldName = "Life Insurance Premium Type";
                lifeInsurancePremiumType.FieldType = PolicyTypeFieldType.String;
                lifeInsurancePremiumType.PropertyType = PropertyType.PolicyTemplate;
                lifeInsurancePremiumType.IsRequired = true;
                lifeInsurancePremiumType.CreatedById = userSystem.Id;
                lifeInsurancePremiumType.CreatedDate = DateTime.Now;
                lifeProperties.Add(lifeInsurancePremiumType);

                PolicyProperty personName = new PolicyProperty();
                personName.FieldName = "Person name";
                personName.FieldType = PolicyTypeFieldType.String;
                personName.PropertyType = PropertyType.PolicyTemplate;
                personName.IsRequired = true;
                personName.IsTitle = true;
                personName.CreatedById = userSystem.Id;
                personName.CreatedDate = DateTime.Now;
                lifeProperties.Add(personName);

                PolicyProperty persondateOfBirth = new PolicyProperty();
                persondateOfBirth.FieldName = "Date of birth";
                persondateOfBirth.FieldType = PolicyTypeFieldType.Date;
                persondateOfBirth.PropertyType = PropertyType.PolicyTemplate;
                persondateOfBirth.IsRequired = true;
                persondateOfBirth.CreatedById = userSystem.Id;
                persondateOfBirth.CreatedDate = DateTime.Now;
                lifeProperties.Add(persondateOfBirth);

                PolicyProperty smoker = new PolicyProperty();
                smoker.FieldName = "Smoker";
                smoker.FieldType = PolicyTypeFieldType.Bool;
                smoker.PropertyType = PropertyType.PolicyTemplate;
                smoker.IsRequired = false;
                smoker.CreatedById = userSystem.Id;
                smoker.CreatedDate = DateTime.Now;
                lifeProperties.Add(smoker);

                context.PolicyProperties.AddRange(lifeProperties);


                PolicyTemplate lifePolicyTemplate = new PolicyTemplate();
                lifePolicyTemplate.PolicyGroupId = groupPersonal.Id;
                lifePolicyTemplate.Name = "Life";
                lifePolicyTemplate.CreatedById = userSystem.Id;
                lifePolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(lifePolicyTemplate);
                context.SaveChanges();

                foreach (var prop in lifeProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = lifePolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();
                #endregion
            }

            {
                #region Policy Template for Travel

                List<PolicyProperty> travelProperties = new List<PolicyProperty>();

                PolicyProperty destination = new PolicyProperty();
                destination.FieldName = "Destination";
                destination.FieldType = PolicyTypeFieldType.String;
                destination.PropertyType = PropertyType.PolicyTemplate;
                destination.IsRequired = true;
                destination.IsTitle = true;
                destination.CreatedById = userSystem.Id;
                destination.CreatedDate = DateTime.Now;
                travelProperties.Add(destination);

                PolicyProperty travellersCovered = new PolicyProperty();
                travellersCovered.FieldName = "Travellers covered";
                travellersCovered.FieldType = PolicyTypeFieldType.Int;
                travellersCovered.PropertyType = PropertyType.PolicyTemplate;
                travellersCovered.IsRequired = true;
                travellersCovered.IsSubtitle = true;
                travellersCovered.CreatedById = userSystem.Id;
                travellersCovered.CreatedDate = DateTime.Now;
                travelProperties.Add(travellersCovered);

                PolicyProperty traveller1 = new PolicyProperty();
                traveller1.FieldName = "Traveller";
                traveller1.FieldType = PolicyTypeFieldType.String;
                traveller1.PropertyType = PropertyType.PolicyTemplate;
                traveller1.IsRequired = false;
                traveller1.CreatedById = userSystem.Id;
                traveller1.CreatedDate = DateTime.Now;
                travelProperties.Add(traveller1);

                PolicyProperty traveller1DateOfBirth = new PolicyProperty();
                traveller1DateOfBirth.FieldName = "Date Of Birth";
                traveller1DateOfBirth.FieldType = PolicyTypeFieldType.Date;
                traveller1DateOfBirth.PropertyType = PropertyType.PolicyTemplate;
                traveller1DateOfBirth.IsRequired = false;
                traveller1DateOfBirth.CreatedById = userSystem.Id;
                traveller1DateOfBirth.CreatedDate = DateTime.Now;
                travelProperties.Add(traveller1DateOfBirth);

                context.PolicyProperties.AddRange(travelProperties);


                PolicyTemplate travelPolicyTemplate = new PolicyTemplate();
                travelPolicyTemplate.PolicyGroupId = groupPersonal.Id;
                travelPolicyTemplate.Name = "Travel";
                travelPolicyTemplate.CreatedById = userSystem.Id;
                travelPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(travelPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in travelProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = travelPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();
                #endregion
            }

            {
                #region Policy Template for Personal-Commercial

                List<PolicyProperty> pCommercialProperties = new List<PolicyProperty>();

                PolicyProperty specialistName = new PolicyProperty();
                specialistName.FieldName = "Person name";
                specialistName.FieldType = PolicyTypeFieldType.String;
                specialistName.PropertyType = PropertyType.PolicyTemplate;
                specialistName.IsRequired = true;
                specialistName.IsTitle = true;
                specialistName.CreatedById = userSystem.Id;
                specialistName.CreatedDate = DateTime.Now;
                pCommercialProperties.Add(specialistName);

                PolicyProperty occupation = new PolicyProperty();
                occupation.FieldName = "Occupation";
                occupation.FieldType = PolicyTypeFieldType.String;
                occupation.PropertyType = PropertyType.PolicyTemplate;
                occupation.IsRequired = true;
                occupation.IsSubtitle = true;
                occupation.CreatedById = userSystem.Id;
                occupation.CreatedDate = DateTime.Now;
                pCommercialProperties.Add(occupation);

                PolicyProperty servicesProvided = new PolicyProperty();
                servicesProvided.FieldName = "Services provided";
                servicesProvided.FieldType = PolicyTypeFieldType.String;
                servicesProvided.PropertyType = PropertyType.PolicyTemplate;
                servicesProvided.IsRequired = false;
                servicesProvided.CreatedById = userSystem.Id;
                servicesProvided.CreatedDate = DateTime.Now;
                pCommercialProperties.Add(servicesProvided);

                context.PolicyProperties.AddRange(pCommercialProperties);


                PolicyTemplate pCommercialPolicyTemplate = new PolicyTemplate();
                pCommercialPolicyTemplate.PolicyGroupId = groupPersonal.Id;
                pCommercialPolicyTemplate.Name = "Commercial";
                pCommercialPolicyTemplate.CreatedById = userSystem.Id;
                pCommercialPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(pCommercialPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in pCommercialProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = pCommercialPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();
                #endregion
            }

            {
                #region Policy Template for Pet

                List<PolicyProperty> petProperties = new List<PolicyProperty>();


                PolicyProperty petName = new PolicyProperty();
                petName.FieldName = "Name";
                petName.FieldType = PolicyTypeFieldType.String;
                petName.PropertyType = PropertyType.PolicyTemplate;
                petName.IsRequired = true;
                petName.IsTitle = true;
                petName.CreatedById = userSystem.Id;
                petName.CreatedDate = DateTime.Now;
                petProperties.Add(petName);

                PolicyProperty petType = new PolicyProperty();
                petType.FieldName = "Pet Type";
                petType.FieldType = PolicyTypeFieldType.List;
                petType.ListDesription = "[\"Dog\",\"Cat\"]";
                petType.PropertyType = PropertyType.PolicyTemplate;
                petType.IsRequired = true;
                petType.IsSubtitle = true;
                petType.CreatedById = userSystem.Id;
                petType.CreatedDate = DateTime.Now;
                petProperties.Add(petType);

                PolicyProperty breedProperty = new PolicyProperty();
                breedProperty.FieldName = "Breed";
                breedProperty.FieldType = PolicyTypeFieldType.String;
                breedProperty.PropertyType = PropertyType.PolicyTemplate;
                breedProperty.IsRequired = true;
                breedProperty.CreatedById = userSystem.Id;
                breedProperty.CreatedDate = DateTime.Now;
                petProperties.Add(breedProperty);

                PolicyProperty dobProperty = new PolicyProperty();
                dobProperty.FieldName = "Date Of Birth";
                dobProperty.FieldType = PolicyTypeFieldType.Date;
                dobProperty.PropertyType = PropertyType.PolicyTemplate;
                dobProperty.IsRequired = true;
                dobProperty.CreatedById = userSystem.Id;
                dobProperty.CreatedDate = DateTime.Now;
                petProperties.Add(dobProperty);

                context.PolicyProperties.AddRange(petProperties);
                context.SaveChanges();

                PolicyTemplate petPolicyTemplate = new PolicyTemplate();
                petPolicyTemplate.PolicyGroupId = groupPersonal.Id;
                petPolicyTemplate.Name = "Pet";
                petPolicyTemplate.CreatedById = userSystem.Id;
                petPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(petPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in petProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = petPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();

                #endregion
            }

            {
                #region Policy Template for Home

                List<PolicyProperty> homeProperties = new List<PolicyProperty>();

                PolicyProperty policyHolder = new PolicyProperty();
                policyHolder.FieldName = "Policy Holder";
                policyHolder.FieldType = PolicyTypeFieldType.String;
                policyHolder.PropertyType = PropertyType.PolicyTemplate;
                policyHolder.IsRequired = false;
                policyHolder.CreatedById = userSystem.Id;
                policyHolder.CreatedDate = DateTime.Now;
                homeProperties.Add(policyHolder);

                PolicyProperty propertyOccupancy = new PolicyProperty();
                propertyOccupancy.FieldName = "Property Occupancy";
                propertyOccupancy.FieldType = PolicyTypeFieldType.List;
                propertyOccupancy.ListDesription = "[\"Renting\",\"Ownership\"]";
                propertyOccupancy.PropertyType = PropertyType.PolicyTemplate;
                propertyOccupancy.IsRequired = false;
                propertyOccupancy.CreatedById = userSystem.Id;
                propertyOccupancy.CreatedDate = DateTime.Now;
                homeProperties.Add(propertyOccupancy);

                PolicyProperty propertyAddress = new PolicyProperty();
                propertyAddress.FieldName = "Property Address";
                propertyAddress.FieldType = PolicyTypeFieldType.String;
                propertyAddress.PropertyType = PropertyType.PolicyTemplate;
                propertyAddress.IsRequired = true;
                propertyAddress.IsTitle = true;
                propertyAddress.CreatedById = userSystem.Id;
                propertyAddress.CreatedDate = DateTime.Now;
                homeProperties.Add(propertyAddress);

                PolicyProperty propertyType = new PolicyProperty();
                propertyType.FieldName = "Property Type";
                propertyType.FieldType = PolicyTypeFieldType.List;
                propertyType.ListDesription = "[\"House\",\"Apartment\",\"Garage\"]";
                propertyType.PropertyType = PropertyType.PolicyTemplate;
                propertyType.IsRequired = false;
                propertyType.CreatedById = userSystem.Id;
                propertyType.CreatedDate = DateTime.Now;
                homeProperties.Add(propertyType);

                PolicyProperty yearBuilt = new PolicyProperty();
                yearBuilt.FieldName = "Year built";
                yearBuilt.FieldType = PolicyTypeFieldType.Int;
                yearBuilt.PropertyType = PropertyType.PolicyTemplate;
                yearBuilt.IsRequired = false;
                yearBuilt.CreatedById = userSystem.Id;
                yearBuilt.CreatedDate = DateTime.Now;
                homeProperties.Add(yearBuilt);

                PolicyProperty securityAlarm = new PolicyProperty();
                securityAlarm.FieldName = "Security Alarm";
                securityAlarm.FieldType = PolicyTypeFieldType.Bool;
                securityAlarm.PropertyType = PropertyType.PolicyTemplate;
                securityAlarm.IsRequired = false;
                securityAlarm.CreatedById = userSystem.Id;
                securityAlarm.CreatedDate = DateTime.Now;
                homeProperties.Add(securityAlarm);

                PolicyProperty wallMaterial = new PolicyProperty();
                wallMaterial.FieldName = "Wall Material";
                wallMaterial.FieldType = PolicyTypeFieldType.String;
                wallMaterial.PropertyType = PropertyType.PolicyTemplate;
                wallMaterial.IsRequired = false;
                wallMaterial.CreatedById = userSystem.Id;
                wallMaterial.CreatedDate = DateTime.Now;
                homeProperties.Add(wallMaterial);

                PolicyProperty roofMaterial = new PolicyProperty();
                roofMaterial.FieldName = "Roof Material";
                roofMaterial.FieldType = PolicyTypeFieldType.String;
                roofMaterial.PropertyType = PropertyType.PolicyTemplate;
                roofMaterial.IsRequired = false;
                roofMaterial.CreatedById = userSystem.Id;
                roofMaterial.CreatedDate = DateTime.Now;
                homeProperties.Add(roofMaterial);

                PolicyProperty isUsedForBusiness = new PolicyProperty();
                isUsedForBusiness.FieldName = "Used for Business";
                isUsedForBusiness.FieldType = PolicyTypeFieldType.Bool;
                isUsedForBusiness.PropertyType = PropertyType.PolicyTemplate;
                isUsedForBusiness.IsRequired = false;
                isUsedForBusiness.CreatedById = userSystem.Id;
                isUsedForBusiness.CreatedDate = DateTime.Now;
                homeProperties.Add(isUsedForBusiness);

                context.PolicyProperties.AddRange(homeProperties);
                context.SaveChanges();

                PolicyTemplate homePolicyTemplate = new PolicyTemplate();
                homePolicyTemplate.PolicyGroupId = groupProperty.Id;
                homePolicyTemplate.Name = "Home";
                homePolicyTemplate.CreatedById = userSystem.Id;
                homePolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(homePolicyTemplate);
                context.SaveChanges();

                foreach (var prop in homeProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = homePolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();
                #endregion
            }

            {
                #region Policy Template for Mortgage Protection
                List<PolicyProperty> homeProperties = new List<PolicyProperty>();

                PolicyProperty policyHolder = new PolicyProperty();
                policyHolder.FieldName = "Policy Holder";
                policyHolder.FieldType = PolicyTypeFieldType.String;
                policyHolder.PropertyType = PropertyType.PolicyTemplate;
                policyHolder.IsRequired = false;
                policyHolder.CreatedById = userSystem.Id;
                policyHolder.CreatedDate = DateTime.Now;
                homeProperties.Add(policyHolder);

                PolicyProperty propertyOccupancy = new PolicyProperty();
                propertyOccupancy.FieldName = "Property Occupancy";
                propertyOccupancy.FieldType = PolicyTypeFieldType.List;
                propertyOccupancy.ListDesription = "[\"Renting\",\"Ownership\"]";
                propertyOccupancy.PropertyType = PropertyType.PolicyTemplate;
                propertyOccupancy.IsRequired = false;
                propertyOccupancy.CreatedById = userSystem.Id;
                propertyOccupancy.CreatedDate = DateTime.Now;
                homeProperties.Add(propertyOccupancy);

                PolicyProperty propertyAddress = new PolicyProperty();
                propertyAddress.FieldName = "Property Address";
                propertyAddress.FieldType = PolicyTypeFieldType.String;
                propertyAddress.PropertyType = PropertyType.PolicyTemplate;
                propertyAddress.IsRequired = true;
                propertyAddress.IsTitle = true;
                propertyAddress.CreatedById = userSystem.Id;
                propertyAddress.CreatedDate = DateTime.Now;
                homeProperties.Add(propertyAddress);

                PolicyProperty propertyType = new PolicyProperty();
                propertyType.FieldName = "Property Type";
                propertyType.FieldType = PolicyTypeFieldType.List;
                propertyType.ListDesription = "[\"House\",\"Apartment\",\"Garage\"]";
                propertyType.PropertyType = PropertyType.PolicyTemplate;
                propertyType.IsRequired = false;
                propertyType.CreatedById = userSystem.Id;
                propertyType.CreatedDate = DateTime.Now;
                homeProperties.Add(propertyType);

                PolicyProperty yearBuilt = new PolicyProperty();
                yearBuilt.FieldName = "Year built";
                yearBuilt.FieldType = PolicyTypeFieldType.Int;
                yearBuilt.PropertyType = PropertyType.PolicyTemplate;
                yearBuilt.IsRequired = false;
                yearBuilt.CreatedById = userSystem.Id;
                yearBuilt.CreatedDate = DateTime.Now;
                homeProperties.Add(yearBuilt);

                PolicyProperty securityAlarm = new PolicyProperty();
                securityAlarm.FieldName = "Security Alarm";
                securityAlarm.FieldType = PolicyTypeFieldType.Bool;
                securityAlarm.PropertyType = PropertyType.PolicyTemplate;
                securityAlarm.IsRequired = false;
                securityAlarm.CreatedById = userSystem.Id;
                securityAlarm.CreatedDate = DateTime.Now;
                homeProperties.Add(securityAlarm);

                PolicyProperty wallMaterial = new PolicyProperty();
                wallMaterial.FieldName = "Wall Material";
                wallMaterial.FieldType = PolicyTypeFieldType.String;
                wallMaterial.PropertyType = PropertyType.PolicyTemplate;
                wallMaterial.IsRequired = false;
                wallMaterial.CreatedById = userSystem.Id;
                wallMaterial.CreatedDate = DateTime.Now;
                homeProperties.Add(wallMaterial);

                PolicyProperty roofMaterial = new PolicyProperty();
                roofMaterial.FieldName = "Roof Material";
                roofMaterial.FieldType = PolicyTypeFieldType.String;
                roofMaterial.PropertyType = PropertyType.PolicyTemplate;
                roofMaterial.IsRequired = false;
                roofMaterial.CreatedById = userSystem.Id;
                roofMaterial.CreatedDate = DateTime.Now;
                homeProperties.Add(roofMaterial);

                PolicyProperty isUsedForBusiness = new PolicyProperty();
                isUsedForBusiness.FieldName = "Used for Business";
                isUsedForBusiness.FieldType = PolicyTypeFieldType.Bool;
                isUsedForBusiness.PropertyType = PropertyType.PolicyTemplate;
                isUsedForBusiness.IsRequired = false;
                isUsedForBusiness.CreatedById = userSystem.Id;
                isUsedForBusiness.CreatedDate = DateTime.Now;
                homeProperties.Add(isUsedForBusiness);

                context.PolicyProperties.AddRange(homeProperties);
                context.SaveChanges();

                PolicyTemplate homePolicyTemplate = new PolicyTemplate();
                homePolicyTemplate.PolicyGroupId = groupProperty.Id;
                homePolicyTemplate.Name = "Mortgage protection";
                homePolicyTemplate.CreatedById = userSystem.Id;
                homePolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(homePolicyTemplate);
                context.SaveChanges();

                foreach (var prop in homeProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = homePolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();
                #endregion
            }

            {
                #region Policy Template for Business-Commertial

                List<PolicyProperty> bCommercialProperties = new List<PolicyProperty>();

                PolicyProperty businessName = new PolicyProperty();
                businessName.FieldName = "Name";
                businessName.FieldType = PolicyTypeFieldType.String;
                businessName.PropertyType = PropertyType.PolicyTemplate;
                businessName.IsRequired = true;
                businessName.IsTitle = true;
                businessName.CreatedById = userSystem.Id;
                businessName.CreatedDate = DateTime.Now;
                bCommercialProperties.Add(businessName);

                PolicyProperty boccupation = new PolicyProperty();
                boccupation.FieldName = "Occupation";
                boccupation.FieldType = PolicyTypeFieldType.String;
                boccupation.PropertyType = PropertyType.PolicyTemplate;
                boccupation.IsRequired = true;
                boccupation.IsSubtitle = true;
                boccupation.CreatedById = userSystem.Id;
                boccupation.CreatedDate = DateTime.Now;
                bCommercialProperties.Add(boccupation);

                PolicyProperty abn = new PolicyProperty();
                abn.FieldName = "Services provided";
                abn.FieldType = PolicyTypeFieldType.String;
                abn.PropertyType = PropertyType.PolicyTemplate;
                abn.IsRequired = false;
                abn.CreatedById = userSystem.Id;
                abn.CreatedDate = DateTime.Now;
                bCommercialProperties.Add(abn);

                context.PolicyProperties.AddRange(bCommercialProperties);


                PolicyTemplate bCommercialPolicyTemplate = new PolicyTemplate();
                bCommercialPolicyTemplate.PolicyGroupId = groupCommercial.Id;
                bCommercialPolicyTemplate.Name = "Commercial";
                bCommercialPolicyTemplate.CreatedById = userSystem.Id;
                bCommercialPolicyTemplate.CreatedDate = DateTime.Now;

                context.PolicyTemplates.Add(bCommercialPolicyTemplate);
                context.SaveChanges();

                foreach (var prop in bCommercialProperties)
                {
                    PolicyTemplatePolicyProperty policyTemplatePolicyProperty = new PolicyTemplatePolicyProperty();
                    policyTemplatePolicyProperty.PolicyPropertyId = prop.Id;
                    policyTemplatePolicyProperty.PolicyTemplateId = bCommercialPolicyTemplate.Id;
                    policyTemplatePolicyProperty.CreatedById = userSystem.Id;
                    policyTemplatePolicyProperty.CreatedDate = DateTime.Now;

                    context.PolicyTemplatePolicyProperties.Add(policyTemplatePolicyProperty);
                }
                context.SaveChanges();
                #endregion
            }

            #region Policy Groups

            PolicyType vehicleCar = new PolicyType();
            vehicleCar.PolicyGroupId = groupVehicle.Id;
            vehicleCar.PolicyTemplateId = carPolicyTemplate.Id;
            vehicleCar.PolicyTypeName = "Comprehensive";
            vehicleCar.CreatedById = userSystem.Id;
            vehicleCar.CreatedDate = DateTime.Now;
            vehicleCar.InsurerId = insurer1.Id;
            vehicleCar.Status = PolicyTypeStatus.Active;


            context.PolicyTypes.Add(vehicleCar);
            context.SaveChanges();

            PolicyProperty vehicleCarColor = new PolicyProperty();
            vehicleCarColor.FieldName = "Color";
            vehicleCarColor.FieldType = PolicyTypeFieldType.List;
            vehicleCarColor.ListDesription = "[\"red\",\"black\",\"white\",\"blue\",\"green\"]";
            vehicleCarColor.PropertyType = PropertyType.AdditionalToPolicyType;
            vehicleCarColor.CreatedById = userSystem.Id;
            vehicleCarColor.CreatedDate = DateTime.Now;

            PolicyProperty vehicleCarNumberOfKeys = new PolicyProperty();
            vehicleCarNumberOfKeys.FieldName = "Number Of Keys";
            vehicleCarNumberOfKeys.FieldType = PolicyTypeFieldType.Int;
            vehicleCarNumberOfKeys.PropertyType = PropertyType.AdditionalToPolicyType;
            vehicleCarNumberOfKeys.CreatedById = userSystem.Id;
            vehicleCarNumberOfKeys.CreatedDate = DateTime.Now;


            context.PolicyProperties.Add(vehicleCarColor);
            context.PolicyProperties.Add(vehicleCarNumberOfKeys);
            context.SaveChanges();


            PolicyTypePolicyProperty vehicleCarColorProp = new PolicyTypePolicyProperty();
            vehicleCarColorProp.PolicyPropertyId = vehicleCarColor.Id;
            vehicleCarColorProp.PolicyTypeId = vehicleCar.Id;
            vehicleCarColorProp.CreatedById = userSystem.Id;
            vehicleCarColorProp.CreatedDate = DateTime.Now;


            PolicyTypePolicyProperty vehicleCarNumberOfKeysProp = new PolicyTypePolicyProperty();
            vehicleCarNumberOfKeysProp.PolicyPropertyId = vehicleCarColor.Id;
            vehicleCarNumberOfKeysProp.PolicyTypeId = vehicleCar.Id;
            vehicleCarNumberOfKeysProp.CreatedById = userSystem.Id;
            vehicleCarNumberOfKeysProp.CreatedDate = DateTime.Now;


            context.PolicyTypePolicyProperties.Add(vehicleCarNumberOfKeysProp);
            #endregion

            #region Coverages

            Coverage c1 = new Coverage();
            c1.Title = "Collision";
            c1.Type = CoverageType.StandardCover;
            c1.Status = CoverageStatus.None;
            c1.InsurerId = insurer1.Id;
            c1.CreatedDate = DateTime.Now;
            c1.CreatedById = userSystem.Id;

            Coverage c2 = new Coverage();
            c2.Title = "Collision";
            c2.Type = CoverageType.StandardCover;
            c2.Status = CoverageStatus.None;
            c2.InsurerId = insurer1.Id;
            c2.CreatedDate = DateTime.Now;
            c2.CreatedById = userSystem.Id;

            context.Coverages.Add(c1);
            context.Coverages.Add(c2);

            context.SaveChanges();

            PolicyTypeCoverage pc2 = new PolicyTypeCoverage();
            pc2.CoverageId = c1.Id;
            pc2.CreatedById = userSystem.Id;
            pc2.CreatedDate = DateTime.Now;
            pc2.PolicyTypeId = vehicleCar.Id;


            context.PolicyTypeCoverage.Add(pc2);

            context.SaveChanges();
            #endregion

            context.SaveChanges();
        }
    }
}
