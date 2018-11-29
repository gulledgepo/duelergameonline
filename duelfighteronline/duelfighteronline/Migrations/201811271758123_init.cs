namespace duelfighteronline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CharacterInfo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PlayerID = c.String(),
                        CharacterName = c.String(nullable: false),
                        CharacterClass = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        StatPointsAvailable = c.Int(nullable: false),
                        CurrentExperience = c.Int(nullable: false),
                        MaxExperienceForLevel = c.Int(nullable: false),
                        Health = c.Int(nullable: false),
                        Damage = c.Int(nullable: false),
                        CritChance = c.Single(nullable: false),
                        DodgeChance = c.Single(nullable: false),
                        Strength = c.Int(nullable: false),
                        Dexterity = c.Int(nullable: false),
                        Vitality = c.Int(nullable: false),
                        Luck = c.Int(nullable: false),
                        DuelsAvailable = c.Int(nullable: false),
                        DuelWins = c.Int(nullable: false),
                        DuelLosses = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DuelHistory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CharacterInfoID = c.Int(nullable: false),
                        Result = c.String(),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CharacterInfo", t => t.CharacterInfoID, cascadeDelete: true)
                .Index(t => t.CharacterInfoID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DuelHistory", "CharacterInfoID", "dbo.CharacterInfo");
            DropIndex("dbo.DuelHistory", new[] { "CharacterInfoID" });
            DropTable("dbo.DuelHistory");
            DropTable("dbo.CharacterInfo");
        }
    }
}
