using BL.Dtos;
using Domin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IChoice : IBaseServices<TbChoice, TbChoiceDto>
    {
       Task< List<TbChoiceDto>> GetByQuestionId(Guid questionId);
        Task< TbChoiceDto> EditChoice(Guid? id, Guid questionId);
        Task<bool>   SaveChoic(TbChoiceDto choiceDto);
    }
}
