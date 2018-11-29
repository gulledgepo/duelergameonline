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
        public static CharacterInfo winner;
        public static CharacterInfo loser;
        public static string AttackerDuelLog = "";
        public static string DefenderDuelLog = "";

        public static DuelViewModel Execute(DuelViewModel characterInfo, CharacterClassContext db)
        {
            //CharacterInfo duelingPlayer = new CharacterInfo();
            //duelingPlayer = characterInfo.CharacterInfo;
            

            //CharacterInfo playerToDuel = new CharacterInfo();
            //playerToDuel.ID = -1;
            int totalPlayers = characterInfo.PlayersTotal;
            //Duel the amount of times requested
            for (int i = 0; i < characterInfo.DuelsRequested; i++)
            {
                int idToFind = -1;
                //As long as the player info is null, we want to repeat and select a new one, it would be null because the random number generated belongs to a deleted account
                while (idToFind <= 0)
                {
                    idToFind = CheckIfPlayerExists(totalPlayers, db, characterInfo.CharacterInfo);
                }
                //playerToDuel = db.CharacterInfo.Find(idToFind);
                characterInfo.DuelTarget = db.CharacterInfo.Find(idToFind);

                bool initiatingPlayerAttacking = true;
                //characterInfo is a DuelViewModel which already holds the attacking and defending player, so we pass just that in and break it down in the CarryOutDuel function
                int attackingPlayerHealth = characterInfo.CharacterInfo.Health;
                int defendingPlayerHealth = characterInfo.DuelTarget.Health;
                AttackerDuelLog = "";
                DefenderDuelLog = "";

                CarryOutDuel(characterInfo, initiatingPlayerAttacking, attackingPlayerHealth, defendingPlayerHealth, db, i);




            }
            return characterInfo;
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

        public static void CarryOutDuel(DuelViewModel characterInfo, bool initiatingPlayerAttacking, int attackingPlayerHealth, int defendingPlayerHealth, CharacterClassContext db, int duelCounter)
        {
            bool isCrit = false;
            bool isDodge = false;
            int damageDone = 0;
            int temp = 0;
            string tempLog = "";
            //Defining which character is which, this changes based on the bool InitiatingPlayerAttacking
            CharacterInfo defendingPlayer;
            CharacterInfo attackingPlayer;
            if (initiatingPlayerAttacking == true)
            {
                defendingPlayer = characterInfo.DuelTarget;
                attackingPlayer = characterInfo.CharacterInfo;
            } else
            {
                defendingPlayer = characterInfo.CharacterInfo;
                attackingPlayer = characterInfo.DuelTarget;
            }

            
            isCrit = CheckIfAttackIsCrit(attackingPlayer);
            //if it is a crit we do the damage and update log accordingly, critical attacks cannot be dodged
            if (isCrit)
            {
                damageDone = (int)(attackingPlayer.Damage * 1.8);
                defendingPlayerHealth = defendingPlayerHealth - damageDone;
                AttackerDuelLog += "You crit " + defendingPlayer.CharacterName + " for " + damageDone + ".\n";
                DefenderDuelLog += attackingPlayer.CharacterName + " crit you for " + damageDone + ".\n";
            } else
            {
                //regular attacks can be dodged, so we must check the defender's dodge chance
                isDodge = CheckIfAttackIsDodged(defendingPlayer);

                if (isDodge)
                {
                    //the Warrior still takes full damage if he dodges, but he retaliates for his full damage as well, everyone else dodges
                    if (defendingPlayer.CharacterClass.ToString() == "Warrior")
                    {
                        //Attacker's hit to defender
                        damageDone = (attackingPlayer.Damage);
                        defendingPlayerHealth = defendingPlayerHealth - damageDone;
                        AttackerDuelLog += "You hit " + defendingPlayer.CharacterName + " for " + damageDone + ".\n";
                        DefenderDuelLog += attackingPlayer.CharacterName + " hit you for " + damageDone + ".\n";
                        //Defender's retaliate
                        damageDone = (defendingPlayer.Damage);
                        attackingPlayerHealth = attackingPlayerHealth - damageDone;
                        AttackerDuelLog += defendingPlayer.CharacterName + " counter attacked you for " + damageDone + ".\n";
                        DefenderDuelLog += "You counter attacked " + attackingPlayer.CharacterName + "for " + damageDone + ".\n";
                    }
                    else
                    {
                        AttackerDuelLog += defendingPlayer.CharacterName + " dodged your attack.\n";
                        DefenderDuelLog += "You dodged " + attackingPlayer.CharacterName + "'s attack.\n";
                    }
                }
                else
                {
                    damageDone = (attackingPlayer.Damage);
                    defendingPlayerHealth = defendingPlayerHealth - damageDone;
                    AttackerDuelLog += "You hit " + defendingPlayer.CharacterName + " for " + damageDone + ".\n";
                    DefenderDuelLog += attackingPlayer.CharacterName + " hit you for " + damageDone + ".\n";
                }
            }
            //As the attacks happen, we check to see if either player has died, if they have, a winner is declared, if not we repeat until there is a winner.
            if (defendingPlayerHealth < 0)
            {
                defendingPlayer.DuelLosses++;
                attackingPlayer.DuelWins++;
                DefenderDuelLog += "You lost the duel.\n";
                AttackerDuelLog += "You won the duel.\n";
                CalculateExperienceResults(attackingPlayer, defendingPlayer, characterInfo);
                
                //update DuelHistory
                db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = attackingPlayer.ID, Initiator = characterInfo.CharacterInfo.CharacterName, Result = "Won", Details = AttackerDuelLog });
                db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = defendingPlayer.ID, Initiator = characterInfo.CharacterInfo.CharacterName, Result = "Lost", Details = DefenderDuelLog });
                db.SaveChanges();
            } else if (attackingPlayerHealth < 0)
            {
                defendingPlayer.DuelWins++;
                attackingPlayer.DuelLosses++;
                DefenderDuelLog += "You won the duel.\n";
                AttackerDuelLog += "You lost the duel.\n";
                CalculateExperienceResults(defendingPlayer, attackingPlayer, characterInfo);
                //update DuelHistory
                db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = attackingPlayer.ID, Initiator = characterInfo.CharacterInfo.CharacterName, Result = "Lost", Details = AttackerDuelLog });
                db.Set<DuelHistory>().Add(new DuelHistory { CharacterInfoID = defendingPlayer.ID, Initiator = characterInfo.CharacterInfo.CharacterName, Result = "Won", Details = DefenderDuelLog });
                db.SaveChanges();
            } else
            {
                //Change current attacker to defender for next iteration
                initiatingPlayerAttacking = !initiatingPlayerAttacking;
                //Swap HP
                temp = attackingPlayerHealth;
                attackingPlayerHealth = defendingPlayerHealth;
                defendingPlayerHealth = temp;
                tempLog = AttackerDuelLog;
                AttackerDuelLog = DefenderDuelLog;
                DefenderDuelLog = tempLog;
                CarryOutDuel(characterInfo, initiatingPlayerAttacking, attackingPlayerHealth, defendingPlayerHealth, db, duelCounter);
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

        public static void CalculateExperienceResults(CharacterInfo winner, CharacterInfo loser, DuelViewModel characterInfo)
        {
            int experienceToAward = 0;
            //If initiating character won
            if (winner.ID == characterInfo.CharacterInfo.ID)
            {
                experienceToAward = CalculateExperienceForWinningInitiator(winner, loser);
                AttackerDuelLog += "You gained " + experienceToAward + " experience!for winning as attacker\n";
                characterInfo.CharacterInfo.CurrentExperience += experienceToAward;

                experienceToAward = CalculateExperienceForLosingDefender(winner, loser);
                characterInfo.DuelTarget.CurrentExperience += experienceToAward;
                DefenderDuelLog += "You gained " + experienceToAward + " experience!for losing as defender\n";
            }
            else
            {
                experienceToAward = CalculateExperienceForWinningDefender(winner, loser);                
                AttackerDuelLog += ("You gained " + experienceToAward + " experience!\nfor winning as defender");
                characterInfo.DuelTarget.CurrentExperience += experienceToAward;


                experienceToAward = CalculateExperienceForLosingInitiator(winner, loser);
                DefenderDuelLog += ("You gained " + experienceToAward + " experience!\nfor losing as attacker");                
                characterInfo.CharacterInfo.CurrentExperience += experienceToAward;

            }

        }

        public static int CalculateExperienceForWinningInitiator(CharacterInfo winner, CharacterInfo loser)
        {
            int experienceToAward = 0;
            int levelDifference = (winner.Level - loser.Level);
            //If the level difference is within 3, full xp is awarded to an attacking winner
            if (levelDifference >= -3 || levelDifference <= 3)
            {
                experienceToAward = 25 + (5 * winner.Level);
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
            return experienceToAward;
        }

        public static int CalculateExperienceForLosingInitiator(CharacterInfo winner, CharacterInfo loser)
        {
            int experienceToAward = 0;
            int levelDifference = (winner.Level - loser.Level);
            //If the level difference is within 3, half XP is awarded to initiator for a defender winning
            if (levelDifference >= -3 || levelDifference <= 3)
            {
                experienceToAward = (25 + (5 * loser.Level)) / 2;
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
            return experienceToAward;
        }

        public static int CalculateExperienceForLosingDefender(CharacterInfo winner, CharacterInfo loser)
        {
            int experienceToAward = 0;
            int levelDifference = (winner.Level - loser.Level);
            //If the level difference is within 3, half xp is awarded to defender for losing
            if (levelDifference >= -3 || levelDifference <= 3)
            {
                experienceToAward = (25 + (5 * loser.Level)) / 2;
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
            return experienceToAward;
        }

        public static int CalculateExperienceForWinningDefender(CharacterInfo winner, CharacterInfo loser)
        {
            int experienceToAward = 0;
            int levelDifference = (winner.Level - loser.Level);
            //If the level difference is within 3, a third xp is awarded to defender for losing
            if (levelDifference >= -3 || levelDifference <= 3)
            {
                experienceToAward = (25 + (5 * loser.Level)) / 3;
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
            return experienceToAward;
        }

        //public static string GenerateDuelID(CharacterInfo defendingPlayer, CharacterInfo attackingPlayer, int duelCounter)
        //{
        //    string duelID = "";
        //    duelID += attackingPlayer.CharacterName.First();
        //    duelID += duelCounter;
        //    duelID += attackingPlayer.ID;            
        //    duelID += defendingPlayer.CharacterName.First();
        //    duelID += DateTime.Now.Millisecond;
        //    return duelID;
        //}

    }


}