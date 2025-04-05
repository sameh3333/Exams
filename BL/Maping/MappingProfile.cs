using AutoMapper;
using BL.Dtos;
using Domin;
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
             
        }
    }
}
