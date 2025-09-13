using BL.Dtos;
using DAL.Dtos;
using Domin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IEaxme : IBaseServices<TbExam, TbExamDto>
    {

        Task<bool> DeleteQuestionWithChoicesAsync(Guid questionId);
        Task<bool> DeleteChoiceAsync(Guid choiceId);
        Task<bool> DeleteExam(Guid examId);
        Task ToggleActive(Guid examId);
        Task<ExamWithQuestionsViewModel> Edit(Guid examId, ExamWithQuestionsViewModel model);
         Task<ExamWithQuestionsViewModel> GetExamWithQuestionsAsync(Guid examId);
        Task<bool> Disable(Guid examId);
        public Task<Guid> Create(ExamWithQuestionsViewModel model);
          Task<Guid> AddExamWithQuestionsAndChoices(TbExamDto examDto, List<TbQuestionDto> questions);
        Task<ViewPageExam> StartExam(Guid examId);
        Task<int> SubmitExam(Dictionary<Guid, Guid> answers, Guid examId, string studentName);
    }
}
