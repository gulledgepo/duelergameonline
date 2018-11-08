﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace duelfighteronline.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CharacterInfo()
        {
            ViewBag.Message = "Character Info.";

            return View();
        }

        public ActionResult CharacterStats()
        {
            ViewBag.Message = "Character Stats.";

            return View();
        }
    }
}