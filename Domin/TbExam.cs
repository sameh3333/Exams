using Domin;
using System;
using System.Collections.Generic;

namespace Domin;

public partial class TbExam : BaseTable
{

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true; // ✅ جديد لتعطيل الامتحان



    public virtual ICollection<TbQuestion> TbQuestions { get; set; } = new List<TbQuestion>();

    public virtual ICollection<TbResult> TbResults { get; set; } = new List<TbResult>();
}
