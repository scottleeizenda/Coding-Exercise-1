using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCourseManagementSystem
{
    public class Course
    {
        // Declarations & Getters/Setters
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreditHours { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public List<Student> RegisteredStudents { get; set; } // this assumes one set of students per course

        public Course(int id, DateTime startDate, DateTime endDate, int creditHours, string courseName, string courseDescription)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            CreditHours = creditHours;
            CourseName = courseName;
            CourseDescription = courseDescription;
            RegisteredStudents = new List<Student>();
        }

        /**
         * Searches through param courses and returns the index of the course that matches param id.
         * Returns -1 if not found. Returns -2 if courses list is empty.
         */
        public static int SearchCourseById(List<Course> courses, int id)
        {
            if (!courses.Any())
            {
                return -2;
            }

            for (int i = 0; i < courses.Count; i++)
            {
                if (courses[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * 
         */
        public int SearchStudentById(int id)
        {
            for (int i = 0; i < RegisteredStudents.Count; i++)
            {
                if (RegisteredStudents[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * 
         */
        public bool ViewRegisteredStudents()
        {
            if (!RegisteredStudents.Any())
            {
                Console.WriteLine("No registered students");
                Console.WriteLine("-----------------------------------------------------------------------------");
                return false;
            }

            Console.WriteLine("-----------------------------------------------------------------------------");
            for (int i = 0; i < RegisteredStudents.Count; i++)
            {
                Console.WriteLine(RegisteredStudents[i]);
            }
            Console.WriteLine("-----------------------------------------------------------------------------");
            return true;
        }

        public override string ToString()
        {
            return $"Course ID: {Id}\nStart Date: {StartDate}\nEnd Date: {EndDate}\nCredit Hours: {CreditHours}\nCourse Name: {CourseName}\nCourse Description: {CourseDescription}\n";
        }
    }
}
