namespace CrawlNews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDB1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UrlSource = c.String(),
                        Title = c.String(),
                        Image = c.String(),
                        Description = c.String(),
                        Content = c.String(),
                        Category = c.String(),
                        CreatedAt = c.Long(nullable: false),
                        SourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sourses", t => t.SourseId, cascadeDelete: true)
                .Index(t => t.SourseId);
            
            CreateTable(
                "dbo.Sourses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        SelectorSubUrl = c.String(),
                        SubUrl = c.String(),
                        SelectorTitle = c.String(),
                        SelectorImage = c.String(),
                        SelectorDescription = c.String(),
                        SelectorContent = c.String(),
                        CategoryId = c.Int(nullable: false),
                        ArticleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Articles", "SourseId", "dbo.Sourses");
            DropForeignKey("dbo.Sourses", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Sourses", new[] { "CategoryId" });
            DropIndex("dbo.Articles", new[] { "SourseId" });
            DropTable("dbo.Categories");
            DropTable("dbo.Sourses");
            DropTable("dbo.Articles");
        }
    }
}
