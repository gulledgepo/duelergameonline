namespace duelfighteronline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DuelHistory", "DateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DuelHistory", "DateTime", c => c.DateTime(nullable: false));
        }
    }
}
