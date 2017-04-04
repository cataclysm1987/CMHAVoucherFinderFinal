namespace CMHAVoucherFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddistancefromsearch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "DistanceFromSearch", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "DistanceFromSearch");
        }
    }
}
