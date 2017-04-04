namespace CMHAVoucherFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class basicdetailsproperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "FiftyFivePlusOnly", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "FiftyFivePlusOnly");
            DropColumn("dbo.Properties", "PropertyType");
        }
    }
}
