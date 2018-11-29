namespace duelfighteronline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DuelHistory", "Initiator", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DuelHistory", "Initiator");
        }
    }
}
