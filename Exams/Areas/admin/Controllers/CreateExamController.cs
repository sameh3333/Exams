using BL.Contracts;
using BL.Dtos;
using BL.Exceptions;
using Exams.Herpers;
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
        private readonly ILogger<CreateExamController> _logger;

        public CreateExamController(IEaxme examService, ILogger<CreateExamController> logger)
        {
            _Exam = examService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var exams = await _Exam.GetAll();
            return View(exams);
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
            try {

                if (ModelState.IsValid)
                {
                    await _Exam.Create(model); // 👈 استدعاء ميثود الـ Service مباشرة
                    TempData["MessageType"] = MessageType.SaveSucess;

                    return RedirectToAction("List"); // بعد الإنشاء يرجع لقائمة الامتحانات
                }

            }
            catch (Exception ex)
            {

                TempData["MessageType"] = MessageType.SaveFailed;
                throw new DataAccessException(ex, "", _logger);
            }



            return View(model);
        }
      
        // 📌 GET: admin/Exam/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            TempData["MessageType"] = null;
            var viewModel = await _Exam.GetExamWithQuestionsAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        // 📌 POST: admin/Exam/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BL.Dtos.ExamWithQuestionsViewModel model)
        {
            TempData["MessageType"] = null;
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if(model==null)
                    await _Exam.GetExamWithQuestionsAsync(id);
                // ✨ هنا الكنترولر بينادي السيرفيس بتاعك علطول
                await _Exam.Edit(id, model);

                TempData["MessageType"] = MessageType.SaveSucess;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                  TempData["MessageType"] = MessageType.SaveFailed;
                ModelState.AddModelError("", $"Error updating exam: {ex.Message}");
                return View(model);
            }
        }

        // 📌 POST: admin/CreateExam/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(Guid id)
        {
            try
            {
                TempData["MessageType"] = null;

                await _Exam.Disable(id); // استدعاء ميثود الـ Service لتعطيل الامتحان
                TempData["MessageType"] = MessageType.SaveSucess;

            }
            catch (Exception ex)
            {

                TempData["MessageType"] = MessageType.SaveFailed;
                throw new DataAccessException(ex, "", _logger);
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            try
            {
                TempData["MessageType"] = null;

                await _Exam.ToggleActive(id); // Service تغير حالة IsActive

                TempData["MessageType"] = MessageType.SaveSucess;
            }
            catch (Exception ex)
            {

                TempData["MessageType"] = MessageType.SaveFailed;
                throw new DataAccessException(ex, "", _logger);
            }

            return RedirectToAction("List");
        }


    }
}
