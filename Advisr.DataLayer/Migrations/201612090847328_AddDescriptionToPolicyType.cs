namespace Advisr.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToPolicyType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PolicyTypes", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PolicyTypes", "Description");
        }
    }
}
