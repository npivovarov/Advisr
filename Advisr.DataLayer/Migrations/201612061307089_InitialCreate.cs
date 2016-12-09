namespace Advisr.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdditionalPolicyProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        PolicyTypeFieldId = c.Int(nullable: false),
                        Value = c.String(),
                        PolicyType_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .ForeignKey("dbo.PolicyTypes", t => t.PolicyType_Id)
                .ForeignKey("dbo.PolicyTypeFields", t => t.PolicyTypeFieldId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.PolicyTypeFieldId)
                .Index(t => t.PolicyType_Id);
            
            CreateTable(
                "dbo.Policies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyTypeId = c.Int(),
                        InsurerId = c.Int(),
                        Title = c.String(),
                        SubTitle = c.String(),
                        PolicyNumber = c.String(),
                        PrePolicyType = c.String(),
                        PolicyEffectiveDate = c.DateTime(),
                        PolicyPremium = c.Decimal(precision: 18, scale: 2),
                        PolicyPaymentFrequency = c.Int(nullable: false),
                        PolicyPaymentAmount = c.Decimal(precision: 18, scale: 2),
                        PolicyExcess = c.Decimal(precision: 18, scale: 2),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        ProcessedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                        ProcessedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.PolicyTypes", t => t.PolicyTypeId)
                .ForeignKey("dbo.Insurers", t => t.InsurerId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ProcessedById)
                .Index(t => t.PolicyTypeId)
                .Index(t => t.InsurerId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById)
                .Index(t => t.ProcessedById);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(maxLength: 65),
                        LastName = c.String(maxLength: 65),
                        AvatarFileId = c.Guid(),
                        HasTempPassword = c.Boolean(nullable: false),
                        CultureName = c.String(),
                        Status = c.Int(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(),
                        LockedByAdmin = c.Boolean(nullable: false),
                        LockedByAdminId = c.String(),
                        AutopilotContactId = c.String(),
                        AutopilotData = c.String(),
                        AutopilotTrack = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.AvatarFileId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .Index(t => t.AvatarFileId)
                .Index(t => t.CreatedById)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FileName = c.String(),
                        FileSize = c.Long(nullable: false),
                        Description = c.String(),
                        ContentType = c.String(),
                        LocationPath = c.String(),
                        IsTemp = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedById = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Hidden = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UserPolicies",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        PolicyId = c.Int(nullable: false),
                        IsPolicyOwner = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.PolicyId })
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.UserId)
                .Index(t => t.PolicyId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Home_P",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        AddressId = c.Int(nullable: false),
                        BuildDate = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.AddressId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address_1 = c.String(),
                        Address_2 = c.String(),
                        Country = c.String(),
                        City = c.String(),
                        Street = c.String(),
                        State = c.String(),
                        PostCode = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Insurers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        URL = c.String(),
                        Phone = c.String(),
                        PhoneOverseas = c.String(),
                        Email = c.String(),
                        AddressId = c.Int(),
                        LogoFileId = c.Guid(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                        Color = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Files", t => t.LogoFileId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .Index(t => t.AddressId)
                .Index(t => t.LogoFileId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InsurerId = c.Int(nullable: false),
                        GroupName = c.String(),
                        PolicyTypeName = c.String(),
                        PolicyGroupType = c.Int(nullable: false),
                        Status = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Insurers", t => t.InsurerId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .Index(t => t.InsurerId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyTypeFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyTypeId = c.Int(nullable: false),
                        FieldName = c.String(),
                        FieldType = c.Int(nullable: false),
                        DefaultValue = c.String(),
                        ListDesription = c.String(),
                        Status = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.PolicyTypes", t => t.PolicyTypeId, cascadeDelete: true)
                .Index(t => t.PolicyTypeId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Life_P",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        Medication = c.String(),
                        MedicationCondition = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyCoverages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        CoverageId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Excess = c.Decimal(precision: 18, scale: 2),
                        MaxPayAmount = c.Decimal(precision: 18, scale: 2),
                        Limit = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Coverages", t => t.CoverageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.CoverageId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Coverages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InsurerId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Insurers", t => t.InsurerId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .Index(t => t.InsurerId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        FileId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.FileId);
            
            CreateTable(
                "dbo.Vehicle_P",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        MakeId = c.Int(nullable: false),
                        ModelId = c.Int(nullable: false),
                        Colour = c.String(),
                        RegistredDriverName = c.String(),
                        RegistrationNumber = c.String(),
                        IsCommercial = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.VehicleMakes", t => t.MakeId)
                .ForeignKey("dbo.VehicleModels", t => t.ModelId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.MakeId)
                .Index(t => t.ModelId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.VehicleMakes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MakeName = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VehicleModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleMakeId = c.Int(nullable: false),
                        ModelName = c.String(),
                        Status = c.Int(nullable: false),
                        VehicleMake_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleMakes", t => t.VehicleMakeId)
                .ForeignKey("dbo.VehicleMakes", t => t.VehicleMake_Id)
                .Index(t => t.VehicleMakeId)
                .Index(t => t.VehicleMake_Id);
            
            CreateTable(
                "dbo.AutopilotErrorBuffers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AutopilotContactId = c.String(),
                        RequestUri = c.String(),
                        RequestData = c.String(),
                        OperationType = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerLogs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                        CustomerId = c.Guid(nullable: false),
                        UserId = c.String(),
                        ModifiedReason = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        UserId = c.String(maxLength: 128),
                        AddressId = c.Int(nullable: false),
                        ContactPhone = c.String(),
                        HomePhone = c.String(),
                        DateOfBirth = c.DateTime(),
                        MaritalStatus = c.Int(),
                        Gender = c.Int(),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                        ModifiedReason = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AddressId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyTypeCoverages",
                c => new
                    {
                        PolicyTypeId = c.Int(nullable: false),
                        CoverageId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.PolicyTypeId, t.CoverageId })
                .ForeignKey("dbo.Coverages", t => t.CoverageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.PolicyTypes", t => t.PolicyTypeId)
                .Index(t => t.PolicyTypeId)
                .Index(t => t.CoverageId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.UserNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipientId = c.String(maxLength: 128),
                        SubjectTitleFirst = c.String(),
                        SubjectTitleSecond = c.String(),
                        Title = c.String(),
                        Body = c.String(),
                        Status = c.Int(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        TargetObjectType = c.Int(nullable: false),
                        TargetObjectId = c.Int(),
                        TargetUrl = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ReadDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.RecipientId)
                .Index(t => t.RecipientId)
                .Index(t => t.CreatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserNotifications", "RecipientId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserNotifications", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypeCoverages", "PolicyTypeId", "dbo.PolicyTypes");
            DropForeignKey("dbo.PolicyTypeCoverages", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypeCoverages", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypeCoverages", "CoverageId", "dbo.Coverages");
            DropForeignKey("dbo.CustomerDetails", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomerDetails", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomerDetails", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomerDetails", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.AdditionalPolicyProperties", "PolicyTypeFieldId", "dbo.PolicyTypeFields");
            DropForeignKey("dbo.Vehicle_P", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.Vehicle_P", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Vehicle_P", "ModelId", "dbo.VehicleModels");
            DropForeignKey("dbo.Vehicle_P", "MakeId", "dbo.VehicleMakes");
            DropForeignKey("dbo.VehicleModels", "VehicleMake_Id", "dbo.VehicleMakes");
            DropForeignKey("dbo.VehicleModels", "VehicleMakeId", "dbo.VehicleMakes");
            DropForeignKey("dbo.Vehicle_P", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Policies", "ProcessedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyFiles", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.PolicyFiles", "FileId", "dbo.Files");
            DropForeignKey("dbo.PolicyCoverages", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.PolicyCoverages", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyCoverages", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyCoverages", "CoverageId", "dbo.Coverages");
            DropForeignKey("dbo.Coverages", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Coverages", "InsurerId", "dbo.Insurers");
            DropForeignKey("dbo.Coverages", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Policies", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Life_P", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.Life_P", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Life_P", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Policies", "InsurerId", "dbo.Insurers");
            DropForeignKey("dbo.PolicyTypeFields", "PolicyTypeId", "dbo.PolicyTypes");
            DropForeignKey("dbo.PolicyTypeFields", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypeFields", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Policies", "PolicyTypeId", "dbo.PolicyTypes");
            DropForeignKey("dbo.PolicyTypes", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypes", "InsurerId", "dbo.Insurers");
            DropForeignKey("dbo.PolicyTypes", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AdditionalPolicyProperties", "PolicyType_Id", "dbo.PolicyTypes");
            DropForeignKey("dbo.Insurers", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Insurers", "LogoFileId", "dbo.Files");
            DropForeignKey("dbo.Insurers", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Insurers", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Home_P", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.Home_P", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Home_P", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Home_P", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Addresses", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Policies", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPolicies", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPolicies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPolicies", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.UserPolicies", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPolicies", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "AvatarFileId", "dbo.Files");
            DropForeignKey("dbo.AdditionalPolicyProperties", "PolicyId", "dbo.Policies");
            DropIndex("dbo.UserNotifications", new[] { "CreatedById" });
            DropIndex("dbo.UserNotifications", new[] { "RecipientId" });
            DropIndex("dbo.PolicyTypeCoverages", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyTypeCoverages", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTypeCoverages", new[] { "CoverageId" });
            DropIndex("dbo.PolicyTypeCoverages", new[] { "PolicyTypeId" });
            DropIndex("dbo.CustomerDetails", new[] { "ModifiedById" });
            DropIndex("dbo.CustomerDetails", new[] { "CreatedById" });
            DropIndex("dbo.CustomerDetails", new[] { "AddressId" });
            DropIndex("dbo.CustomerDetails", new[] { "UserId" });
            DropIndex("dbo.VehicleModels", new[] { "VehicleMake_Id" });
            DropIndex("dbo.VehicleModels", new[] { "VehicleMakeId" });
            DropIndex("dbo.Vehicle_P", new[] { "ModifiedById" });
            DropIndex("dbo.Vehicle_P", new[] { "CreatedById" });
            DropIndex("dbo.Vehicle_P", new[] { "ModelId" });
            DropIndex("dbo.Vehicle_P", new[] { "MakeId" });
            DropIndex("dbo.Vehicle_P", new[] { "PolicyId" });
            DropIndex("dbo.PolicyFiles", new[] { "FileId" });
            DropIndex("dbo.PolicyFiles", new[] { "PolicyId" });
            DropIndex("dbo.Coverages", new[] { "ModifiedById" });
            DropIndex("dbo.Coverages", new[] { "CreatedById" });
            DropIndex("dbo.Coverages", new[] { "InsurerId" });
            DropIndex("dbo.PolicyCoverages", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyCoverages", new[] { "CreatedById" });
            DropIndex("dbo.PolicyCoverages", new[] { "CoverageId" });
            DropIndex("dbo.PolicyCoverages", new[] { "PolicyId" });
            DropIndex("dbo.Life_P", new[] { "ModifiedById" });
            DropIndex("dbo.Life_P", new[] { "CreatedById" });
            DropIndex("dbo.Life_P", new[] { "PolicyId" });
            DropIndex("dbo.PolicyTypeFields", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyTypeFields", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTypeFields", new[] { "PolicyTypeId" });
            DropIndex("dbo.PolicyTypes", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyTypes", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTypes", new[] { "InsurerId" });
            DropIndex("dbo.Insurers", new[] { "ModifiedById" });
            DropIndex("dbo.Insurers", new[] { "CreatedById" });
            DropIndex("dbo.Insurers", new[] { "LogoFileId" });
            DropIndex("dbo.Insurers", new[] { "AddressId" });
            DropIndex("dbo.Addresses", new[] { "ModifiedById" });
            DropIndex("dbo.Addresses", new[] { "CreatedById" });
            DropIndex("dbo.Home_P", new[] { "ModifiedById" });
            DropIndex("dbo.Home_P", new[] { "CreatedById" });
            DropIndex("dbo.Home_P", new[] { "AddressId" });
            DropIndex("dbo.Home_P", new[] { "PolicyId" });
            DropIndex("dbo.UserPolicies", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.UserPolicies", new[] { "ModifiedById" });
            DropIndex("dbo.UserPolicies", new[] { "CreatedById" });
            DropIndex("dbo.UserPolicies", new[] { "PolicyId" });
            DropIndex("dbo.UserPolicies", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "CreatedById" });
            DropIndex("dbo.AspNetUsers", new[] { "AvatarFileId" });
            DropIndex("dbo.Policies", new[] { "ProcessedById" });
            DropIndex("dbo.Policies", new[] { "ModifiedById" });
            DropIndex("dbo.Policies", new[] { "CreatedById" });
            DropIndex("dbo.Policies", new[] { "InsurerId" });
            DropIndex("dbo.Policies", new[] { "PolicyTypeId" });
            DropIndex("dbo.AdditionalPolicyProperties", new[] { "PolicyType_Id" });
            DropIndex("dbo.AdditionalPolicyProperties", new[] { "PolicyTypeFieldId" });
            DropIndex("dbo.AdditionalPolicyProperties", new[] { "PolicyId" });
            DropTable("dbo.UserNotifications");
            DropTable("dbo.PolicyTypeCoverages");
            DropTable("dbo.CustomerDetails");
            DropTable("dbo.CustomerLogs");
            DropTable("dbo.AutopilotErrorBuffers");
            DropTable("dbo.VehicleModels");
            DropTable("dbo.VehicleMakes");
            DropTable("dbo.Vehicle_P");
            DropTable("dbo.PolicyFiles");
            DropTable("dbo.Coverages");
            DropTable("dbo.PolicyCoverages");
            DropTable("dbo.Life_P");
            DropTable("dbo.PolicyTypeFields");
            DropTable("dbo.PolicyTypes");
            DropTable("dbo.Insurers");
            DropTable("dbo.Addresses");
            DropTable("dbo.Home_P");
            DropTable("dbo.UserPolicies");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Files");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Policies");
            DropTable("dbo.AdditionalPolicyProperties");
        }
    }
}
