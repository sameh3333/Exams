using BL.Contracts;
using BL.Dtos;
using Microsoft.AspNetCore.Mvc;
using Exams.Herpers;
using BL.Exceptions;
using Microsoft.AspNetCore.Authorization;
namespace Exams.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Admin")]

    public class ExamController : Controller
    {
        private readonly IEaxme _Exam;
        private readonly ILogger<ExamController> _logger;
        public ExamController(IEaxme exam , ILogger<ExamController> logger)
        {

            _Exam = exam;
            _logger = logger;
        }
        public IActionResult List()
        {
            var getdata = _Exam.GetAll();

            return View(getdata);
        }
        public IActionResult Edit(Guid? Id)
        {
            var data = Id.HasValue ? _Exam.GetById((Guid)Id) : new TbExamDto();
            return View(data);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TbExamDto data)
        {
            TempData["MessageType"] = null;
            if (!ModelState.IsValid)
                return View("Edit", data);
            try
            {
                if (data.Id == Guid.Empty)
                    _Exam.Add(data, data.Id);
                else
                    _Exam.Update(data, data.Id);
                TempData["MessageType"] = MessageType.SaveSucess;
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = MessageType.SaveFailed;

                throw new DataAccessException(ex, "", _logger);
            }
            return RedirectToAction("List");
        }
        public IActionResult Delete(Guid id)
        {
            TempData["MessageType"] = null;
            try
            {
                _Exam.ChangeStatus(id, Guid.NewGuid());
               TempData["MessageType"] = MessageType.DeleteSucess;
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = MessageType.DeleteFailed;

                throw new DataAccessException(ex, "", _logger);
            }
            return RedirectToAction("List");
        }
    }
}
