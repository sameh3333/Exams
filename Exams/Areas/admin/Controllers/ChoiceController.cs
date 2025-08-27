using BL.Contracts;
using BL.Dtos;
using Microsoft.AspNetCore.Mvc;
using Exams.Herpers;
using BL.Exceptions;
using Exams.Repositorys;
using Domin;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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

        public async Task<IActionResult> List(Guid questionId)
        {
            var choices =await _Choice.GetByQuestionId(questionId);
            ViewBag.QuestionId = questionId;
            return View(choices);

        }

        public async Task<IActionResult> Edit(Guid? Id, Guid questionId)
        {
            var data =await _Choice.EditChoice(Id, questionId);
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TbChoiceDto data)
        {
            TempData["MessageType"] = null;

            if (data == null)
            {
                TempData["MessageType"] = MessageType.SaveFailed;
                return RedirectToAction("List", new { questionId = data?.QuestionId });
            }

            try
            {
                // استدعاء Service لحفظ الاختيار (إضافة أو تعديل)
                var result =await  _Choice.SaveChoic(data);

                TempData["MessageType"] = result ? MessageType.SaveSucess : MessageType.SaveFailed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving choice.");
                TempData["MessageType"] = MessageType.SaveFailed;
            }

            return RedirectToAction("List", new { questionId = data.QuestionId });
        }

        public async Task<IActionResult> Delete(Guid id, Guid questionId)
        {
            TempData["MessageType"] = null;

            try
            {
                var choice = _Choice.GetById(id);
                if (choice != null)
                {
                   await _Choice.ChangeStatus(id, Guid.NewGuid());
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
