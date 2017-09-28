using System;
using System.Collections.Generic;

namespace IzendaCMS.DataModel.Models
{
    public partial class Student_CourseGrades
    {
        public Student_CourseGrades() { }

        public Student_CourseGrades(int id, int studentId, int courseGradesId)
        {
            Id = id;
            StudentId = studentId;
            CourseGradesId = courseGradesId;
        }

        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseGradesId { get; set; }
        public virtual CourseGrade CourseGrade { get; set; }
        public virtual Student Student { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}\nStudent ID: {StudentId}\nCourseGrade ID: {CourseGradesId}\n";
        }
    }
}
