using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCourseManagementSystem
{
    public class Administrator : User
    {
        // Extra declarations from a User
        public DateTime HireDate { get; set; }

        public Administrator(int id, string firstName, string lastName, DateTime hireDate, string userName, string password)
            : base(id, firstName, lastName, userName, password)
        {
            HireDate = hireDate;
            UserType = "Administrator";
        }

        /**
         * 
         */
        public bool CreateCourse(List<Course> courses, string[] courseFields)
        {
            int id, hours;
            DateTime startDate, endDate;

            // return false upon an invalid input field
            if (!Int32.TryParse(courseFields[0], out id))
            {
                return false;
            }
            if (!DateTime.TryParse(courseFields[1], out startDate))
            {
                return false;
            }
            if (!DateTime.TryParse(courseFields[2], out endDate))
            {
                return false;
            }
            if (!Int32.TryParse(courseFields[3], out hours))
            {
                return false;
            }

            courses.Add(new Course(id, startDate, endDate, hours, courseFields[4], courseFields[5]));
            return true;
        }

        public bool UpdateCourse(List<Course> courses, int courseIndex, string[] courseFields)
        {
            int hours;
            DateTime startDate, endDate;

            // return false upon an invalid input field
            // check all fields' validity first to ensure no half-way updates
            if (!DateTime.TryParse(courseFields[0], out startDate))
            {
                return false;
            }
            if (!DateTime.TryParse(courseFields[1], out endDate))
            {
                return false;
            }
            if (!Int32.TryParse(courseFields[2], out hours))
            {
                return false;
            }

            courses[courseIndex].StartDate = startDate;
            courses[courseIndex].EndDate = endDate;
            courses[courseIndex].CreditHours = hours;
            courses[courseIndex].CourseName = courseFields[3];
            courses[courseIndex].CourseDescription = courseFields[4];
            return true;
        }

        /**
         * Returns true if course successfully deleted. Error checking already done before.
         * This method may be expanded to delete from a database.
         */
        public bool DeleteCourse(List<Course> courses, int courseIndex)
        {
            courses.RemoveAt(courseIndex);
            return true;
        }

        /**
         * Displays all Instructors that an Administrator assigned a Course to.
         * If the list is empty, returns false. Otherwise, successfully displays and returns true.
         */
        public bool ViewInstructors(List<Instructor> instructors)
        {
            if (!instructors.Any())
            {
                return false;
            }

            foreach(Instructor i in instructors)
            {
                Console.WriteLine(i);
            }
            return true;
        }

        /**
         * Searches through param courses and returns the index of the course that matches param id.
         * Returns -1 if not found.
         */
        public int SearchInstructorById(List<Instructor> instructors, int id)
        {
            for (int i = 0; i < instructors.Count; i++)
            {
                if (instructors[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * 
         */
        public bool AssignInstructor(Course selectedCourse, Instructor selectedInstructor)
        {
            selectedInstructor.AssignedCourses.Add(selectedCourse);
            return true;
        }
    }
}
