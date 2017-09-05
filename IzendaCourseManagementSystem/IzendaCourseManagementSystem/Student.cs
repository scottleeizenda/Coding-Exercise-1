using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCourseManagementSystem
{
    public class Student : User
    {
        // Extra declarations from a User
        public float GPA { get; set; }
        public int CreditHours { get; set; }
        public string Level { get; set; }
        public List<Course> RegisteredCourses { get; set; }

        public Student(int id, string firstName, string lastName, string userName, string password, float gpa, int creditHours)
            : base(id, firstName, lastName, userName, password)
        {
            GPA = gpa;
            CreditHours = creditHours;
            Level = CalculateLevel(creditHours);
            RegisteredCourses = new List<Course>();
        }

        /**
         * Returns a grade level based on amount of credit hours,
         * and if hours is negative, null is returned.
         */
        public string CalculateLevel(int hours)
        {
            if (hours >= 0 && hours < 30)
            {
                return "Freshman";
            }
            else if (hours >= 30 && hours < 60)
            {
                return "Sophomore";
            }
            else if (hours >= 60 && hours < 90)
            {
                return "Junior";
            }
            else if (hours >= 90)
            {
                return "Senior";
            }
            else
            {
                return null;
            }
        }

        public bool RegisterCourse(List<Course> courses, int index)
        {
            return true;
        }
    }
}
