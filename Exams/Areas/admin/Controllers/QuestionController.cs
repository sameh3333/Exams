using BL.Contracts;
using BL.Dtos;
using Microsoft.AspNetCore.Mvc;
using Exams.Herpers;
using BL.Exceptions;
using Domin;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
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
   
        public async Task<IActionResult> List(Guid? ExamId)
        {


            var getdata =await _Qusetion.GetByExamId(ExamId);
            return View(getdata);
        }
        public IActionResult Edit(Guid? Id, Guid examId)
        {

            var data = _Qusetion.QuestionEdit(Id, examId);  
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TbQuestionDto data)
        {
            TempData["MessageType"] = null;
           
          
            if (!ModelState.IsValid)
                return View("Edit", data);
            try
            {
                if (data.Id == Guid.Empty)
                  await  _Qusetion.Add(data);
                else
                  await  _Qusetion.Update(data);
                TempData["MessageType"] = MessageType.SaveSucess;
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = MessageType.SaveFailed;

                //throw new DataAccessException(ex, "", _logger);
            }
            return RedirectToAction("List", new { ExamId = data.ExamId });
        }
        
        public async Task<IActionResult> Delete(Guid id, Guid examId)
        {
            TempData["MessageType"] = null;
            try
            {
                var question =await _Qusetion.GetById(id);
                if (question != null)
                {
                   await _Qusetion.ChangeStatus(id, Guid.NewGuid());
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

