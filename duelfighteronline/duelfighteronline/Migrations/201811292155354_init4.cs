namespace duelfighteronline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DuelHistory", "Target", c => c.String());
            AddColumn("dbo.DuelHistory", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DuelHistory", "DateTime");
            DropColumn("dbo.DuelHistory", "Target");
        }
    }
}
