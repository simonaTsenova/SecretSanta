namespace SecretSanta.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addotherentitiesandconfigurations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 15),
                        AdminId = c.String(maxLength: 128),
                        hasLinkingProcessStarted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AdminId)
                .Index(t => t.Name, unique: true, name: "IX_GroupName")
                .Index(t => t.AdminId);
            
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SentDate = c.DateTime(nullable: false),
                        GroupId = c.Guid(nullable: false),
                        ReceiverId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.ReceiverId)
                .Index(t => t.GroupId)
                .Index(t => t.ReceiverId);
            
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        GroupId = c.Guid(nullable: false),
                        SenderId = c.String(maxLength: 128),
                        ReceiverId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.ReceiverId)
                .ForeignKey("dbo.Users", t => t.SenderId)
                .Index(t => t.GroupId)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId);
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Group_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Group_Id })
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.Group_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Group_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Links", "SenderId", "dbo.Users");
            DropForeignKey("dbo.Links", "ReceiverId", "dbo.Users");
            DropForeignKey("dbo.Links", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "AdminId", "dbo.Users");
            DropForeignKey("dbo.Invitations", "ReceiverId", "dbo.Users");
            DropForeignKey("dbo.Invitations", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.UserGroups", "Group_Id", "dbo.Groups");
            DropForeignKey("dbo.UserGroups", "User_Id", "dbo.Users");
            DropIndex("dbo.UserGroups", new[] { "Group_Id" });
            DropIndex("dbo.UserGroups", new[] { "User_Id" });
            DropIndex("dbo.Links", new[] { "ReceiverId" });
            DropIndex("dbo.Links", new[] { "SenderId" });
            DropIndex("dbo.Links", new[] { "GroupId" });
            DropIndex("dbo.Invitations", new[] { "ReceiverId" });
            DropIndex("dbo.Invitations", new[] { "GroupId" });
            DropIndex("dbo.Groups", new[] { "AdminId" });
            DropIndex("dbo.Groups", "IX_GroupName");
            DropTable("dbo.UserGroups");
            DropTable("dbo.Links");
            DropTable("dbo.Invitations");
            DropTable("dbo.Groups");
        }
    }
}
