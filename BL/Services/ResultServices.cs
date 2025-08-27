using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using BL.Maping;
using Domin;
using Exams.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public  class ResultServices : BaseServices < TbResult,TbResultDto>, IResult
    {



        public ResultServices(IGenericRepository<TbResult> redo, AutoMapper.IMapper mapper, IUserServices userservices) : base(redo, mapper, userservices) {
        
        
        }

       
    }
}
