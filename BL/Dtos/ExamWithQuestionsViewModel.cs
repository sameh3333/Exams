using BL.Contracts;
using BL.Dtos;
using Domin;
using System.ComponentModel.DataAnnotations;

namespace BL.Dtos
{
  
        public class ExamWithQuestionsViewModel
        {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Exam Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [MinLength(1, ErrorMessage = "At least one question is required")]
        public List<QuestionViewModel> Questions { get; set; } = new();


        public bool IsActive { get; set; } = true; // ✅ جديد لتعطيل الامتحان
    


}

        public class QuestionViewModel
        {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "Question text is required")]
        public string Text { get; set; }
        [MinLength(2, ErrorMessage = "Each question must have at least 2 choices")]
        public List<ChoiceViewModel> Choices { get; set; } = new();
        public bool IsActive { get; set; } = true; // ✅ جديد لتعطيل الامتحان

    }

    public class ChoiceViewModel
        {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "Choice text is required")]
        public string Text { get; set; }
            public bool IsCorrect { get; set; }
        public bool IsActive { get; set; } = true; // ✅ جديد لتعطيل الامتحان

    }

}


