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
         * FinalGrades list. Returns false if letterGrade is invalid. Also updates student's credit hours if need be.
         */
        public bool SubmitFinalGrade(Student selectedStudent, string letterGrade, int courseId, int courseGradeId)
        {
            if (letterGrade.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("B", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("C", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                // first update student's credit hours if final grade is at least a 'C'
                selectedStudent.FinalGrades.Add(new CourseGrades(courseGradeId, courseId, letterGrade.ToUpper()[0]));
                int index = Course.SearchCourseById(selectedStudent.RegisteredCourses, courseId);
                if (letterGrade.ToUpper() == "A" || letterGrade.ToUpper() == "B" || letterGrade.ToUpper() == "C")
                {
                    selectedStudent.CreditHours += selectedStudent.RegisteredCourses[index].CreditHours;
                    selectedStudent.Level = selectedStudent.CalculateLevel(selectedStudent.CreditHours);
                }

                // now deregister student regardless of what grade received
                selectedStudent.DeregisterCourse(index);
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
