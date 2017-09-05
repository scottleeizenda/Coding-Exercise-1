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

            foreach (Course c in AssignedCourses)
            {
                Console.WriteLine(c);
            }
            return true;
        }
    }
}
