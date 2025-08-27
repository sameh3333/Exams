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
using DAL.Dtos;
using Exams.Models;
namespace BL.Services
{
    public  class ExamSecrvices :BaseServices<TbExam,TbExamDto>, IEaxme
    {


        private readonly IQuestion _questionRepo;
        private readonly IChoice _choiceRepo;
        private readonly IMapper _mapper;
        private readonly IResult _resultRepo;

        public ExamSecrvices(
         IGenericRepository<TbExam> redo,
       IQuestion questionRepo,
        IChoice choiceRepo,
         IMapper mapper,
         IUserServices userServices,
            IResult resultRepo

     ) : base(redo, mapper, userServices)
        {
            _questionRepo = questionRepo;
            _choiceRepo = choiceRepo;
            _mapper = mapper;
            _resultRepo = resultRepo;
        }



        public async Task<DAL.Dtos.ViewPageExam> StartExam(Guid examId)
        {
            // 1️⃣ جلب الامتحان
            var exam = await GetById(examId);
            if (exam == null)
                return null; // أو throw new NotFoundException("Exam not found");

            // 2️⃣ جلب كل الأسئلة والاختيارات مرة واحدة
            var questions = await _questionRepo.GetAll();
            var choices = await _choiceRepo.GetAll();

            // 3️⃣ إنشاء قائمة الأسئلة مع تحويل الاختيارات إلى DTO
            var examQuestions = questions
                .Where(q => q.ExamId == examId)
                .Select(q =>
                {
                    // تحويل السؤال إلى DTO
                    var questionDto = _mapper.Map<TbQuestionDto>(q);

                    // جلب وتحويل الاختيارات الخاصة بهذا السؤال
                    questionDto.Choices = _mapper.Map<List<TbChoiceDto>>(
                        choices
                            .Where(c => c.QuestionId == q.Id)
                            .OrderBy(c => c.ChoiceText)
                            .ToList()
                    );

                    return questionDto;
                })
                .Where(q => q.Choices.Any()) // حذف أي سؤال بدون اختيارات
                .ToList();

            // 4️⃣ التحقق من وجود أسئلة صالحة
            if (!examQuestions.Any())
                return null; // أو throw new Exception("No questions with choices found");

            // 5️⃣ إنشاء نموذج ViewPageExam وإرجاعه
            return new ViewPageExam
            {
                Exam = exam,
                Questions = examQuestions
            };
        }


        public async Task<int> SubmitExam(Dictionary<Guid, Guid> answers, Guid examId, string studentName)
        {
            if (answers == null || !answers.Any())
                return 0;

            // جلب الامتحان
            var exam = await GetById(examId);
            if (exam == null)
                throw new Exception("Exam not found.");

            // جلب كل الأسئلة الخاصة بالامتحان
            var questions = await _questionRepo.GetByExamId(examId);
            if (questions == null || !questions.Any())
                return 0;

            int totalQuestions = questions.Count;
            int correctAnswers = 0;

            // جلب كل الاختيارات لكل الأسئلة مرة واحدة
            var choices = new List<TbChoiceDto>();
            foreach (var q in questions)
            {
                var questionChoices = await _choiceRepo.GetByQuestionId(q.Id);
                choices.AddRange(questionChoices);
            }

            // التحقق من صحة الإجابات
            foreach (var question in questions)
            {
                var correctChoiceId = choices
                    .Where(c => c.QuestionId == question.Id && c.IsCorrect)
                    .Select(c => c.Id)
                    .FirstOrDefault();

                if (answers.TryGetValue(question.Id, out Guid selectedChoiceId))
                {
                    if (correctChoiceId != Guid.Empty && correctChoiceId == selectedChoiceId)
                        correctAnswers++;
                }
            }

            // حساب النتيجة
            int score = totalQuestions > 0 ? (int)((double)correctAnswers / totalQuestions * 100) : 0;

            // حفظ النتيجة
            var result = new TbResultDto
            {
                Id = Guid.NewGuid(),
                StudentName = studentName,
                ExamId = examId,
                Score = score,
                TakenDate = DateTime.Now
            };

            await _resultRepo.Add(result);

            return score;
        }
    }

}

