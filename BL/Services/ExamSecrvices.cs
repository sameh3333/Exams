using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using DAL.Dtos;
using Domin;
using Exams.Contracts;
namespace BL.Services
{
    public  class ExamSecrvices :BaseServices<TbExam,TbExamDto>, IEaxme
    {


        private readonly IQuestion _questionRepo;
        private readonly IChoice _choiceRepo;
        private readonly IMapper _mapper;
        private readonly IResult _resultRepo;
        private readonly IUnitOfWork _uow;
        public ExamSecrvices(
         IGenericRepository<TbExam> redo,
       IQuestion questionRepo,
        IChoice choiceRepo,
         IMapper mapper,
         IUserServices userServices,
            IResult resultRepo,
            IUnitOfWork uow


     ) : base(redo, mapper, userServices)
        {
            _uow=uow;   
            _questionRepo = questionRepo;
            _choiceRepo = choiceRepo;   
            _mapper = mapper;
            _resultRepo = resultRepo;
        }

        public async Task<ExamWithQuestionsViewModel> GetExamWithQuestionsAsync(Guid examId)
        {
            // ✅ هات الامتحان
            var exam = await _uow.Repository<TbExam>().GetById(examId);
            if (exam == null) return null;

            // ✅ هات الأسئلة الخاصة بالامتحان
            var questions = (await _uow.Repository<TbQuestion>().GetAll())
                .Where(q => q.ExamId == examId)
                .ToList();

            // ✅ هات الاختيارات الخاصة بالأسئلة دي فقط
            var questionIds = questions.Select(q => q.Id).ToList();
            var allChoices = (await _uow.Repository<TbChoice>().GetAll())
                .Where(c => questionIds.Contains(c.QuestionId))
                .ToList();

            // ✅ رجّع الـ ViewModel كامل
            var data = new ExamWithQuestionsViewModel
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                Questions = questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    Text = q.QuestionText,
                    Choices = allChoices
                        .Where(c => c.QuestionId == q.Id)
                        .Select(c => new ChoiceViewModel
                        {
                            Id = c.Id,
                            Text = c.ChoiceText,
                            IsCorrect = c.IsCorrect
                        }).ToList()
                }).ToList()
            };

            return data;
        }

        public async Task ToggleActive(Guid examId)
        {
            var exam = await _uow.Repository<TbExam>().GetById(examId);
            if (exam == null) throw new Exception("Exam not found.");

            exam.IsActive = !exam.IsActive; // ✅ تبديل الحالة
            await _uow.Repository<TbExam>().Update(exam);
            await _uow.CommitAsync();
        }


        public async Task<bool> Disable(Guid examId)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var exam = await _uow.Repository<TbExam>().GetById(examId);
                if (exam == null)
                    throw new Exception("Exam not found.");

                exam.IsActive = false; // تعطيل الامتحان
                await _uow.Repository<TbExam>().Update(exam);

                var questions = (await _uow.Repository<TbQuestion>().GetAll())
                                .Where(q => q.ExamId == examId)
                                .ToList();

                var allChoices = await _uow.Repository<TbChoice>().GetAll();

                foreach (var q in questions)
                {
                    q.IsActive = false; // تعطيل السؤال
                    await _uow.Repository<TbQuestion>().Update(q);

                    var choices = allChoices.Where(c => c.QuestionId == q.Id).ToList();
                    foreach (var choice in choices)
                    {
                        choice.IsActive = false; // تعطيل الاختيار
                        await _uow.Repository<TbChoice>().Update(choice);
                    }
                }

                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> Edit(Guid examId, ExamWithQuestionsViewModel model)
        {
            if (model == null)
                throw new ArgumentException("Invalid data to update exam.");

            await _uow.BeginTransactionAsync();
            try
            {
                // ✅ Update exam
                var existingExam = await _uow.Repository<TbExam>().GetById(examId);
                if (existingExam == null)
                    throw new Exception("Exam not found.");

                existingExam.Title = model.Title;
                existingExam.Description = model.Description;
                existingExam.UpdatedDate = DateTime.Now;

                await _uow.Repository<TbExam>().Update(existingExam);

                // ✅ Get all existing questions + choices
                var existingQuestions = (await _uow.Repository<TbQuestion>().GetAll())
                    .Where(q => q.ExamId == examId)
                    .ToList();

                var existingChoices = await _uow.Repository<TbChoice>().GetAll();

                // ✅ Loop over incoming questions
                foreach (var questionVm in model.Questions)
                {
                    TbQuestion question = null;

                    if (questionVm.Id.HasValue)
                    {
                        question = existingQuestions.FirstOrDefault(q => q.Id == questionVm.Id.Value);
                    }

                    if (question != null)
                    {
                        // Update
                        question.QuestionText = questionVm.Text;
                        question.UpdatedDate = DateTime.Now;
                        await _uow.Repository<TbQuestion>().Update(question);

                        // Update choices
                        foreach (var choiceVm in questionVm.Choices)
                        {
                            var choice = choiceVm.Id.HasValue
                                ? existingChoices.FirstOrDefault(c => c.Id == choiceVm.Id.Value)
                                : null;

                            if (choice != null)
                            {
                                choice.ChoiceText = choiceVm.Text;
                                choice.IsCorrect = choiceVm.IsCorrect;
                                choice.UpdatedDate = DateTime.Now;
                                await _uow.Repository<TbChoice>().Update(choice);
                            }
                            else
                            {
                                var newChoice = new TbChoice
                                {
                                    Id = Guid.NewGuid(),
                                    QuestionId = question.Id,
                                    ChoiceText = choiceVm.Text,
                                    IsCorrect = choiceVm.IsCorrect,
                                    CreatedDate = DateTime.Now,
                                    CurrentState = 1
                                };
                                await _uow.Repository<TbChoice>().Add(newChoice);
                            }
                        }
                    }
                    else
                    {
                        // Add new question
                        var newQuestion = new TbQuestion
                        {
                            Id = Guid.NewGuid(),
                            ExamId = examId,
                            QuestionText = questionVm.Text,
                            CreatedDate = DateTime.Now,
                            CurrentState = 1
                        };
                        await _uow.Repository<TbQuestion>().Add(newQuestion);

                        foreach (var choiceVm in questionVm.Choices)
                        {
                            var newChoice = new TbChoice
                            {
                                Id = Guid.NewGuid(),
                                QuestionId = newQuestion.Id,
                                ChoiceText = choiceVm.Text,
                                IsCorrect = choiceVm.IsCorrect,
                                CreatedDate = DateTime.Now,
                                CurrentState = 1
                            };
                            await _uow.Repository<TbChoice>().Add(newChoice);
                        }
                    }
                }

                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        
        public async Task<Guid> Create(ExamWithQuestionsViewModel model)
        {
            if (model == null || model.Questions == null || !model.Questions.Any())
                throw new ArgumentException("البيانات غير مكتملة لإنشاء الامتحان.");

            // نحول ViewModel إلى DTOs
            var examDto = new TbExamDto
            {
                Title = model.Title,
                Description = model.Description,
            };

            var questionDtos = model.Questions.Select(q => new TbQuestionDto
            {
                QuestionText = q.Text,
                Choices = q.Choices.Select(c => new TbChoiceDto
                {
                    ChoiceText = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            }).ToList();

            // نستدعي الميثود اللي بتحفظ
            return await AddExamWithQuestionsAndChoices(examDto, questionDtos);
        }

        public async Task<Guid> AddExamWithQuestionsAndChoices( TbExamDto examDto,List<TbQuestionDto> questions)
        {
            if (examDto == null || questions == null || !questions.Any())
                throw new ArgumentException("البيانات غير مكتملة لإنشاء الامتحان.");

            await _uow.BeginTransactionAsync();
            try
            {
                // 1️⃣ إضافة الامتحان
                examDto.Id = Guid.NewGuid();
                examDto.CreatedDate = DateTime.Now;
                examDto.CurrentState = 1;

                await _uow.Repository<TbExam>().Add(_mapper.Map<TbExam>(examDto));

                // 2️⃣ إضافة الأسئلة والاختيارات
                foreach (var questionDto in questions)
                {
                    questionDto.Id = Guid.NewGuid();
                    questionDto.ExamId = examDto.Id;
                    questionDto.CreatedDate = DateTime.Now;
                    questionDto.CurrentState = 1;

                    await _uow.Repository<TbQuestion>().Add(_mapper.Map<TbQuestion>(questionDto));

                    if (questionDto.Choices != null && questionDto.Choices.Any())
                    {
                        foreach (var choiceDto in questionDto.Choices)
                        {
                            choiceDto.Id = Guid.NewGuid();
                            choiceDto.QuestionId = questionDto.Id;
                            choiceDto.CreatedDate = DateTime.Now;
                            choiceDto.CurrentState = 1;

                            await _uow.Repository<TbChoice>().Add(_mapper.Map<TbChoice>(choiceDto));
                        }
                    }
                }

                // 3️⃣ حفظ التغييرات والـ Commit
                await _uow.CommitAsync();

                return examDto.Id;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
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

