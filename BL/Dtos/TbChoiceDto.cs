using BL.Dtos.BaseDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class TbChoiceDto : BasDto
    {



        public string ChoiceText { get; set; }= string.Empty;

        public bool IsCorrect { get; set; }
        [ForeignKey("Question")]
        public Guid QuestionId { get; set; }

    }
}
