using BL.Contracts;
using BL.Dtos;
using Domin;
using System.ComponentModel.DataAnnotations;

namespace BL.Dtos
{
  
        public class ExamWithQuestionsViewModel
        {
        [Required(ErrorMessage = "Exam Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [MinLength(1, ErrorMessage = "At least one question is required")]
        public List<QuestionViewModel> Questions { get; set; } = new();
        }

        public class QuestionViewModel
        {

        [Required(ErrorMessage = "Question text is required")]
        public string Text { get; set; }
        [MinLength(2, ErrorMessage = "Each question must have at least 2 choices")]
        public List<ChoiceViewModel> Choices { get; set; } = new();
        }

        public class ChoiceViewModel
        {
        [Required(ErrorMessage = "Choice text is required")]
        public string Text { get; set; }
            public bool IsCorrect { get; set; }
        }

    }

