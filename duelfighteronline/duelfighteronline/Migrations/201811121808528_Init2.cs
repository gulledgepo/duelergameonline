namespace duelfighteronline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CharacterInfo", "DuelsAvailable", c => c.Int(nullable: false));
            AddColumn("dbo.CharacterInfo", "DuelWins", c => c.Int(nullable: false));
            AddColumn("dbo.CharacterInfo", "DuelLosses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CharacterInfo", "DuelLosses");
            DropColumn("dbo.CharacterInfo", "DuelWins");
            DropColumn("dbo.CharacterInfo", "DuelsAvailable");
        }
    }
}
