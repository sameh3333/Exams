using BL.Contracts;
using BL.Dtos;
using Microsoft.AspNetCore.Mvc;
using Exams.Herpers;
using BL.Exceptions;
using Exams.Repositorys;
using Domin;
using Microsoft.AspNetCore.Authorization;
namespace Exams.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]

    public class QuestionController : Controller
    {
        private readonly IQuestion _Qusetion;
        private readonly ILogger<QuestionController> _logger;
        public QuestionController(IQuestion qestion, ILogger<QuestionController> logger)
        {

            _Qusetion = qestion;
            _logger = logger;
        }
   
        public IActionResult List(Guid? ExamId)
        {
            if (!ExamId.HasValue || ExamId == Guid.Empty)
            {
                TempData["MessageType"] = MessageType.SaveSucess;
                return RedirectToAction("List", "Exam");
            }

            var getdata = _Qusetion.GetAll()
                                   .Where(e => e.ExamId == ExamId)
                                   .ToList();

            ViewBag.ExamId = ExamId.Value; // ✅ التأكد من تمرير ExamId عند العودة
            return View(getdata);
        }
        public IActionResult Edit(Guid? Id, Guid examId)
        {
            var data = Id.HasValue ? _Qusetion.GetById((Guid)Id) : new BL.Dtos.TbQuestionDto { ExamId = examId };
            ViewBag.ExamId = data.ExamId;
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TbQuestionDto data)
        {
            TempData["MessageType"] = null;
            if (data.ExamId == Guid.Empty)
            {
                TempData["MessageType"] = MessageType.SaveFailed;
                return RedirectToAction("Edit", new { Id = data.Id, ExamId = ViewBag.ExamId });
            }
            if (!ModelState.IsValid)
                return View("Edit", data);
            try
            {
                if (data.Id == Guid.Empty)
                    _Qusetion.Add(data, data.Id);
                else
                    _Qusetion.Update(data, data.Id);
                TempData["MessageType"] = MessageType.SaveSucess;
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = MessageType.SaveFailed;

                //throw new DataAccessException(ex, "", _logger);
            }
            return RedirectToAction("List", new { ExamId = data.ExamId });
        }
        
        public IActionResult Delete(Guid id, Guid examId)
        {
            TempData["MessageType"] = null;
            try
            {
                var question = _Qusetion.GetById(id);
                if (question != null)
                {
                    _Qusetion.ChangeStatus(id, Guid.NewGuid());
                    TempData["MessageType"] = MessageType.DeleteSucess;
                }
                else
                {
                    TempData["MessageType"] = MessageType.DeleteFailed;
                }
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = MessageType.DeleteFailed;
               throw new DataAccessException(ex, "", _logger);
            }

            return RedirectToAction("List", new { ExamId = examId });
        }
    }
    }

