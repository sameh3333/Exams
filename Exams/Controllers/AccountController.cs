using BL.Contracts;
using BL.Dtos;
using Exams.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace Exams.Controllers
{
    public class AccountController : Controller
    {
 
        private readonly IUserServices _userServices;        

        public AccountController(IUserServices userServices  )
        {
           _userServices = userServices;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserDto model)
        {
            if (!ModelState.IsValid) return View(model);
            var registar = await _userServices.RegisterAsync(model);

            if (!registar.Success)
                return View("Register",model);

            return RedirectToAction("List", "Home");
        }

            public async Task<IActionResult> Logout()
            {
                await _userServices.LogoutAsenc();
                return RedirectToAction("Login");
            }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginDto model)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    var lodin = await _userServices.LoginAsync(model);
        //    if (!lodin.Success)
        //        return View("Login", model);
        //    return RedirectToAction("List", "Home");
        //}
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var login = await _userServices.LoginAsync(model);

            if (!login.Success)
                return View("Login", model);

            // ✅ بعد تسجيل الدخول بنجاح
            var dbUser = await _userServices.GetUserByEmailAsync(model.Email);
            var role = dbUser?.Role?.ToLower();

            if (role == "admin")
            {
                return RedirectToRoute(new { area = "Admin", controller = "Home", action = "Index" });
            }
            else
            {
                return RedirectToAction("List", "Home");
            }
        }

    }
}
