using Exams.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BL.Contracts;
using BL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
namespace Exams.Controllers
{

    [Authorize]
    public class HomeController : Controller
    { 
        private readonly ILogger<HomeController> _logger;
        private readonly IEaxme _eaxme;
        private readonly IQuestion _Question;
        public HomeController(ILogger<HomeController> logger , IEaxme eaxme, IQuestion question)
        {
            _logger = logger;
            _eaxme = eaxme;
            _Question = question;   
           
        }
        public async Task<IActionResult> List()
        {
            var getdata =await _eaxme.GetAll();
            return View(getdata);
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
