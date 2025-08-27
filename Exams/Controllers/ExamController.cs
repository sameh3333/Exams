using BL.Contracts;
using BL.Dtos;
using Exams.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Exams.Controllers
{
    [Authorize]

    public class ExamController : Controller
    {
        private readonly ILogger<ExamController> _logger;
        private readonly IEaxme _Exam;
        private readonly IQuestion _Qustion;
        private readonly IChoice _Choice;
        private readonly BL.Contracts.IResult _Result;

        public ExamController(ILogger<ExamController> logger, IEaxme exam, IQuestion qustion, IChoice choice, BL.Contracts.IResult result)
        {
            _logger = logger;
            _Exam = exam;
            _Qustion = qustion;
            _Choice = choice;
            _Result = result;
        }
        // ✅ عرض صفحة الامتحان مع الأسئلة
       

        public async Task<IActionResult> StartExam(Guid examId)
        {
            var model = await _Exam.StartExam(examId); // ✅ انتظار النتيجة
            return View(model);                         // ✅ تمرير النتيجة الفعلية وليس Task
        }
        // ✅ استقبال الإجابات ومعالجتها
        [HttpPost]
        public async Task<IActionResult> SubmitExam(Dictionary<Guid, Guid> Answers, Guid ExamId)
        {
            if (Answers == null || !Answers.Any())
            {
                TempData["MessageType"] = "NoAnswersReceived";
                return RedirectToAction("StartExam", new { examId = ExamId });
            }

            var studentName = User.Identity?.Name ?? "On Account";

            // استخدام الـ Service Async
            int score = await _Exam.SubmitExam(Answers, ExamId, studentName);

            return RedirectToAction("ExamResult", new { examId = ExamId, score = score });
        }

        // ✅ عرض صفحة النتيجة بعد الامتحان
        public async Task<IActionResult> ExamResult(Guid examId, int score)
        {
            var exam = await _Exam.GetById(examId);
            if (exam == null) return NotFound("Exam not found.");

            ViewBag.ExamTitle = exam.Title;
            ViewBag.Score = score;

            return View();
        }

        // ✅ عرض صفحة النتيجة بعد الامتحان
     
    }
}
