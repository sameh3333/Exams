using BL.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exams.Areas.admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]


    public class UsersController : Controller
    {
        private readonly IUserServices _userServices;

        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        public async Task<IActionResult> List()
        {
            var users = await _userServices.GetAllUsersAsync();
            return View(users);
        }
    }
}
