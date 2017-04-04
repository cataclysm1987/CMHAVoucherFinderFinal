namespace CMHAVoucherFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addlandlordinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FullName", c => c.String());
            AddColumn("dbo.AspNetUsers", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CompanyName");
            DropColumn("dbo.AspNetUsers", "FullName");
        }
    }
}
