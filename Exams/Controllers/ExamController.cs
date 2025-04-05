using BL.Contracts;
using BL.Dtos;
using Exams.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exams.Controllers
{
    [Authorize(Roles = "Student")]
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
        public IActionResult StartExam(Guid examId)
        {
            var exam = _Exam.GetById(examId);
            if (exam == null) return NotFound("Exam not found.");

            var questions = _Qustion.GetAll()
                                    .Where(q => q.ExamId == examId)
                                    .Select(q => new TbQuestionDto
                                    {
                                        QuestionText = q.QuestionText,
                                        Id = q.Id,
                                        ExamId = q.ExamId,
                                        Choices = _Choice.GetAll()
                                                         .Where(c => c.QuestionId == q.Id)
                                                         .ToList()
                                    }).ToList();

            if (!questions.Any() || questions.All(q => q.Choices.Count == 0))
            {
                TempData["MessageType"] = "NoQuestionsOrChoices";
                return RedirectToAction("List", "Home");
            }

            var model = new ViewPageExam
            {
                Exam = exam,
                Questions = questions
            };

            return View(model);
        }


        // ✅ استقبال الإجابات ومعالجتها
        [HttpPost]
        [HttpPost]
        public IActionResult SubmitExam(Dictionary<Guid, Guid> Answers, Guid ExamId)
        {
            if (Answers == null || !Answers.Any())
            {
                TempData["MessageType"] = "NoAnswersReceived";
                return RedirectToAction("StartExam", new { examId = ExamId });
            }

            var exam = _Exam.GetById(ExamId);
            if (exam == null) return NotFound("Exam not found.");

            var questions = _Qustion.GetAll().Where(q => q.ExamId == ExamId).ToList();
            int totalQuestions = questions.Count;
            int correctAnswers = 0;

            // ✅ التحقق من صحة الإجابات
            foreach (var question in questions)
            {
                var correctChoice = _Choice.GetAll()
                                           .Where(c => c.QuestionId == question.Id && c.IsCorrect)
                                           .Select(c => c.Id) // ✅ استخراج الـ ID الصحيح فقط
                                           .FirstOrDefault();

                if (Answers.TryGetValue(question.Id, out Guid selectedChoiceId))
                {
                    if (correctChoice != Guid.Empty && correctChoice == selectedChoiceId)
                    {
                        correctAnswers++;
                    }
                }
            }

            // ✅ حساب النتيجة النهائية
            int score = totalQuestions > 0 ? (int)((double)correctAnswers / totalQuestions * 100) : 0;

            // ✅ حفظ النتيجة في قاعدة البيانات
            TbResultDto result = new TbResultDto
            {
                Id = Guid.NewGuid(),
                StudentName = User.Identity?.Name ?? "Unknown Student",
                ExamId = ExamId,
                Score = score,
                TakenDate = DateTime.Now
            };
            _Result.Add(result, result.Id);

            TempData["MessageType"] = "ExamSubmitted";
            return RedirectToAction("ExamResult", new { examId = ExamId, score = score });
        }


        // ✅ عرض صفحة النتيجة بعد الامتحان
        public IActionResult ExamResult(Guid examId, int score)
        {
            var exam = _Exam.GetById(examId);
            if (exam == null) return NotFound("Exam not found.");

            ViewBag.ExamTitle = exam.Title;
            ViewBag.Score = score;

            return View();
        }
    }
}
