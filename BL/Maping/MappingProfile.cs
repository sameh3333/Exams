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

        }
    }
}
