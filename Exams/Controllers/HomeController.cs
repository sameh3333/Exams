using Exams.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BL.Contracts;
using BL.Services;
using Microsoft.AspNetCore.Authorization;
namespace Exams.Controllers
{
    [Authorize(Roles = "Student")]
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
        public IActionResult List()
        {
            var getdata = _eaxme.GetAll();
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
