using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace duelfighteronline.Models
{
    public class CharacterInfo
    {
        [Key]
        public int ID { get; set; }

        public string PlayerID { get; set; }
        [Required()]
        [Display(Name="Character Name")]
        public string CharacterName { get; set; }
        [Display(Name ="Character Class")]
        public CharacterClass CharacterClass { get; set; }
        public int Level { get; set; }
        [Display(Name = "Available Stat Points")]
        public int StatPointsAvailable { get; set; }
        [Display(Name = "Experience")]
        public int CurrentExperience { get; set; }
        [Display(Name = "Experience to Level")]
        public int MaxExperienceForLevel { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        [Display(Name = "Critical Strike Chance")]
        public float CritChance { get; set; }
        [Display(Name = "Dodge Chance")]
        public float DodgeChance { get; set; }
        [Range(1, 1000)]
        public int Strength { get; set; }
        [Range(1, 1000)]
        public int Dexterity { get; set; }
        [Range(1, 1000)]
        public int Vitality { get; set; }
        [Range(1, 1000)]
        public int Luck { get; set; }
        [Display(Name = "Duels Available")]
        public int DuelsAvailable { get; set; }
        [Display(Name = "Duels Won")]
        public int DuelWins { get; set; }
        [Display(Name = "Duels Lost")]
        public int DuelLosses { get; set; }
        public ICollection<DuelHistory> DuelHistory { get; set; }

        public int CalculateHealth(CharacterInfo characterInfo)
        {
            if (characterInfo.CharacterClass.ToString() == "Jester")
            {
                characterInfo.Health = (int)(50 + (characterInfo.Vitality * 5) * 1.2);
            } else
            {
                characterInfo.Health = (int)(50 + (characterInfo.Vitality * 5));
            }
            return characterInfo.Health;
        }

        public int CalculateDamage(CharacterInfo characterInfo)
        {
            int damage = 10;
            if (characterInfo.CharacterClass.ToString() == "Knight")
            {
                damage += (int)(characterInfo.Strength * 1.5);
            }
            else if (characterInfo.CharacterClass.ToString() == "Assassin")
            {
                damage += (int)(characterInfo.Dexterity * 0.8);
            }
            else if (characterInfo.CharacterClass.ToString() == "Warrior")
            {
                damage += (int)((characterInfo.Strength) + (characterInfo.Dexterity));
            }
            else if (characterInfo.CharacterClass.ToString() == "Jester")
            {
                damage += (int)((characterInfo.Strength * .8) + (characterInfo.Dexterity * .8));                                                                                                                                                                               
            }
            return damage;
        }

        public float CalculateCritChance(CharacterInfo characterInfo)
        {
            float critChance = 5;
            if (characterInfo.CharacterClass.ToString() == "Jester")
            {
                critChance += (int)(characterInfo.Luck * .4);
            } else
            {
                critChance += (int)(characterInfo.Luck * .2);
            }
            return critChance;
        }

        public float CalculateDodgeChance(CharacterInfo characterInfo)
        {
            float dodgeChance = 3;
            dodgeChance += (int)(characterInfo.Dexterity * .3);
            return dodgeChance;
        }
    }

    


    public enum CharacterClass { Knight, Assassin, Warrior, Jester }
}