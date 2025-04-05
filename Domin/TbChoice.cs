using Domin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domin;

public partial class TbChoice :BaseTable
{

    public string ChoiceText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
    [ForeignKey("Question")]
    public Guid QuestionId { get; set; }
    public virtual TbQuestion Question { get; set; } = null!;

}
