namespace CMHAVoucherFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addlatlong : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Properties", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "Longitude");
            DropColumn("dbo.Properties", "Latitude");
        }
    }
}
