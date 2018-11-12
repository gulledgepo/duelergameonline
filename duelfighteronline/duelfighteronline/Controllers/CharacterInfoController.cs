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
            ViewBag.userid = user.Id;
            return View(db.CharacterInfo.Where(x => x.PlayerID == user.Id).ToList());
            
        }

        // GET: CharacterInfo/Details/5
        public ActionResult Details(int? id)
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
            return View(characterInfo);
        }

        // GET: CharacterInfo/Create
        public ActionResult Create()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            //Pass in the initial stats for each character for creation.
            CharacterInfo characterInfo = new CharacterInfo();
            characterInfo.Level = 1;
            characterInfo.CurrentExperience = 0;
            characterInfo.MaxExperienceForLevel = 50;
            characterInfo.StatPointsAvailable = 30;
            characterInfo.Health = 50;
            characterInfo.Strength = 1;
            characterInfo.Vitality = 1;
            characterInfo.Dexterity = 1;
            characterInfo.Luck = 1;
            //This passes the userID into the character creation where it will be stored to associate that character with that user.
            characterInfo.PlayerID = user.Id;
            return View(characterInfo);
        }

        // POST: CharacterInfo/Create
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
                        db.CharacterInfo.Add(characterInfo);
                        db.SaveChanges();
                        return RedirectToAction("Index");
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
                model.characterInfo = characterInfo;
                model.characterInfo.StatPointsAvailable = 5;
                model.ExperienceDisplay = characterInfo.CurrentExperience + "/" + characterInfo.MaxExperienceForLevel;
                return View(model);
            } else
            {
                TempData["message"] = "Unable to access other player's characters.";
                return RedirectToAction("Index");
            }
            
        }

        // POST: CharacterInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CharacterName,CharacterClass,Level,StatPointsAvailable,CurrentExperience,MaxExperienceForLevel,Health,Strength,Dexterity,Vitality,Luck")] CharacterInfo characterInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(characterInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(characterInfo);
        }

        // GET: CharacterInfo/Delete/5
        public ActionResult Delete(int? id)
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
            return View(characterInfo);
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
