namespace SecretSanta.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Deleteusersesionmodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserSessions", "UserId", "dbo.Users");
            DropIndex("dbo.UserSessions", new[] { "UserId" });
            DropTable("dbo.UserSessions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserSessions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Authtoken = c.String(nullable: false),
                        ExpiresOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.UserSessions", "UserId");
            AddForeignKey("dbo.UserSessions", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
