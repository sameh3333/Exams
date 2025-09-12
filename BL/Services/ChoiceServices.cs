using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using Domin;
using Exams.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class ChoiceServices : BaseServices<TbChoice, TbChoiceDto>, IChoice
    {
        private readonly IGenericRepository<TbChoice> _repo;
        private readonly AutoMapper.IMapper _mapper;
        public ChoiceServices(IGenericRepository<TbChoice> repo, AutoMapper.IMapper mapper, IUserServices userservices)
             : base(repo, mapper, userservices)
        {
            _repo = repo;
            _mapper = mapper;
        }


        // جلب الاختيارات الخاصة بسؤال معين
        public async Task<List<TbChoiceDto>> GetByQuestionId(Guid questionId)
        {
            
            var allChoices = await _repo.GetAll();

            // بعد ما نحصل على القائمة، نفلتر حسب QuestionId
            var choices = allChoices.Where(c => c.QuestionId == questionId).ToList();

            // نحول النتيجة إلى DTO
            return _mapper.Map<List<TbChoice>, List<TbChoiceDto>>(choices);
        }

        // حفظ أو تحديث اختيار


       
        public async Task<TbChoiceDto> EditChoice(Guid? id, Guid questionId)
        {
            if (id.HasValue && id != Guid.Empty)
            {
                var entity = await  _repo.GetById(id.Value);
                return   _mapper.Map<TbChoice, TbChoiceDto>(entity);
            }

            return new TbChoiceDto { QuestionId = questionId };
        }


   

        public async Task<bool> SaveChoic(TbChoiceDto choiceDto)
        {
            try
            {
                if (choiceDto.IsCorrect)
                {
                    // ننتظر الحصول على كل الاختيارات أولاً
                    var allChoices = await GetByQuestionId(choiceDto.QuestionId);

                    // فلترة الاختيارات الأخرى
                    var otherChoices = allChoices.Where(c => c.Id != choiceDto.Id).ToList();

                    foreach (var choice in otherChoices)
                    {
                        if (choice.IsCorrect)
                        {
                            choice.IsCorrect = false;

                            // نفترض أن Update async
                            await Update(choice);
                        }
                    }
                }

                if (choiceDto.Id == Guid.Empty)
                {
                    // إضافة اختيار جديد
                    return await Add(choiceDto);
                }
                else
                {
                    // تحديث اختيار موجود
                    var existingChoice = await GetById(choiceDto.Id);
                    if (existingChoice == null)
                        return false;

                    existingChoice.ChoiceText = choiceDto.ChoiceText;
                    existingChoice.IsCorrect = choiceDto.IsCorrect;

                    return await Update(existingChoice);
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
