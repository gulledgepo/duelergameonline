namespace duelfighteronline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CharacterInfo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CharacterName = c.String(nullable: false),
                        CharacterClass = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        StatPointsAvailable = c.Int(nullable: false),
                        CurrentExperience = c.Int(nullable: false),
                        MaxExperienceForLevel = c.Int(nullable: false),
                        Health = c.Int(nullable: false),
                        Strength = c.Int(nullable: false),
                        Dexterity = c.Int(nullable: false),
                        Vitality = c.Int(nullable: false),
                        Luck = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CharacterInfo");
        }
    }
}
