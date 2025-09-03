using BL.Contracts;
using BL.Dtos;
using Exams.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exams.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]
    public class CreateExamController : Controller
    {
        private readonly IEaxme _Exam;

        public CreateExamController(IEaxme examService)
        {
            _Exam = examService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new BL.Dtos.ExamWithQuestionsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BL.Dtos.ExamWithQuestionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _Exam.Create(model); // 👈 استدعاء ميثود الـ Service مباشرة

                return RedirectToAction("Index"); // بعد الإنشاء يرجع لقائمة الامتحانات
            }


            return View(model);
        }

    }
}
