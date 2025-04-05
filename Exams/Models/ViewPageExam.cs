
using BL.Dtos;
using Domin;

namespace Exams.Models
{
    public class ViewPageExam
    {

        
            public ViewPageExam()
            {
                Questions = new List<TbQuestionDto>();
            Choices = new List<TbChoiceDto>();
            }

            public TbExamDto Exam { get; set; }  // امتحان واحد فقط
            public List<TbQuestionDto> Questions { get; set; }  // قائمة الأسئلة داخل الامتحان
        public List<TbChoiceDto> Choices { get; set; }       
            
    }




}
