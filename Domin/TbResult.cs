using System;
using System.Collections.Generic;

namespace Domin;

public partial class TbResult : BaseTable
{

    public string StudentName { get; set; } = null!;

    public Guid ExamId { get; set; }

    public int Score { get; set; }

    public DateTime? TakenDate { get; set; }

    
    public virtual TbExam Exam { get; set; } = null!;
}
