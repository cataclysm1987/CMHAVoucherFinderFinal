namespace CMHAVoucherFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intitialmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavoriteProperties",
                c => new
                    {
                        FavoritePropertyId = c.Int(nullable: false, identity: true),
                        PropertyId = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.FavoritePropertyId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserType = c.Int(nullable: false),
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
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
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
                "dbo.Properties",
                c => new
                    {
                        PropertyId = c.Int(nullable: false, identity: true),
                        PropertyName = c.String(nullable: false),
                        StreetAddress = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        ZipCode = c.Int(nullable: false),
                        DateAvailable = c.DateTime(),
                        Price = c.Double(nullable: false),
                        Deposit = c.Double(nullable: false),
                        Beds = c.Int(nullable: false),
                        Baths = c.Int(nullable: false),
                        SquareFeet = c.Int(nullable: false),
                        YearBuilt = c.Int(nullable: false),
                        PropertyDescription = c.String(nullable: false),
                        UserId = c.String(maxLength: 128),
                        CeilingFans = c.Int(nullable: false),
                        Furnished = c.Int(nullable: false),
                        Fireplace = c.Int(nullable: false),
                        CablePaid = c.Int(nullable: false),
                        HeatStyle = c.Int(nullable: false),
                        DishWasher = c.Int(nullable: false),
                        Stove = c.Int(nullable: false),
                        GarbageDisposal = c.Int(nullable: false),
                        Refrigerator = c.Int(nullable: false),
                        Microwave = c.Int(nullable: false),
                        SwimmingPool = c.Int(nullable: false),
                        GatedCommunity = c.Int(nullable: false),
                        LawnCareIncluded = c.Int(nullable: false),
                        ParkingType = c.Int(nullable: false),
                        FencedYard = c.Int(nullable: false),
                        PatioPorch = c.Int(nullable: false),
                        IsSmokingAllowed = c.Int(nullable: false),
                        LotSize = c.Int(nullable: false),
                        PestControl = c.Int(nullable: false),
                        TenantPaysElectric = c.Int(nullable: false),
                        TenantPaysWater = c.Int(nullable: false),
                        TenantPaysSewer = c.Int(nullable: false),
                        TenantPaysHeat = c.Int(nullable: false),
                        ElectricStatus = c.String(),
                        WaterStatus = c.String(),
                        SewerStatus = c.String(),
                        HeatStatus = c.String(),
                        IsParkingClose = c.Int(nullable: false),
                        NoStepEntry = c.Int(nullable: false),
                        RampedEntry = c.Int(nullable: false),
                        Doorway32OrWider = c.Int(nullable: false),
                        AccessiblePathInHome = c.Int(nullable: false),
                        AutomaticEntryDoor = c.Int(nullable: false),
                        LeverStyleDoorHandles = c.Int(nullable: false),
                        SingleLevelOrFirstFloow = c.Int(nullable: false),
                        InsideSteps = c.Int(nullable: false),
                        OutsideSteps = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PropertyImages",
                c => new
                    {
                        PropertyImageId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FilePath = c.String(),
                        ThumbFilePath = c.String(),
                        Property_PropertyId = c.Int(),
                    })
                .PrimaryKey(t => t.PropertyImageId)
                .ForeignKey("dbo.Properties", t => t.Property_PropertyId)
                .Index(t => t.Property_PropertyId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Properties", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PropertyImages", "Property_PropertyId", "dbo.Properties");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.FavoriteProperties", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.PropertyImages", new[] { "Property_PropertyId" });
            DropIndex("dbo.Properties", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.FavoriteProperties", new[] { "ApplicationUserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.PropertyImages");
            DropTable("dbo.Properties");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.FavoriteProperties");
        }
    }
}
