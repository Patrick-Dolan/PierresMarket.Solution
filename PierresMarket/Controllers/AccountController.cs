using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PierresMarket.Models;
using System.Threading.Tasks;
using PierresMarket.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    public class AccountController : Controller
    {
        private readonly PierresMarketContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController (UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, PierresMarketContext db, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public ActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            ApplicationUser user = _userManager.FindByIdAsync(userId).Result;
            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register (RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register");
            }
            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, ImageFile = model.ImageFile  };
            //Image handling
            if (user.ImageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(user.ImageFile.FileName);
                string extention = Path.GetExtension(user.ImageFile.FileName);
                user.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssffff") + extention;
                string path = Path.Combine(wwwRootPath + "/img/ProfilePictures/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                await user.ImageFile.CopyToAsync(fileStream);
                }
            }
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}