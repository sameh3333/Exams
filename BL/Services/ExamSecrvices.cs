using BL.Contracts;
using Domin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos;
using Exams.Contracts;
using System.Threading;
using AutoMapper;
namespace BL.Services
{
    public  class ExamSecrvices :BaseServices<TbExam,TbExamDto>, IEaxme
    {
        public ExamSecrvices(IGenericRepository<TbExam> redo, IMapper mapper) : base(redo, mapper) { }
    }
}
