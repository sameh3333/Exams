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
        private readonly  IGenericRepository<TbExam> _redo;
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
            _redo = redo;
        }



        

        public async Task<ExamWithQuestionsViewModel> GetExamWithQuestionsAsync(Guid examId)
        {
            // 1️⃣ جلب الامتحان من قاعدة البيانات
            var exam = await _uow.Repository<TbExam>().GetById(examId);
            if (exam == null) return null;

            // 2️⃣ جلب جميع الأسئلة الخاصة بالامتحان
            var questionDtos = await _questionRepo.GetByExamId(examId);

            // 3️⃣ إنشاء الـ ViewModel للامتحان
            var examVm = new ExamWithQuestionsViewModel
            {
                Id = exam.Id,
                Title = exam.Title ?? string.Empty,
                Description = exam.Description ?? string.Empty,
                Questions = new List<QuestionViewModel>(),
                IsActive = exam.IsActive
            };

            // 4️⃣ جلب الاختيارات لكل سؤال
            foreach (var q in questionDtos)
            {
                var choiceDtos = await _choiceRepo.GetByQuestionId(q.Id);

                var questionVm = new QuestionViewModel
                {
                    Id = q.Id,
                    Text = q.QuestionText ?? string.Empty,
                    Choices = choiceDtos.Select(c => new ChoiceViewModel
                    {
                        Id = c.Id,
                        Text = c.ChoiceText ?? string.Empty,
                        IsCorrect = c.IsCorrect,
                        IsActive = c.IsActive
                    }).ToList(),
                    IsActive = q.IsActive
                };

                examVm.Questions.Add(questionVm);
            }

            return examVm;
        }

        public async Task ToggleActive(Guid examId)
        {
            var exam = await _redo.GetById(examId);
            if (exam == null) throw new Exception("Exam not found.");

            exam.IsActive = !exam.IsActive; // ✅ تبديل الحالة
            await _redo.Update(exam);
            await _uow.CommitAsync();
        }

        public async Task<bool> Disable(Guid examId)
        {
            var exam = await _uow.Repository<TbExam>().GetById(examId);
            if (exam == null) return false;

            // عطّل الامتحان
            exam.IsActive = false;
            await _uow.Repository<TbExam>().Update(exam);

            // هات الأسئلة
            var questions = await _questionRepo.GetByExamId(examId);

            foreach (var q in questions)
            {
                var questionEntity = await _uow.Repository<TbQuestion>().GetById(q.Id);
                if (questionEntity != null)
                {
                    questionEntity.IsActive = false;
                    await _uow.Repository<TbQuestion>().Update(questionEntity);
                }

                // هات الاختيارات
                var choices = await _choiceRepo.GetByQuestionId(q.Id);
                foreach (var c in choices)
                {
                    var choiceEntity = await _uow.Repository<TbChoice>().GetById(c.Id);
                    if (choiceEntity != null)
                    {
                        choiceEntity.IsActive = false;
                        await _uow.Repository<TbChoice>().Update(choiceEntity);
                    }
                }
            }

            await _uow.CommitAsync();
            return true;
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


        

        public async Task<ExamWithQuestionsViewModel> Edit(Guid examId, ExamWithQuestionsViewModel model)
        {
            if (model == null)
                throw new ArgumentException("Invalid data to update exam.");

            await _uow.BeginTransactionAsync();
            try
            {
                // 1️⃣ تحديث بيانات الامتحان
                var existingExam = await _uow.Repository<TbExam>().GetById(examId);
                if (existingExam == null)
                    throw new Exception("Exam not found.");

                _mapper.Map(model, existingExam);
                existingExam.UpdatedDate = DateTime.Now;
                await _uow.Repository<TbExam>().Update(existingExam);

                // 2️⃣ جلب كل الأسئلة والاختيارات من DB
                var existingQuestions = await _questionRepo.GetByExamId(examId); // TbQuestionDto
                var existingChoices = await _choiceRepo.GetAll();                 // TbChoiceDto

                // 3️⃣ معالجة الأسئلة
                foreach (var questionVm in model.Questions)
                {
                    TbQuestionDto questionDto;

                    if (questionVm.Id.HasValue) // تحديث سؤال موجود
                    {
                        questionDto = existingQuestions.FirstOrDefault(q => q.Id == questionVm.Id.Value);
                        if (questionDto == null)
                            throw new Exception("Question not found for update.");

                        _mapper.Map(questionVm, questionDto);
                        await _questionRepo.Update(questionDto);
                    }
                    else // إضافة سؤال جديد
                    {
                        questionDto = _mapper.Map<TbQuestionDto>(questionVm);
                        questionDto.Id = Guid.NewGuid();
                        questionDto.ExamId = examId;
                        questionDto.CreatedDate = DateTime.Now;
                        questionDto.CurrentState = 1;

                        await _questionRepo.Add(questionDto);
                    }

                    // 4️⃣ معالجة الاختيارات
                    var incomingChoiceIds = questionVm.Choices
                        .Where(c => c.Id.HasValue)
                        .Select(c => c.Id.Value)
                        .ToList();

                    // حذف الاختيارات الغير موجودة
                    var choicesToDelete = existingChoices
                        .Where(c => c.QuestionId == questionDto.Id && !incomingChoiceIds.Contains(c.Id))
                        .ToList();

                    foreach (var choice in choicesToDelete)
                        await _choiceRepo.ChangeStatus(choice.Id, Guid.Empty, 0); // Soft Delete

                    // تحديث / إضافة الاختيارات
                    foreach (var choiceVm in questionVm.Choices)
                    {
                        if (choiceVm.Id.HasValue) // تحديث
                        {
                            var choiceDto = existingChoices.FirstOrDefault(c => c.Id == choiceVm.Id.Value);
                            if (choiceDto != null)
                            {
                                choiceDto.ChoiceText = choiceVm.Text; // مطابق لاسم Property في DTO
                                choiceDto.IsCorrect = choiceVm.IsCorrect;
                                await _choiceRepo.Update(choiceDto);
                            }
                        }
                        else // إضافة جديد
                        {
                            var newChoiceDto = new TbChoiceDto
                            {
                                Id = Guid.NewGuid(),
                                QuestionId = questionDto.Id,
                                ChoiceText = choiceVm.Text,
                                IsCorrect = choiceVm.IsCorrect,
                                CreatedDate = DateTime.Now,
                                CurrentState = 1
                            };
                            await _choiceRepo.Add(newChoiceDto);
                        }
                    }
                }

                await _uow.CommitAsync();

                // 5️⃣ جلب الأسئلة والاختيارات بعد الحفظ للـ ViewModel
                var updatedQuestions = await _questionRepo.GetByExamId(examId);
                foreach (var q in updatedQuestions)
                    q.Choices = (await _choiceRepo.GetByQuestionId(q.Id)).ToList();

                model.Questions = updatedQuestions.Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    Text = q.QuestionText, // مطابق لاسم Property في DTO
                    Choices = q.Choices.Select(c => new ChoiceViewModel
                    {
                        Id = c.Id,
                        Text = c.ChoiceText,  // مطابق لاسم Property في DTO
                        IsCorrect = c.IsCorrect
                    }).ToList()
                }).ToList();

                return model; // ترجع النموذج الكامل بعد الحفظ
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
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



        public async Task<bool> DeleteQuestionWithChoicesAsync(Guid questionId)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                // 1. جلب السؤال
                var question = await _questionRepo.GetById(questionId);
                if (question == null)
                    return false;

                // 2. جلب كل الاختيارات الخاصة بالسؤال
                var choices = await _choiceRepo.GetByQuestionId(questionId);

                // 3. حذف الاختيارات أولاً (Soft Delete)
                foreach (var choice in choices)
                {
                    await _choiceRepo.ChangeStatus(choice.Id, Guid.Empty, 0); // Soft Delete
                }

                // 4. حذف السؤال نفسه (Soft Delete)
                await _questionRepo.ChangeStatus(questionId, Guid.Empty, 0);

                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteChoiceAsync(Guid choiceId)
        {
            try
            {
                var choice = await _choiceRepo.GetById(choiceId);
                if (choice == null)
                    return false;

                // مسح الاختيار (Soft Delete)
                await _choiceRepo.ChangeStatus(choiceId, Guid.Empty, 0);
                return true;
            }
            catch
            {
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





