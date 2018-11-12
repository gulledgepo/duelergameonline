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

        // GET: CharacterCreate
        public ActionResult Index()
        {
            return View(db.CharacterInfo.ToList());
        }

        // GET: CharacterCreate/Details/5
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

        // GET: CharacterCreate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CharacterCreate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,PlayerID,CharacterName,CharacterClass,Level,StatPointsAvailable,CurrentExperience,MaxExperienceForLevel,Health,Strength,Dexterity,Vitality,Luck,DuelsAvailable,DuelWins,DuelLosses")] CharacterInfo characterInfo)
        {
            if (ModelState.IsValid)
            {
                db.CharacterInfo.Add(characterInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(characterInfo);
        }

        // GET: CharacterCreate/Edit/5
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
            return View(characterInfo);
        }

        // POST: CharacterCreate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,PlayerID,CharacterName,CharacterClass,Level,StatPointsAvailable,CurrentExperience,MaxExperienceForLevel,Health,Strength,Dexterity,Vitality,Luck,DuelsAvailable,DuelWins,DuelLosses")] CharacterInfo characterInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(characterInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(characterInfo);
        }

        // GET: CharacterCreate/Delete/5
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

        // POST: CharacterCreate/Delete/5
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
