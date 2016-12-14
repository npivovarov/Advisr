namespace Advisr.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TitleForHealth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PolicyTemplates", "Title", c => c.String());
            AddColumn("dbo.PolicyTemplates", "Subtitle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PolicyTemplates", "Subtitle");
            DropColumn("dbo.PolicyTemplates", "Title");
        }
    }
}
