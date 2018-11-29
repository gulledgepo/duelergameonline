using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace duelfighteronline.Models
{
    public class DuelViewModel
    {

        public CharacterInfo CharacterInfo { get; set; }
        public CharacterInfo DuelTarget { get; set; }
        [Display(Name = "Win Percent")]
        public float WinPercent { get; set; }
        [Display(Name = "How many duels would you like to fight in?")]
        public int DuelsRequested { get; set; }
        public int PlayersTotal { get; set; }

        public float CalculateWinPercent(CharacterInfo characterInfo)
        {
            if (characterInfo.DuelLosses == 0)
            {
                return 100;
            } else if (characterInfo.DuelWins == 0)  
            {
                return 0;
            }
            else
            {
                return (characterInfo.DuelWins / characterInfo.DuelLosses);
            }

        }

    }
}