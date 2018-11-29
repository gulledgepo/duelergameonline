using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using duelfighteronline.Context;
using duelfighteronline.GameLogic;
using duelfighteronline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace duelfighteronline.GameLogic
{
    public class CalculateDuelResultCommand
    {
        public static Random rand = new Random(DateTime.Now.Millisecond);
        public static int winnerID;
        public static string attackerDuelLog = "";
        public static string defenderDuelLog = "";

        public static DuelViewModel Execute(DuelViewModel duelCharacterInfo, CharacterClassContext db)
        {
            int totalPlayers = duelCharacterInfo.PlayersTotal;
            //Duel the amount of times requested
            for (int i = 0; i < duelCharacterInfo.DuelsRequested; i++)
            {
                int idToFind = -1;
                //As long as the player info is null, we want to repeat and select a new one, it would be null because the random number generated belongs to a deleted account
                while (idToFind <= 0)
                {
                    idToFind = CheckIfPlayerExists(totalPlayers, db, duelCharacterInfo.DuelInitiator);
                }

                duelCharacterInfo.DuelTarget = db.CharacterInfo.Find(idToFind);

                bool initiatingPlayerAttacking = true;
                //characterInfo is a DuelViewModel which already holds the attacking and defending player, so we pass just that in and break it down in the CarryOutDuel function
                int attackingPlayerHealth = duelCharacterInfo.DuelInitiator.Health;
                int defendingPlayerHealth = duelCharacterInfo.DuelTarget.Health;
                attackerDuelLog = "";
                defenderDuelLog = "";
                winnerID = 0;
                CarryOutDuel(duelCharacterInfo, initiatingPlayerAttacking, attackingPlayerHealth, defendingPlayerHealth, db, i);
                //Award Duel Initiator EXP
                int experienceToAward = CalculateDuelInitiatorExperience(duelCharacterInfo);
                attackerDuelLog += "You gained " + experienceToAward + " experience!\n";
                duelCharacterInfo.DuelInitiator.CurrentExperience += experienceToAward;
                //Award Duel Target EXP
                experienceToAward = CalculateDuelTargetExperience(duelCharacterInfo);
                defenderDuelLog += "You gained " + experienceToAward + " experience!\n";
                duelCharacterInfo.DuelTarget.CurrentExperience += experienceToAward;
                bool levelUp = false;
                levelUp = CheckIfLevelUp(duelCharacterInfo.DuelInitiator);
                if (levelUp)
                {
                    attackerDuelLog += "You leveled up!\nYou gained 5 stat points.";
                }
                levelUp = CheckIfLevelUp(duelCharacterInfo.DuelTarget);
                if (levelUp)
                {
                    defenderDuelLog += "You leveled up!\nYou gained 5 stat points.";
                }

                //update DuelHistory
                if (winnerID == duelCharacterInfo.DuelInitiator.ID)
                {
                    db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = duelCharacterInfo.DuelInitiator.ID, Initiator = duelCharacterInfo.DuelInitiator.CharacterName, Result = "Won", Details = attackerDuelLog });
                    db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = duelCharacterInfo.DuelTarget.ID, Initiator = duelCharacterInfo.DuelInitiator.CharacterName, Result = "Lost", Details = defenderDuelLog });
                    db.SaveChanges();
                }
                else
                {
                    db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = duelCharacterInfo.DuelInitiator.ID, Initiator = duelCharacterInfo.DuelInitiator.CharacterName, Result = "Lost", Details = attackerDuelLog });
                    db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = duelCharacterInfo.DuelTarget.ID, Initiator = duelCharacterInfo.DuelInitiator.CharacterName, Result = "Won", Details = defenderDuelLog });
                    db.SaveChanges();
                }




            }
            return duelCharacterInfo;
        }

        public static int CheckIfPlayerExists(int totalPlayers, CharacterClassContext db, CharacterInfo duelInitiate)
        {
            CharacterInfo duelTarget = new CharacterInfo();
            int random = rand.Next(1, totalPlayers + 1);
            //Sets random to a number between 1 and the max # of players (inclusive), then looks up that entry in CharacterInfo
            duelTarget = db.CharacterInfo.Find(random);
            //If its null, it doesn't exist, return -1 so we try again
            if (duelTarget == null)
            {
                return -1;
                //If the ID of the looked up is the same as the one who requested the duel, then they are owned by same player, which is not allowed, return -1 to try again
            } else if (duelTarget.PlayerID == duelInitiate.PlayerID)
            {
                return -1;
                //success
            } else
            {
                return duelTarget.ID;
            }                      
        }

        public static void CarryOutDuel(DuelViewModel duelCharacterInfo, bool initiatingPlayerAttacking, int attackingPlayerHealth, int defendingPlayerHealth, CharacterClassContext db, int duelCounter)
        {
            bool isCrit = false;
            bool isDodge = false;
            int damageDone = 0;

            //If (initiatingPlayerAttacking) then it is the duelCharacterInfo.DuelInitiator
            if (initiatingPlayerAttacking)
            {
                isCrit = CheckIfAttackIsCrit(duelCharacterInfo.DuelInitiator);
                //if it is a crit we do the damage and update log accordingly, critical attacks cannot be dodged
                if (isCrit)
                {
                    damageDone = (int)(duelCharacterInfo.DuelInitiator.Damage * 1.8);
                    defendingPlayerHealth = defendingPlayerHealth - damageDone;
                    attackerDuelLog += "You crit " + duelCharacterInfo.DuelTarget.CharacterName + " for " + damageDone + ".\n";
                    defenderDuelLog += duelCharacterInfo.DuelInitiator.CharacterName + " crit you for " + damageDone + ".\n";
                }
                else
                {
                    //regular attacks can be dodged, so we must check the defender's dodge chance
                    isDodge = CheckIfAttackIsDodged(duelCharacterInfo.DuelTarget);

                    if (isDodge)
                    {
                        //the Warrior still takes full damage if he dodges, but he retaliates for his full damage as well, everyone else dodges
                        if (duelCharacterInfo.DuelTarget.CharacterClass.ToString() == "Warrior")
                        {
                            //Attacker's hit to defender
                            damageDone = (duelCharacterInfo.DuelInitiator.Damage);
                            defendingPlayerHealth = defendingPlayerHealth - damageDone;
                            attackerDuelLog += "You hit " + duelCharacterInfo.DuelTarget.CharacterName + " for " + damageDone + ".\n";
                            defenderDuelLog += duelCharacterInfo.DuelInitiator.CharacterName + " hit you for " + damageDone + ".\n";
                            //Defender's retaliate
                            damageDone = (duelCharacterInfo.DuelTarget.Damage);
                            attackingPlayerHealth = attackingPlayerHealth - damageDone;
                            attackerDuelLog += duelCharacterInfo.DuelTarget.CharacterName + " counter attacked you for " + damageDone + ".\n";
                            defenderDuelLog += "You counter attacked " + duelCharacterInfo.DuelInitiator.CharacterName + "for " + damageDone + ".\n";
                        }
                        else
                        {
                            attackerDuelLog += duelCharacterInfo.DuelTarget.CharacterName + " dodged your attack.\n";
                            defenderDuelLog += "You dodged " + duelCharacterInfo.DuelInitiator.CharacterName + "'s attack.\n";
                        }
                    }
                    else
                    {
                        damageDone = (duelCharacterInfo.DuelInitiator.Damage);
                        defendingPlayerHealth = defendingPlayerHealth - damageDone;
                        attackerDuelLog += "You hit " + duelCharacterInfo.DuelTarget.CharacterName + " for " + damageDone + ".\n";
                        defenderDuelLog += duelCharacterInfo.DuelInitiator.CharacterName + " hit you for " + damageDone + ".\n";
                    }
                }


                //If !initiatingPlayerAttacking then the DuelTarget/Defender is attacking
            } else
            {
                isCrit = CheckIfAttackIsCrit(duelCharacterInfo.DuelTarget);
                //if it is a crit we do the damage and update log accordingly, critical attacks cannot be dodged
                if (isCrit)
                {
                    damageDone = (int)(duelCharacterInfo.DuelTarget.Damage * 1.8);
                    attackingPlayerHealth = attackingPlayerHealth - damageDone;
                    defenderDuelLog += "You crit " + duelCharacterInfo.DuelInitiator.CharacterName + " for " + damageDone + ".\n";
                    attackerDuelLog += duelCharacterInfo.DuelTarget.CharacterName + " crit you for " + damageDone + ".\n";
                }
                else
                {
                    //regular attacks can be dodged, so we must check the defender's dodge chance
                    isDodge = CheckIfAttackIsDodged(duelCharacterInfo.DuelInitiator);

                    if (isDodge)
                    {
                        //the Warrior still takes full damage if he dodges, but he retaliates for his full damage as well, everyone else dodges
                        if (duelCharacterInfo.DuelInitiator.CharacterClass.ToString() == "Warrior")
                        {
                            //Defenders's hit to attacker
                            damageDone = (duelCharacterInfo.DuelTarget.Damage);
                            attackingPlayerHealth = attackingPlayerHealth - damageDone;
                            defenderDuelLog += "You hit " + duelCharacterInfo.DuelInitiator.CharacterName + " for " + damageDone + ".\n";
                            attackerDuelLog += duelCharacterInfo.DuelTarget.CharacterName + " hit you for " + damageDone + ".\n";
                            //Attacker's retaliate
                            damageDone = (duelCharacterInfo.DuelInitiator.Damage);
                            defendingPlayerHealth = defendingPlayerHealth - damageDone;
                            defenderDuelLog += duelCharacterInfo.DuelInitiator.CharacterName + " counter attacked you for " + damageDone + ".\n";
                            attackerDuelLog += "You counter attacked " + duelCharacterInfo.DuelTarget.CharacterName + "for " + damageDone + ".\n";
                        }
                        else
                        {
                            defenderDuelLog += duelCharacterInfo.DuelInitiator.CharacterName + " dodged your attack.\n";
                            attackerDuelLog += "You dodged " + duelCharacterInfo.DuelTarget.CharacterName + "'s attack.\n";
                        }
                    }
                    else
                    {
                        damageDone = (duelCharacterInfo.DuelTarget.Damage);
                        attackingPlayerHealth = attackingPlayerHealth - damageDone;
                        defenderDuelLog += "You hit " + duelCharacterInfo.DuelInitiator.CharacterName + " for " + damageDone + ".\n";
                        attackerDuelLog += duelCharacterInfo.DuelTarget.CharacterName + " hit you for " + damageDone + ".\n";
                    }
                }
            }
            //As the attacks happen, we check to see if either player has died, if they have, a winner is declared, if not we repeat until there is a winner.
            if (defendingPlayerHealth < 0)
            {
                duelCharacterInfo.DuelTarget.DuelLosses++;
                duelCharacterInfo.DuelInitiator.DuelWins++;
                defenderDuelLog += "You lost the duel.\n";
                attackerDuelLog += "You won the duel.\n";
                winnerID = duelCharacterInfo.DuelInitiator.ID;

            } else if (attackingPlayerHealth < 0)
            {
                duelCharacterInfo.DuelTarget.DuelWins++;
                duelCharacterInfo.DuelInitiator.DuelLosses++;
                defenderDuelLog += "You won the duel.\n";
                attackerDuelLog += "You lost the duel.\n";
                winnerID = duelCharacterInfo.DuelTarget.ID;
                
            } else
            {
                //Change current attacker to defender for next iteration
                initiatingPlayerAttacking = !initiatingPlayerAttacking;
                CarryOutDuel(duelCharacterInfo, initiatingPlayerAttacking, attackingPlayerHealth, defendingPlayerHealth, db, duelCounter);
            }
            
        }

        public static bool CheckIfAttackIsCrit(CharacterInfo attacker)
        {
            bool isCrit;
            
            int random = rand.Next(0, 101);
            if (attacker.CritChance > random)
            {
                isCrit = true;
            } else
            {
                isCrit = false;
            }
            return isCrit;
        }

        public static bool CheckIfAttackIsDodged(CharacterInfo defender)
        {
            bool isDodged;

            int random = rand.Next(0, 101);
            if (defender.DodgeChance > random)
            {
                isDodged = true;
            } else
            {
                isDodged = false;
            }
            return isDodged;
        }

        public static int CalculateDuelInitiatorExperience(DuelViewModel duelCharacterInfo)
        {
            int experienceToAward = 0;
            int levelDifference = (duelCharacterInfo.DuelInitiator.Level - duelCharacterInfo.DuelTarget.Level);
            //ExperienceGainForWinning
            if (winnerID == duelCharacterInfo.DuelInitiator.ID)
            {
                //If the level difference is within 3, full xp is awarded to an attacking winner
                if (levelDifference >= -3 || levelDifference <= 3)
                {
                    experienceToAward = 25 + (5 * duelCharacterInfo.DuelInitiator.Level);
                }
                //If an attacker is greater than 4 levels, there is a penalty
                else if (levelDifference >= 4)
                {
                    experienceToAward = (25 / levelDifference) + 5;
                }
                //If the attacker is underleveled, we give them bonus xp to compensate for them being underlevelled and winning
                else if (levelDifference <= -4)
                {
                    experienceToAward = 25 + (Math.Abs(levelDifference) * 5);
                }
            }
            //ExperienceGainForLosing
            else
            {
                //If the level difference is within 3, half XP is awarded to initiator for a defender winning
                if (levelDifference >= -3 || levelDifference <= 3)
                {
                    experienceToAward = (25 + (5 * duelCharacterInfo.DuelInitiator.Level)) / 2;
                }
                //If an initiator is greater than 4 levels and lost they get less XP
                else if (levelDifference >= 4)
                {
                    experienceToAward = (25 / levelDifference) + 5;
                }
                //If the attacker is underleveled, we give them bonus xp to compensate for them being underlevelled and losing
                else if (levelDifference <= -4)
                {
                    experienceToAward = 25 + (Math.Abs(levelDifference) * 5);
                }
            }

            return experienceToAward;
        }

        public static int CalculateDuelTargetExperience(DuelViewModel duelCharacterInfo)
        {
            int experienceToAward = 0;
            int levelDifference = (duelCharacterInfo.DuelInitiator.Level - duelCharacterInfo.DuelTarget.Level);
            //ExperienceGainForWinning
            if (winnerID == duelCharacterInfo.DuelTarget.ID)
            {
                //If the level difference is within 3, a third xp is awarded to defender for losing
                if (levelDifference >= -3 || levelDifference <= 3)
                {
                    experienceToAward = (25 + (5 * duelCharacterInfo.DuelTarget.Level)) / 3;
                }
                //If an initiator is greater than 4 levels and won, defender gets extra XP
                else if (levelDifference >= 4)
                {
                    experienceToAward = (25 + (levelDifference * 5)) / 2;
                }
                //If the attacker is underleveled, and the defender loses, they receive less because they should have won
                else if (levelDifference <= -4)
                {

                    experienceToAward = (25 / Math.Abs(levelDifference)) + 5;
                }
            }
            //ExperienceGainForLosing
            else
            {
                //If the level difference is within 3, half xp is awarded to defender for losing
                if (levelDifference >= -3 || levelDifference <= 3)
                {
                    experienceToAward = (25 + (5 * duelCharacterInfo.DuelTarget.Level)) / 2;
                }
                //If an initiator is greater than 4 levels and lost, defender gets extra XP
                else if (levelDifference >= 4)
                {
                    experienceToAward = (25 + (levelDifference * 7)) / 2;
                }
                //If the attacker is underleveled, and the defender wins, they receive less because they were supposed to win
                else if (levelDifference <= -4)
                {

                    experienceToAward = (25 / Math.Abs(levelDifference)) + 5;
                }
            }

            return experienceToAward;
        }

        public static bool CheckIfLevelUp(CharacterInfo experienceCharacterInfo)
        {
            if (experienceCharacterInfo.CurrentExperience > experienceCharacterInfo.MaxExperienceForLevel)
            {
                experienceCharacterInfo.Level++;
                experienceCharacterInfo.StatPointsAvailable += 5;
                experienceCharacterInfo.CurrentExperience -= experienceCharacterInfo.MaxExperienceForLevel;
                experienceCharacterInfo.MaxExperienceForLevel = 50 + (50 * experienceCharacterInfo.Level);
                return true;   
            } else
            {
                return false;
            }
        }
        

    }


}
