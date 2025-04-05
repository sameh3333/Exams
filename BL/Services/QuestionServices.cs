using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using BL.Exceptions;
using DAL.Context;
using Domin;
using Exams.Contracts;
using Exams.Repositorys;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class QuestionServices : BaseServices<TbQuestion, TbQuestionDto>, IQuestion
    {
        public QuestionServices(IGenericRepository<TbQuestion> redo, IMapper mapper) : base(redo, mapper) { }
    }
}
