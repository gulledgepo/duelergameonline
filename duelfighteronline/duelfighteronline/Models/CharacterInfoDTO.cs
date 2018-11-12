using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace duelfighteronline.Models
{
    public class CharacterInfoDTO
    {


        public string CharacterName { get; set; }
        public string CharacterClass { get; set; }
        public int Level { get; set; }
        public int StatPointsAvailable { get; set; }
        public int CurrentExperience { get; set; }
        public int MaxExperienceForLevel { get; set; }
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Vitality { get; set; }
        public int Luck { get; set; }

    }
}