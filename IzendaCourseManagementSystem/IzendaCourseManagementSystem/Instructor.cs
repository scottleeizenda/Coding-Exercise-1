using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCourseManagementSystem
{
    public class Instructor : User
    {
        // Extra declarations from a User
        public DateTime HireDate { get; set; }
        public List<Course> AssignedCourses { get; set; }

        public Instructor(int id, string firstName, string lastName, DateTime hireDate, string userName, string password)
            : base (id, firstName, lastName, userName, password)
        {
            HireDate = hireDate;
            UserType = "Instructor";
            AssignedCourses = new List<Course>();
        }

        /**
         * Displays all courses that an Administrator assigned to an Instructor.
         * If the list is empty, returns false. Otherwise, successfully displays and returns true.
         * BEWARE, NO PARAMS PASSED IN. Maybe think about restructuring later.
         */
        public bool ViewAssignedCourses()
        {
            if (!AssignedCourses.Any())
            {
                return false;
            }

            Console.WriteLine("You currently teach:\n");
            foreach (Course c in AssignedCourses)
            {
                Console.WriteLine(c);
            }
            return true;
        }

        /**
         * Searches through AssignedCourses list and returns the index of the course that matches param id.
         * Returns -1 if not found. Check for empty list already done before method call.
         */
        public int SearchAssignedCourseById(int id)
        {
            for (int i = 0; i < AssignedCourses.Count; i++)
            {
                if (AssignedCourses[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Checks if param letterGrade is a valid grade, then adds a new CourseGrade to param selectedStudent's
         * FinalGrades list. Returns false if letterGrade is invalid.
         */
        public bool SubmitFinalGrade(Student selectedStudent, string letterGrade, int courseId, int courseGradeId)
        {
            if (letterGrade.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("B", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("C", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                //selectedStudent.FinalGrades.Add(new CourseGrades(courseGradeId, courseId, letterGrade.ToUpper().ToCharArray()[0]));
                selectedStudent.FinalGrades.Add(new CourseGrades(courseGradeId, courseId, letterGrade.ToUpper()[0]));
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"Instructor ID: {Id}\nName: {LastName}, {FirstName}\nHire Date: {HireDate}\nUser Name: {UserName}\nPassword: {Password}\n";
        }
    }
}
