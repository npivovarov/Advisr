namespace Advisr.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToInsurer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Insurers", "Description", c => c.String());
            DropColumn("dbo.Policies", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Policies", "Description", c => c.String());
            DropColumn("dbo.Insurers", "Description");
        }
    }
}
