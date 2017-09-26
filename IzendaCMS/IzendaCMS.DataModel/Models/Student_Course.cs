using System;
using System.Collections.Generic;

namespace IzendaCMS.DataModel.Models
{
    public partial class Student_Course
    {
        public Student_Course() { }
        public Student_Course(int id, int studentId, int courseId)
        {
            Id = id;
            StudentId = studentId;
            CourseId = courseId;
        }

        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
