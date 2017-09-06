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
        public List<CourseGrades> FinalGrades { get; set; }

        public Student(int id, string firstName, string lastName, string userName, string password, float gpa, int creditHours)
            : base(id, firstName, lastName, userName, password)
        {
            GPA = gpa;
            CreditHours = creditHours;
            Level = CalculateLevel(creditHours);
            RegisteredCourses = new List<Course>();
            FinalGrades = new List<CourseGrades>();
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

        /**
         * Displays all courses the Student is registered for from the RegisteredCourses list.
         * If the list is empty, returns false. Otherwise, successfully displays and returns true.
         * BEWARE, NO PARAMS PASSED IN. Maybe think about restructuring later.
         */
        public bool ViewRegisteredCourses()
        {
            if (!RegisteredCourses.Any())
            {
                return false;
            }

            foreach (Course c in RegisteredCourses)
            {
                Console.WriteLine(c);
            }
            return true;
        }

        /**
         * Displays all CourseGrades the student has earned.
         * If the list is empty, returns false. Otherwise, successfully displays and returns true.
         * BEWARE, NO PARAMS PASSED IN. Maybe think about restructuring later.
         */
        public bool ViewFinalGrades()
        {
            if (!FinalGrades.Any())
            {
                return false;
            }

            foreach (CourseGrades cg in FinalGrades)
            {
                Console.WriteLine(cg);
            }
            return true;
        }

        /**
         * Adds the Course specified in the params to the RegisteredCourses list.
         * Check for valid course and index done before this method call.
         */
        public bool RegisterCourse(List<Course> courses, int index)
        {
            RegisteredCourses.Add(courses[index]);
            return true;
        }

        /**
         * Searches through the RegisteredCourses list and returns the index of the course
         * that matches param id. Returns -1 if not found. Returns -2 if list is empty.
         */
        public int SearchRegisteredCourse(int id)
        {
            if (!RegisteredCourses.Any())
            {
                return -2;
            }

            for (int i = 0; i < RegisteredCourses.Count; i++)
            {
                if (RegisteredCourses[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Adds the Course specified in the params to the RegisteredCourses list.
         * Check for valid course and index done before this method call.
         */
        public bool DeregisterCourse(int index)
        {
            RegisteredCourses.RemoveAt(index);
            return true;
        }

        public override string ToString()
        {
            return $"Student Id: {Id}\nName: {LastName}, {FirstName}\nCredit Hours Earned: {CreditHours}\nGPA: {GPA}\nGrade Level: {Level}\n";
        }
    }
}
