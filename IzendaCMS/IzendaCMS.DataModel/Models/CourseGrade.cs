using System;
using System.Collections.Generic;

namespace IzendaCMS.DataModel.Models
{
    public partial class CourseGrade
    {
        public CourseGrade()
        {
            this.Student_CourseGrades = new List<Student_CourseGrades>();
        }

        public CourseGrade(int id, int courseId, string finalGrade)
        {
            Id = id;
            CourseId = courseId;
            FinalGrade = finalGrade;
            this.Student_CourseGrades = new List<Student_CourseGrades>();
        }

        public int Id { get; set; }
        public int CourseId { get; set; }
        public string FinalGrade { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Student_CourseGrades> Student_CourseGrades { get; set; }
    }
}
