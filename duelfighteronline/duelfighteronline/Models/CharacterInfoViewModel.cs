using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace duelfighteronline.Models
{
    public class CharacterInfoViewModel
    {

        public CharacterInfo characterInfo { get; set; }
        public int Damage { get; set; }
        [Display(Name = "Critical Strike Chance")]
        public int CritChance { get; set; }
        [Display(Name = "Dodge Chance")]
        public int DodgeChance { get; set; }
        public string ExperienceDisplay { get; set; }


    }
}