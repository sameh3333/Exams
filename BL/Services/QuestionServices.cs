using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using BL.Exceptions;
using BL.Maping;
using DAL.Context;
using Domin;
using Exams.Contracts;
using System.Threading.Tasks;


namespace BL.Services
{
    public class QuestionServices : BaseServices<TbQuestion, TbQuestionDto>, IQuestion
    {

        private readonly IGenericRepository<TbQuestion> _repo;
        private readonly AutoMapper.IMapper _mapper;


        public QuestionServices(IGenericRepository<TbQuestion> repo, AutoMapper.IMapper mapper, IUserServices userservices)
            : base(repo, mapper, userservices)
        {
            _repo = repo;
            _mapper = mapper;
        }


     


        public async Task<List<TbQuestionDto>> GetByExamId(Guid? examId)
        {
            // انتظار الحصول على كل الأسئلة
            var allQuestions = await _repo.GetAll();

            // تصفية حسب examId
            var filteredQuestions = allQuestions
                                     .Where(q => q.ExamId == examId)
                                     .ToList();

            // تحويل باستخدام AutoMapper
            return _mapper.Map<List<TbQuestion>, List<TbQuestionDto>>(filteredQuestions);
        }

        public async Task<TbQuestionDto> QuestionEdit(Guid? id, Guid examId)
        {
            if (id.HasValue && id != Guid.Empty)
            {
                var entity = await _repo.GetById(id.Value);
                return  _mapper.Map<TbQuestion, TbQuestionDto>(entity);
            }

            // لو Id مش موجود، يبقى إنشاء سؤال جديد
            return new TbQuestionDto { ExamId = examId };
        }

      

    }
}
