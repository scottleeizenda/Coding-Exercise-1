using System;
using System.Collections.Generic;

namespace IzendaCMS.DataModel.Models
{
    public partial class Student : User
    {
        public Student()
        {
            this.Student_CourseGrades = new List<Student_CourseGrades>();
            this.Student_Course = new List<Student_Course>();
        }

        //public int Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public double GPA { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
        public int CreditHours { get; set; }
        //public string UserType { get; set; }
        public string Level { get; set; }
        //public Level Level { get; set; }
        public ICollection<Student_CourseGrades> Student_CourseGrades { get; set; }
        public ICollection<Student_Course> Student_Course { get; set; }

        public override string ToString()
        {
            return $"Student Id: {Id}\nName: {LastName}, {FirstName}\nCredit Hours Earned: {CreditHours}\nGPA: {GPA.ToString("0.###")}\nGrade Level: {Level}\n";
        }
    }
}
