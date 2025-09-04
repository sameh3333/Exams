using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.BaseDto;

namespace BL.Dtos
{
    public  class TbQuestionDto : BasDto
    {
        public bool IsActive { get; set; } = true; // ✅ جديد لتعطيل الامتحان

        [Required(ErrorMessage = "Question text is required.")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Question must be between 5 and 500 characters.")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "ExamId is required.")]
        public Guid ExamId { get; set; }

        public List<TbChoiceDto> Choices { get; set; } = new List<TbChoiceDto>();
    }
}
