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

namespace duelfighteronline.Controllers
{
    public class CharacterInfoController : Controller
    {
        private CharacterClassContext db = new CharacterClassContext();

        // GET: CharacterInfo
        public ActionResult Index()
        {
            return View(db.CharacterInfo.ToList());
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
            return View(characterInfo);
        }

        // POST: CharacterInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CharacterName,CharacterClass,Level,StatPointsAvailable,CurrentExperience,MaxExperienceForLevel,Health,Strength,Dexterity,Vitality,Luck")] CharacterInfo characterInfo)
        {
            if (ModelState.IsValid)
            {
                db.CharacterInfo.Add(characterInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            return View(characterInfo);
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
