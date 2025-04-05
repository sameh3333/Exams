using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Dtos.BaseDto;

namespace BL.Dtos
{
    public  class TbResultDto : BasDto
    {
        public string StudentName { get; set; } = null!;

        public Guid ExamId { get; set; }

        public int Score { get; set; }

        public DateTime? TakenDate { get; set; }
    }
}
