﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly WebShopDbContext _context;

        public HomeController(ILogger<HomeController> logger, WebShopDbContext context)
        {
            _logger = logger;
            //_context = context;
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            if (HttpContext.Session.ToString() == null)
            {
                TempData["Logout"] = "Bạn đã đăng xuất thành công";
            }
            else
            {
                TempData["MessLogout"] = "Đăng xuất thất bại, bạn chưa đăng nhập à";
            }
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Index()
        {
            var userrole = HttpContext.Session.GetString("UserRole");
            TempData["UserRole"] = userrole ?? "Guest";
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
