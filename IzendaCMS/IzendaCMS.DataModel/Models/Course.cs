using System;
using System.Collections.Generic;

namespace IzendaCMS.DataModel.Models
{
    public partial class Course
    {
        public Course()
        {
            this.CourseGrades = new List<CourseGrade>();
            this.Instructor_Course = new List<Instructor_Course>();
            this.Student_Course = new List<Student_Course>();
        }

        public Course(int id, DateTime startDate, DateTime endDate, int hours, string name, string description)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            CreditHours = hours;
            CourseName = name;
            CourseDescription = description;
            this.CourseGrades = new List<CourseGrade>();
            this.Instructor_Course = new List<Instructor_Course>();
            this.Student_Course = new List<Student_Course>();
        }

        public int Id { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int CreditHours { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public virtual ICollection<CourseGrade> CourseGrades { get; set; }
        public virtual ICollection<Instructor_Course> Instructor_Course { get; set; }
        public virtual ICollection<Student_Course> Student_Course { get; set; }

        public override string ToString()
        {
            return $"Course ID: {Id}\nStart Date: {StartDate}\nEnd Date: {EndDate}\nCredit Hours: {CreditHours}\nCourse Name: {CourseName}\nCourse Description: {CourseDescription}\n";
        }
    }
}
