using BL.Dtos;
using Domin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IQuestion : IBaseServices<TbQuestion, TbQuestionDto>
    {

         Task<List<TbQuestionDto>> GetByExamId(Guid? examId);
        Task< TbQuestionDto> QuestionEdit(Guid? id, Guid examId);
    }

}

