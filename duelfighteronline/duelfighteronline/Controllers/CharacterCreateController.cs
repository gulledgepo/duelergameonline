using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using duelfighteronline.Context;
using duelfighteronline.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using duelfighteronline.GameLogic;
namespace duelfighteronline.Controllers
{
    public class CharacterCreateController : Controller
    {
        private CharacterClassContext db = new CharacterClassContext();
        //These two are necessary to access the UserID outside of account controller
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public CharacterCreateController()
        {
            //Again, necessary to access user ID
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: CharacterCreate/Create
        public ActionResult Create()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            //Pass in the initial stats for each character for creation.
            CharacterInfo characterInfo = new CharacterInfo();
            characterInfo = SetCharacterStats.SetInitialCharacterStats(characterInfo);
            //This passes the userID into the character creation where it will be stored to associate that character with that user.
            characterInfo.PlayerID = user.Id;
            return View(characterInfo);
        }

        // POST: CharacterCreate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CharacterName,CharacterClass,Level,StatPointsAvailable,CurrentExperience,MaxExperienceForLevel,Health,Strength,Dexterity,Vitality,Luck,PlayerID")] CharacterInfo characterInfo)
        {
            if (ModelState.IsValid)
            {
                //Check if the name is taken, if it is ask them to retry, reloading page with their stats distributed already.                
                var characterNameCheck = db.CharacterInfo.FirstOrDefault(x => x.CharacterName == characterInfo.CharacterName);
                //If characterNameCheck is null then there was no match for an existing name, so we continue to check if everything else is correct for creation
                if (characterNameCheck == null)
                {
                    //Every character upon creation must spend their 30 stat points, giving them a total of 34. If their total stats
                    //is greater than 34, we know they tampered with something and redirect them to create.
                    if ((characterInfo.Strength + characterInfo.Dexterity + characterInfo.Vitality + characterInfo.Luck) == 34)
                    {
                        characterInfo.Health = characterInfo.CalculateHealth(characterInfo);
                        characterInfo.Damage = characterInfo.CalculateDamage(characterInfo);
                        characterInfo.CritChance = characterInfo.CalculateCritChance(characterInfo);
                        characterInfo.DodgeChance = characterInfo.CalculateDodgeChance(characterInfo);
                        db.CharacterInfo.Add(characterInfo);
                        db.SaveChanges();
                        return RedirectToAction("Index", "CharacterInfo");
                    }
                    else
                    {
                        TempData["message"] = "Invalid stat points.";
                        return RedirectToAction("Create");
                    }
                    //Might be redundant, but another check to see if the names are the same and send back an error
                } else if (characterNameCheck.CharacterName == characterInfo.CharacterName)
                    {
                        TempData["message"] = "Character name already taken.";
                        return View(characterInfo);
                    }
                

            }

            return View(characterInfo);
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
