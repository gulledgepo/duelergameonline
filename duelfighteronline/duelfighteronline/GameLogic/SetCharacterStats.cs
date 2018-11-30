using duelfighteronline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace duelfighteronline.GameLogic
{
    public class SetCharacterStats
    {

        public static CharacterInfo SetInitialCharacterStats(CharacterInfo characterInfo)
        {
            characterInfo.Level = 1;
            characterInfo.CurrentExperience = 0;
            characterInfo.MaxExperienceForLevel = 50;
            characterInfo.StatPointsAvailable = 30;
            characterInfo.Health = 50;
            characterInfo.Strength = 1;
            characterInfo.Vitality = 1;
            characterInfo.Dexterity = 1;
            characterInfo.Luck = 1;
            return characterInfo;
        }

    }
}