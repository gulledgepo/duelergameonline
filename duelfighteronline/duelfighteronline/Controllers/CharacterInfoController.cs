using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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

namespace duelfighteronline.Controllers
{
    public class CharacterInfoController : Controller
    {
        private CharacterClassContext db = new CharacterClassContext();

        //These two are necessary to access the UserID outside of account controller
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public CharacterInfoController()
        {
            //Again, necessary to access user ID
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }


        // GET: CharacterInfo
        public ActionResult Index()
        {
            //When a character is created the UserId is stored in a string attached to that character. 
            //On the index for each account we want to display on the characters associated with their account.
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.userid = user.Id;
            return View(db.CharacterInfo.Where(x => x.PlayerID == user.Id).ToList());
            
        }

        // GET: CharacterInfo/Details/5
        public ActionResult Duel (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterInfo characterInfo = db.CharacterInfo.Find(id);
            if (characterInfo == null)
            {
                return HttpNotFound();
            }
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (characterInfo.PlayerID == user.Id)
            {
                DuelViewModel model = new DuelViewModel();
                model.DuelInitiator = characterInfo;
                model.WinPercent = model.CalculateWinPercent(model.DuelInitiator);
                model.DuelInitiator.DuelsAvailable = 50;
                model.DuelInitiator.DuelHistory = db.DuelHistory.Where(x => x.CharacterInfoID == model.DuelInitiator.ID).ToList();
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to access other player's characters.";
                return RedirectToAction("Index", "CharacterInfo");
            }
        }

        // POST: CharacterInfo/Delete/5
        [HttpPost, ActionName("Duel")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CharacterName,CharacterClass,Level,StatPointsAvailable,CurrentExperience,MaxExperienceForLevel,Health,Strength,Dexterity,Vitality,Luck,PlayerID,DuelsAvailable,DuelWins,DuelLosses,Damage,CritChance,Dodge,CharacterInfo,DuelInitiator,DuelsRequested,DuelHistory")] DuelViewModel duelInfoReturn)
        {
            //Get the last entry in table as that IDs go up by 1 and that will always be the highest number for random generation
            CharacterInfo lastPlayer = new CharacterInfo();
            lastPlayer = db.CharacterInfo.OrderByDescending(x => x.ID).First();
            //Apply the last player to the view model to send to the duel logic
            duelInfoReturn.PlayersTotal = lastPlayer.ID;
            duelInfoReturn = CalculateDuelResultCommand.Execute(duelInfoReturn, db);
            
            //Set the character to the viewmodel
            CharacterInfo characterInfoSet = duelInfoReturn.DuelInitiator;
            CharacterInfo duelTargetInfo = duelInfoReturn.DuelTarget;
            //This is only a test
            TempData["message"] = duelInfoReturn.DuelTarget.CharacterName + "Was his namo" + characterInfoSet.CharacterName + "was the fighter Wins are: " + characterInfoSet.DuelWins + "Losses are: " + characterInfoSet.DuelLosses;
            //db.Entry(characterInfoSet).State = EntityState.Modified;
            db.Set<CharacterInfo>().AddOrUpdate(characterInfoSet);
            db.Set<CharacterInfo>().AddOrUpdate(duelTargetInfo);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }


        // GET: CharacterInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharacterInfo characterInfo = db.CharacterInfo.Find(id);
            if (characterInfo == null)
            {
                return HttpNotFound();
            }
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (characterInfo.PlayerID == user.Id)
            {
                CharacterInfoViewModel model = new CharacterInfoViewModel();
                model.CharacterInfo = characterInfo;
                model.ExperienceDisplay = characterInfo.CurrentExperience + "/" + characterInfo.MaxExperienceForLevel;
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to access other player's characters.";
                return RedirectToAction("Index", "CharacterInfo");
            }

        }

        // POST: CharacterInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CharacterName,CharacterClass,Level,StatPointsAvailable,CurrentExperience,MaxExperienceForLevel,Health,Strength,Dexterity,Vitality,Luck,PlayerID,DuelsAvailable,DuelWins,DuelLosses,Damage,CritChance,Dodge,CharacterInfo")] CharacterInfoViewModel characterInfoReturn)
        {
            if (ModelState.IsValid)
            {
                CharacterInfo characterInfoSet = characterInfoReturn.CharacterInfo;
                characterInfoSet.Health = characterInfoSet.CalculateHealth(characterInfoSet);
                characterInfoSet.Damage = characterInfoSet.CalculateDamage(characterInfoSet);
                characterInfoSet.CritChance = characterInfoSet.CalculateCritChance(characterInfoSet);
                characterInfoSet.DodgeChance = characterInfoSet.CalculateDodgeChance(characterInfoSet);
                db.Entry(characterInfoSet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            TempData["message"] = "Messed Up.";
            return View(characterInfoReturn);
        }



        // GET: CharacterInfo/Delete/5
        public ActionResult DuelHistory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DuelHistory duelHistory = db.DuelHistory.Find(id);
            if (duelHistory == null)
            {
                return HttpNotFound();
            }
            return View(duelHistory);
        }

        // POST: CharacterInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CharacterInfo characterInfo = db.CharacterInfo.Find(id);
            db.CharacterInfo.Remove(characterInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
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
