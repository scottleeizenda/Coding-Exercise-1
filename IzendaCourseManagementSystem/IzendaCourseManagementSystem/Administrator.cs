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
        
         /// <summary>
         ///    Checks if all fields in courseFields are valid, then proceeds to create and add a
         ///    new Course in the parameter courses list.
         /// </summary>
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

        /// <summary>
        ///     Checks if all fields in courseFields are valid, where a blank ("") element means to
        ///     keep that field the same as before. Otherwise, return false if an invalid field is
        ///     detected or return true if all is good.
        /// </summary>
        public bool UpdateCourse(List<Course> courses, int courseIndex, string[] courseFields)
        {
            int hours;
            DateTime startDate, endDate;

            // check all fields' validity first to ensure no half-way updates, return false upon an invalid input field

            // if blank courseField detected, keep field same as the one before. Otherwise, check for validity of non-string fields
            if (courseFields[0] == "")
            {
                startDate = courses[courseIndex].StartDate;
            }
            else if (!DateTime.TryParse(courseFields[0], out startDate))
            {
                return false;
            }
            if (courseFields[1] == "")
            {
                endDate = courses[courseIndex].EndDate;
            }
            else if (!DateTime.TryParse(courseFields[1], out endDate))
            {
                return false;
            }
            if (courseFields[2] == "")
            {
                hours = courses[courseIndex].CreditHours;
            }
            else if (!Int32.TryParse(courseFields[2], out hours))
            {
                return false;
            }
            // string fields don't need the TryParse
            if (courseFields[3] == "")
            {
                courseFields[3] = courses[courseIndex].CourseName;
            }
            if (courseFields[4] == "")
            {
                courseFields[4] = courses[courseIndex].CourseDescription;
            }

            courses[courseIndex].StartDate = startDate;
            courses[courseIndex].EndDate = endDate;
            courses[courseIndex].CreditHours = hours;
            courses[courseIndex].CourseName = courseFields[3];
            courses[courseIndex].CourseDescription = courseFields[4];
            return true;
        }
        
        /// <summary>
        ///     Returns true if course successfully deleted. Error checking done in AdminActionHandler.
        ///     This method may be expanded later to delete from a database which will do more then.
        /// </summary>
        public bool DeleteCourse(List<Course> courses, int courseIndex)
        {
            courses.RemoveAt(courseIndex);
            return true;
        }
        
        /// <summary>
        ///     Displays all available Instructors that an Administrator can assign a Course to.
        ///     Returns false if list is empty. Otherwise, successfully displays and returns true.
        /// </summary>
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
        
        /// <summary>
        ///     Searches through param instructors list and returns the index of the instructor that has
        ///     an ID matching with param id. Returns -1 if not found.
        /// </summary>
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

        /// <summary>
        ///     Assigns an Instructor to a Course by adding the Course to an instructor's AssignedCourses list.
        ///     This method may be expanded later by inserting in Instructor-Course associative table for a database.
        /// </summary>
        public bool AssignInstructor(Course selectedCourse, Instructor selectedInstructor)
        {
            selectedInstructor.AssignedCourses.Add(selectedCourse);
            return true;
        }

        /// <summary>
        ///     Handles an Administrator action specified by user input from Main. Database not yet implemented,
        ///     so data comes from Lists created in Main. The check for a valid action number also done in Main.
        ///     Action #1 = Creating a course (Database INSERT)
        ///     Action #2 = View all courses (Database SELECT, may expand later to view specific courses)
        ///     Action #3 = Edit a course (Database UPDATE)
        ///     Action #4 = Delete a course (Database DELETE)
        ///     Action #5 = Assign an Instructor to teach a Course (will also later deal with a database)
        /// </summary>
        /// <param name="courses">List of all existing courses</param>
        /// <param name="instructors">List of all available instructors</param>
        /// <param name="action">Number to represent what course of action for an Administrator to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public bool AdminActionHandler(List<Course> courses, List<Instructor> instructors, int action)
        {
            if (action == 1) // create course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                string[] courseFields = new string[6];
                Console.Write("Enter the course ID: ");
                courseFields[0] = Console.ReadLine();
                Console.Write("Enter the course start date: ");
                courseFields[1] = Console.ReadLine();
                Console.Write("Enter the course end date: ");
                courseFields[2] = Console.ReadLine();
                Console.Write("Enter the course credit hours: ");
                courseFields[3] = Console.ReadLine();
                Console.Write("Enter the course name: ");
                courseFields[4] = Console.ReadLine();
                Console.Write("Enter the course description: ");
                courseFields[5] = Console.ReadLine();

                bool status = this.CreateCourse(courses, courseFields);
                if (status)
                {
                    Console.WriteLine("Course successfully created.");
                }
                else
                {
                    Console.WriteLine("Failed to create course.");
                }
                Console.WriteLine("-----------------------------------------------------------------------------");
                return status;
            }
            else if (action == 2) // view courses
            {
                bool status = this.ViewCourses(courses);
                if (!status)
                {
                    Console.WriteLine("There are currently no existing courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }
                return status;
            }
            else if (action == 3) // edit course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to edit: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    int index = Course.SearchCourseById(courses, id);
                    if (index >= 0)
                    {
                        Console.WriteLine($"Course Successfully Found:\n{courses[index]}");
                        Console.WriteLine("[Leave a field blank if you wish to keep it the same as before]");
                        string[] courseFields = new string[5];
                        Console.Write("Enter the new course start date: ");
                        courseFields[0] = Console.ReadLine();
                        Console.Write("Enter the new course end date: ");
                        courseFields[1] = Console.ReadLine();
                        Console.Write("Enter the new course credit hours: ");
                        courseFields[2] = Console.ReadLine();
                        Console.Write("Enter the new course name: ");
                        courseFields[3] = Console.ReadLine();
                        Console.Write("Enter the new course description: ");
                        courseFields[4] = Console.ReadLine();

                        bool status = this.UpdateCourse(courses, index, courseFields);
                        if (status)
                        {
                            Console.WriteLine("Course successfully updated.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to update course.");
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return status;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id}");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
            }
            else if (action == 4) // delete course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to delete: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    int index = Course.SearchCourseById(courses, id);
                    if (index >= 0)
                    {
                        Console.WriteLine($"Course Successfully Found:\n{courses[index]}");
                        Console.WriteLine("Are you sure you would like to delete this course?");
                        Console.Write("[Enter Y/N]: ");
                        string choice = Console.ReadLine();
                        if (choice.Equals("Y", StringComparison.OrdinalIgnoreCase) || choice.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                        {
                            bool status = this.DeleteCourse(courses, index);
                            Console.WriteLine($"Successfully deleted course with ID {id}");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return status; // should always be true at this point (for now)
                        }
                        else if (choice.Equals("N", StringComparison.OrdinalIgnoreCase) || choice.Equals("No", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Cancelling deletion of course with ID {id}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice, returning to menu...");
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false; // if no successful delete occurs, return false no matter what
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id}");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
            }
            else // assign instructor
            {
                // Check for empty instructor or course lists
                if (instructors.Count == 0)
                {
                    Console.WriteLine("There are currently no available instructors.");
                    return false;
                }
                if (courses.Count == 0)
                {
                    Console.WriteLine("There are currently no existing courses.");
                    return false;
                }

                Instructor selectedInstructor;
                Course selectedCourse;
                // loop for user input on selecting an instructor
                while (true)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Type the ID of the instructor you would like to assign a course to");
                    Console.WriteLine("[Enter 'list' to see all the available instructors]");
                    Console.Write("[Enter 'quit' to cancel]: ");
                    string input = Console.ReadLine();
                    int id;
                    if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ViewInstructors(instructors);
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        int index = this.SearchInstructorById(instructors, id);
                        if (index >= 0)
                        {
                            selectedInstructor = instructors[index];
                            Console.WriteLine($"Instructor {selectedInstructor.FirstName} {selectedInstructor.LastName} successfully found.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            // loop for user input on selecting a course to assign
                            while (true)
                            {
                                Console.WriteLine("Type the ID of the course you would like to assign to this instructor");
                                Console.WriteLine("[Enter 'list' to see all the available courses]");
                                Console.Write("[Enter 'quit' to cancel]: ");
                                input = Console.ReadLine();
                                if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                                {
                                    this.ViewCourses(courses);
                                }
                                else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Cancelled, returning to instructor select...");
                                    break;
                                }
                                else if (Int32.TryParse(input, out id))
                                {
                                    index = Course.SearchCourseById(courses, id);
                                    if (index >= 0)
                                    {
                                        selectedCourse = courses[index];
                                        Console.WriteLine($"Course {selectedCourse.CourseName} successfully found.");
                                        bool status = this.AssignInstructor(selectedCourse, selectedInstructor);
                                        if (status)
                                        {
                                            Console.WriteLine($"Instructor {selectedInstructor.FirstName} {selectedInstructor.LastName} has been assigned to teach course {selectedCourse.CourseName}");
                                            Console.WriteLine("-----------------------------------------------------------------------------");
                                            return true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Failed to assign {selectedInstructor.FirstName} {selectedInstructor.LastName} to teach the course {selectedCourse.CourseName}");
                                            Console.WriteLine("-----------------------------------------------------------------------------");
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to find course with ID {id}");
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again.");
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to find instructor with ID {id}");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }
        }
    }
}
