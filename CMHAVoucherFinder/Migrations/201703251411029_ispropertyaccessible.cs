namespace CMHAVoucherFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ispropertyaccessible : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "IsPropertyAccessible", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "IsPropertyAccessible");
        }
    }
}
