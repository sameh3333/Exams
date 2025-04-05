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

    public class ChoiceController : Controller
    {
        private readonly IChoice _Choice;
        private readonly ILogger<ChoiceController> _logger;

        public ChoiceController(IChoice choice, ILogger<ChoiceController> logger)
        {
            _Choice = choice;
            _logger = logger;
        }

        public IActionResult List(Guid questionId)
        {
            var choices = _Choice.GetAll()
                                 .Where(q => q.QuestionId == questionId)
                                 .ToList();

            ViewBag.QuestionId = questionId;
            return View(choices);

        }

        public IActionResult Edit(Guid? Id, Guid questionId)
        {
            var data = Id.HasValue && Id != Guid.Empty ? _Choice.GetById((Guid)Id) : new TbChoiceDto { QuestionId = questionId };
            ViewBag.QuestionId = questionId;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TbChoiceDto data)
        {
            TempData["MessageType"] = null;

            if (data.QuestionId == Guid.Empty)
            {
                TempData["MessageType"] = MessageType.SaveFailed;
                return RedirectToAction("Edit", new { Id = data.Id, QuestionId = data.QuestionId });
            }

            try
            {
                if (data.IsCorrect)
                {
                    // ✅ اجعل كل الاختيارات الأخرى غير صحيحة إذا تم تحديد هذا الاختيار كإجابة صحيحة
                    var allChoices = _Choice.GetAll().Where(q => q.QuestionId == data.QuestionId).ToList();
                    foreach (var choice in allChoices)
                    {
                        var currentCorrectChoice = _Choice.GetAll()
                                             .FirstOrDefault(q => q.QuestionId == data.QuestionId && q.IsCorrect);

                        if (choice.Id != data.Id) // ✅ تأكد من عدم تحديث نفس الإجابة التي يتم حفظها الآن
                        {
                            choice.IsCorrect = false;
                            _Choice.Update(choice, choice.Id);
                            TempData["MessageType"] = MessageType.SaveSucess;
                        }
                    }
                }

                if (data.Id == Guid.Empty)
                {
                    // ✅ إضافة اختيار جديد
                    _Choice.Add(data, data.Id);
                    TempData["MessageType"] = MessageType.SaveSucess;
                }
                else
                {
                    // ✅ تحديث اختيار موجود
                    var existingChoice = _Choice.GetById(data.Id);
                    if (existingChoice == null)
                    {
                        TempData["MessageType"] = MessageType.SaveFailed;
                        return RedirectToAction("Edit", new { Id = data.Id, QuestionId = data.QuestionId });
                    }

                    // تحديث البيانات
                    existingChoice.ChoiceText = data.ChoiceText;
                    existingChoice.IsCorrect = data.IsCorrect;

                    _Choice.Update(existingChoice, existingChoice.Id);
                }

                TempData["MessageType"] = MessageType.SaveSucess;
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = MessageType.SaveFailed;
            }

            return RedirectToAction("List", new { questionId = data.QuestionId });
        }

        public IActionResult Delete(Guid id, Guid questionId)
        {
            TempData["MessageType"] = null;

            try
            {
                var choice = _Choice.GetById(id);
                if (choice != null)
                {
                    _Choice.ChangeStatus(id, Guid.NewGuid());
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

            return RedirectToAction("List", new { questionId });
        }



        
    }
}
