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
        Task<ViewPageExam> StartExam(Guid examId);
        Task<int> SubmitExam(Dictionary<Guid, Guid> answers, Guid examId, string studentName);
    }
}
