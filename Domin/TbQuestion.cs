using System;
using System.Collections.Generic;

namespace Domin;

public partial class TbQuestion : BaseTable
{

    public string QuestionText { get; set; } = string.Empty;

    public Guid ExamId { get; set; }

        public bool IsActive { get; set; } = true; // ✅ جديد لتعطيل الامتحان


    public virtual TbExam Exam { get; set; } = null!;
  
    public virtual ICollection<TbChoice> TbChoices { get; set; } = new List<TbChoice>();
}
