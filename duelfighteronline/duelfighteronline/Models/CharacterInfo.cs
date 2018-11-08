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

        [Required()]
        [Display(Name="Character Name")]
        public string CharacterName { get; set; }
        [Display(Name ="Character Class")]
        public CharacterClass CharacterClass { get; set; }
        public int Level { get; set; }
        [Display(Name = "Available Stat Points")]
        [Range(0, 0, ErrorMessage = "All attribute points must be distributed before a character can be created.")]
        public int StatPointsAvailable { get; set; }
        [Display(Name = "Experience")]
        public int CurrentExperience { get; set; }
        [Display(Name = "Experience to Level")]
        public int MaxExperienceForLevel { get; set; }
        public int Health { get; set; }
        [Range(1, 1000)]
        public int Strength { get; set; }
        [Range(1, 1000)]
        public int Dexterity { get; set; }
        [Range(1, 1000)]
        public int Vitality { get; set; }
        [Range(1, 1000)]
        public int Luck { get; set; }
    }

    public enum CharacterClass { Knight, Assassin, Warrior, Jester }
}