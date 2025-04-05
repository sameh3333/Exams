using Domin;
using System;
using System.Collections.Generic;

namespace Domin;

public partial class TbExam : BaseTable
{

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

 

    public virtual ICollection<TbQuestion> TbQuestions { get; set; } = new List<TbQuestion>();

    public virtual ICollection<TbResult> TbResults { get; set; } = new List<TbResult>();
}
