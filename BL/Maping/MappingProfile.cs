using AutoMapper;
using BL.Dtos;
using Domin;
using Exams.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Maping
{
    public  class MappingProfile :Profile
    {
        public MappingProfile() 
        {
            CreateMap<TbExam, TbExamDto>().ReverseMap();
           CreateMap<TbQuestion , TbQuestionDto>().ReverseMap();
            CreateMap<TbChoice, TbChoiceDto>().ReverseMap();
            CreateMap<TbResult, TbResultDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>().ReverseMap();

            // تحويل من RegisterDto إلى ApplicationUser (للتسجيل)
            CreateMap<RegisterDto, ApplicationUser>();

            // تحديث بيانات المستخدم
            CreateMap<UpdateUserDto, ApplicationUser>().ReverseMap();



            // Exam
            CreateMap<ExamWithQuestionsViewModel, TbExam>().ReverseMap();
            CreateMap<TbExam, ExamWithQuestionsViewModel>().ReverseMap();

            // Question
           

            // Choice

            // Question
            CreateMap<TbQuestionDto, TbQuestion>().ReverseMap();
            CreateMap<QuestionViewModel, TbQuestionDto>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Text))
                .ReverseMap()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.QuestionText));

            // Choice
            CreateMap<TbChoiceDto, TbChoice>().ReverseMap();

            CreateMap<ChoiceViewModel, TbChoiceDto>()
                .ForMember(dest => dest.ChoiceText, opt => opt.MapFrom(src => src.Text))
                .ReverseMap()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.ChoiceText));


        }
    }
}
