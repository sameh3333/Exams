using BL.Contracts;
using BL.Dtos;
using BL.Exceptions;
using BL.Services;
using Exams.Herpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Exams.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]

    public class ResultController : Controller
    {
        private readonly BL.Contracts.IResult _Result;
        private readonly ILogger<ResultController> _logger;
        public ResultController(BL.Contracts.IResult exam , ILogger<ResultController> logger)
        {

            _Result = exam;
            _logger = logger;
        }

        public async Task<IActionResult> List()
        {
            var getdata = await _Result.GetAll();
              

            return View(getdata);
        }
        // ✅ تفاصيل نتيجة
        public async Task<IActionResult> Details(Guid id)
        {
            var result =await _Result.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // ✅ إنشاء نتيجة جديدة
       

        

      

        // ✅ حذف نتيجة
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var result = _Result.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

       
    }
}
