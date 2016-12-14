namespace Advisr.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PolicyDataChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdditionalPolicyProperties", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.Home_P", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Home_P", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Home_P", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Home_P", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.AdditionalPolicyProperties", "PolicyType_Id", "dbo.PolicyTypes");
            DropForeignKey("dbo.PolicyTypeFields", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypeFields", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypeFields", "PolicyTypeId", "dbo.PolicyTypes");
            DropForeignKey("dbo.Life_P", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Life_P", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Life_P", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.Vehicle_P", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Vehicle_P", "MakeId", "dbo.VehicleMakes");
            DropForeignKey("dbo.Vehicle_P", "ModelId", "dbo.VehicleModels");
            DropForeignKey("dbo.Vehicle_P", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Vehicle_P", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.AdditionalPolicyProperties", "PolicyTypeFieldId", "dbo.PolicyTypeFields");
            DropIndex("dbo.AdditionalPolicyProperties", new[] { "PolicyId" });
            DropIndex("dbo.AdditionalPolicyProperties", new[] { "PolicyTypeFieldId" });
            DropIndex("dbo.AdditionalPolicyProperties", new[] { "PolicyType_Id" });
            DropIndex("dbo.Home_P", new[] { "PolicyId" });
            DropIndex("dbo.Home_P", new[] { "AddressId" });
            DropIndex("dbo.Home_P", new[] { "CreatedById" });
            DropIndex("dbo.Home_P", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyTypeFields", new[] { "PolicyTypeId" });
            DropIndex("dbo.PolicyTypeFields", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTypeFields", new[] { "ModifiedById" });
            DropIndex("dbo.Life_P", new[] { "PolicyId" });
            DropIndex("dbo.Life_P", new[] { "CreatedById" });
            DropIndex("dbo.Life_P", new[] { "ModifiedById" });
            DropIndex("dbo.Vehicle_P", new[] { "PolicyId" });
            DropIndex("dbo.Vehicle_P", new[] { "MakeId" });
            DropIndex("dbo.Vehicle_P", new[] { "ModelId" });
            DropIndex("dbo.Vehicle_P", new[] { "CreatedById" });
            DropIndex("dbo.Vehicle_P", new[] { "ModifiedById" });
            CreateTable(
                "dbo.PolicyGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PolicyTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyGroupId = c.Int(nullable: false),
                        Name = c.String(),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.PolicyGroups", t => t.PolicyGroupId, cascadeDelete: true)
                .Index(t => t.PolicyGroupId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyTemplatePolicyProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyTemplateId = c.Int(nullable: false),
                        PolicyPropertyId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.PolicyProperties", t => t.PolicyPropertyId, cascadeDelete: true)
                .ForeignKey("dbo.PolicyTemplates", t => t.PolicyTemplateId, cascadeDelete: true)
                .Index(t => t.PolicyTemplateId)
                .Index(t => t.PolicyPropertyId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldName = c.String(),
                        FieldType = c.Int(nullable: false),
                        OrderIndex = c.Int(nullable: false),
                        PropertyType = c.Int(nullable: false),
                        IsRequired = c.Boolean(nullable: false),
                        IsTitle = c.Boolean(nullable: false),
                        IsSubtitle = c.Boolean(nullable: false),
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
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyTypePolicyProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyTypeId = c.Int(nullable: false),
                        PolicyPropertyId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedById = c.String(maxLength: 128),
                        ModifiedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.PolicyProperties", t => t.PolicyPropertyId, cascadeDelete: true)
                .ForeignKey("dbo.PolicyTypes", t => t.PolicyTypeId, cascadeDelete: true)
                .Index(t => t.PolicyTypeId)
                .Index(t => t.PolicyPropertyId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.PolicyPolicyProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        PolicyPropertyId = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Policies", t => t.PolicyId, cascadeDelete: true)
                .ForeignKey("dbo.PolicyProperties", t => t.PolicyPropertyId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.PolicyPropertyId);
            
            AddColumn("dbo.PolicyTypes", "PolicyTemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.PolicyTypes", "PolicyGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.PolicyTypes", "PolicyTemplateId");
            CreateIndex("dbo.PolicyTypes", "PolicyGroupId");
            AddForeignKey("dbo.PolicyTypes", "PolicyGroupId", "dbo.PolicyGroups", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PolicyTypes", "PolicyTemplateId", "dbo.PolicyTemplates", "Id", cascadeDelete: false);
            DropColumn("dbo.PolicyTypes", "GroupName");
            DropColumn("dbo.PolicyTypes", "PolicyGroupType");
            DropTable("dbo.AdditionalPolicyProperties");
            DropTable("dbo.Home_P");
            DropTable("dbo.PolicyTypeFields");
            DropTable("dbo.Life_P");
            DropTable("dbo.Vehicle_P");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PolicyTypes", "PolicyGroupType", c => c.Int(nullable: false));
            AddColumn("dbo.PolicyTypes", "GroupName", c => c.String());
            DropForeignKey("dbo.PolicyPolicyProperties", "PolicyPropertyId", "dbo.PolicyProperties");
            DropForeignKey("dbo.PolicyPolicyProperties", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.PolicyTypePolicyProperties", "PolicyTypeId", "dbo.PolicyTypes");
            DropForeignKey("dbo.PolicyTypePolicyProperties", "PolicyPropertyId", "dbo.PolicyProperties");
            DropForeignKey("dbo.PolicyTypePolicyProperties", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypePolicyProperties", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypes", "PolicyTemplateId", "dbo.PolicyTemplates");
            DropForeignKey("dbo.PolicyTemplatePolicyProperties", "PolicyTemplateId", "dbo.PolicyTemplates");
            DropForeignKey("dbo.PolicyTemplatePolicyProperties", "PolicyPropertyId", "dbo.PolicyProperties");
            DropForeignKey("dbo.PolicyProperties", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyProperties", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTemplatePolicyProperties", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTemplatePolicyProperties", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTemplates", "PolicyGroupId", "dbo.PolicyGroups");
            DropForeignKey("dbo.PolicyTemplates", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTemplates", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.PolicyTypes", "PolicyGroupId", "dbo.PolicyGroups");
            DropIndex("dbo.PolicyPolicyProperties", new[] { "PolicyPropertyId" });
            DropIndex("dbo.PolicyPolicyProperties", new[] { "PolicyId" });
            DropIndex("dbo.PolicyTypePolicyProperties", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyTypePolicyProperties", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTypePolicyProperties", new[] { "PolicyPropertyId" });
            DropIndex("dbo.PolicyTypePolicyProperties", new[] { "PolicyTypeId" });
            DropIndex("dbo.PolicyProperties", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyProperties", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTemplatePolicyProperties", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyTemplatePolicyProperties", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTemplatePolicyProperties", new[] { "PolicyPropertyId" });
            DropIndex("dbo.PolicyTemplatePolicyProperties", new[] { "PolicyTemplateId" });
            DropIndex("dbo.PolicyTemplates", new[] { "ModifiedById" });
            DropIndex("dbo.PolicyTemplates", new[] { "CreatedById" });
            DropIndex("dbo.PolicyTemplates", new[] { "PolicyGroupId" });
            DropIndex("dbo.PolicyTypes", new[] { "PolicyGroupId" });
            DropIndex("dbo.PolicyTypes", new[] { "PolicyTemplateId" });
            DropColumn("dbo.PolicyTypes", "PolicyGroupId");
            DropColumn("dbo.PolicyTypes", "PolicyTemplateId");
            DropTable("dbo.PolicyPolicyProperties");
            DropTable("dbo.PolicyTypePolicyProperties");
            DropTable("dbo.PolicyProperties");
            DropTable("dbo.PolicyTemplatePolicyProperties");
            DropTable("dbo.PolicyTemplates");
            DropTable("dbo.PolicyGroups");
            CreateIndex("dbo.Vehicle_P", "ModifiedById");
            CreateIndex("dbo.Vehicle_P", "CreatedById");
            CreateIndex("dbo.Vehicle_P", "ModelId");
            CreateIndex("dbo.Vehicle_P", "MakeId");
            CreateIndex("dbo.Vehicle_P", "PolicyId");
            CreateIndex("dbo.Life_P", "ModifiedById");
            CreateIndex("dbo.Life_P", "CreatedById");
            CreateIndex("dbo.Life_P", "PolicyId");
            CreateIndex("dbo.PolicyTypeFields", "ModifiedById");
            CreateIndex("dbo.PolicyTypeFields", "CreatedById");
            CreateIndex("dbo.PolicyTypeFields", "PolicyTypeId");
            CreateIndex("dbo.Home_P", "ModifiedById");
            CreateIndex("dbo.Home_P", "CreatedById");
            CreateIndex("dbo.Home_P", "AddressId");
            CreateIndex("dbo.Home_P", "PolicyId");
            CreateIndex("dbo.AdditionalPolicyProperties", "PolicyType_Id");
            CreateIndex("dbo.AdditionalPolicyProperties", "PolicyTypeFieldId");
            CreateIndex("dbo.AdditionalPolicyProperties", "PolicyId");
            AddForeignKey("dbo.AdditionalPolicyProperties", "PolicyTypeFieldId", "dbo.PolicyTypeFields", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Vehicle_P", "PolicyId", "dbo.Policies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Vehicle_P", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Vehicle_P", "ModelId", "dbo.VehicleModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Vehicle_P", "MakeId", "dbo.VehicleMakes", "Id");
            AddForeignKey("dbo.Vehicle_P", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Life_P", "PolicyId", "dbo.Policies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Life_P", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Life_P", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PolicyTypeFields", "PolicyTypeId", "dbo.PolicyTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PolicyTypeFields", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.PolicyTypeFields", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AdditionalPolicyProperties", "PolicyType_Id", "dbo.PolicyTypes", "Id");
            AddForeignKey("dbo.Home_P", "PolicyId", "dbo.Policies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Home_P", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Home_P", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Home_P", "AddressId", "dbo.Addresses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AdditionalPolicyProperties", "PolicyId", "dbo.Policies", "Id", cascadeDelete: true);
        }
    }
}
