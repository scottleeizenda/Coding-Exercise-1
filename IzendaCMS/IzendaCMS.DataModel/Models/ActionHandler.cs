using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCMS.DataModel.Models
{
    public class ActionHandler
    {
        /// <summary>
        ///     Handles an Administrator action specified by user input from Main. Database not yet implemented,
        ///     so data comes from Lists created in Main. The check for a valid action number also done in Main.
        ///     Action #1 = Creating a course (Database INSERT)
        ///     Action #2 = View all courses (Database SELECT, may expand later to view specific courses)
        ///     Action #3 = Edit a course (Database UPDATE)
        ///     Action #4 = Delete a course (Database DELETE)
        ///     Action #5 = Assign an Instructor to teach a Course (will also later deal with a database)
        /// </summary>
        /// <param name="connection">Connection to the database</param>
        /// <param name="action">Number to represent what course of action for an Administrator to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        static bool AdminActionHandler(int action)
        {
            if (action == 1) // create course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                string[] courseFields = Utilities.ReceiveCourseFields(1);

                bool status = Utilities.CreateCourse(courseFields);
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
                Utilities.ViewCoursesHandler();
                return true;
            }
            else if (action == 3) // edit course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to edit: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    Course currentCourse = Utilities.SearchCourseById(null, id);
                    if (currentCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found:\n{currentCourse}");
                        string[] courseFields = Utilities.ReceiveCourseFields(2);

                        bool status = Utilities.UpdateCourse(id, courseFields);
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
                    Course currentCourse = Utilities.SearchCourseById(null, id);
                    if (currentCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found:\n{currentCourse}");
                        Console.WriteLine("Are you sure you would like to delete this course?");
                        Console.Write("[Enter Y/N]: ");
                        string choice = Console.ReadLine();
                        if (choice.Equals("Y", StringComparison.OrdinalIgnoreCase) || choice.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                        {
                            bool status = Utilities.DeleteCourse(id);
                            if (status)
                            {
                                Console.WriteLine($"Successfully deleted course with ID {id}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to delete course with ID {id}.");
                                Console.WriteLine("Some reasons may be:");
                                Console.WriteLine("1) An Instructor is still assigned to teach this course.");
                                Console.WriteLine("2) Student(s) have a CourseGrade for this course.");
                                Console.WriteLine("3) Student(s) are currently registered for this course.");
                            }
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return status;
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
                        return false; // return false upon answering "No" or invalid input
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
                // check for empty Instructor table
                int instructorCourseStatus = Utilities.ViewUsers(2);
                if (instructorCourseStatus == 0)
                {
                    Console.WriteLine("There are currently no instructors to assign a course to.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }

                Instructor selectedInstructor;
                Course selectedCourse;
                // loop for user input on selecting an instructor
                while (true)
                {
                    Console.WriteLine("Type the ID of the instructor you would like to assign a course to");
                    Console.WriteLine("[Enter 'list' to see all the available instructors]");
                    Console.Write("[Enter 'quit' to cancel]: ");
                    string input = Console.ReadLine();
                    int id;
                    if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        Utilities.ViewUsers(2);
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        selectedInstructor = (Instructor)Utilities.SearchUserById(id, 2);
                        if (selectedInstructor != null)
                        {
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
                                    Utilities.ViewCourses(null);
                                }
                                else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Cancelled, returning to instructor select...");
                                    break;
                                }
                                else if (Int32.TryParse(input, out id))
                                {
                                    selectedCourse = Utilities.SearchCourseById(null, id);
                                    if (selectedCourse != null)
                                    {
                                        Console.WriteLine($"Course {selectedCourse.CourseName} successfully found.");
                                        bool status = Utilities.AssignInstructor(selectedCourse, selectedInstructor);
                                        if (status)
                                        {
                                            Utilities.AssignInstructorIdNumber++; // increment ID to keep unique ID's coming
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

        /// <summary>
        ///     Handles an Instructor action specified by user input from Main. Database not yet implemented,
        ///     so data comes from Lists created in Main. The check for a valid action number also done in Main.
        ///     Action #1 = View all courses (Database SELECT, may expand later to view specific courses)
        ///     Action #2 = View all courses the Instructor is assigned to teach (will later deal with a database)
        ///     Action #3 = Submit a final grade for a Student (Deregisters Student from Course upon submission,
        ///                 and updates the Student's CreditHours if final grade is at least a 'C')
        /// </summary>
        /// <param name="action">Number to represent what course of action for an Instructor to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public static bool InstructorActionHandler(int action)
        {
            if (action == 1) // view courses
            {
                Utilities.ViewCoursesHandler();
                return true;
            }
            else if (action == 2) // view assigned
            {
                int status = Utilities.ViewCourses(Utilities.CurrentInstructor.Id, 2);
                if (status == -1) { return false; }
                else { return true; }
            }
            else // submit final grades
            {
                // check if instructor has any courses assigned or error in database operation
                int coursesStatus = Utilities.ViewCourses(Utilities.CurrentInstructor.Id, 2);
                if (coursesStatus == 0)
                {
                    Console.WriteLine("You are not currently assigned to teach any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
                else if (coursesStatus == -1)
                {
                    Console.WriteLine("Failed to display assigned courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }

                // otherwise, proceed to getting user input on course to submit a grade for
                Course selectedCourse;
                Student selectedStudent;
                // query to retrieve full course info of courses assigned to this Instructor
                string assignedCoursesQuery = "SELECT Course.Id, Course.StartDate, Course.EndDate, Course.CreditHours, Course.CourseName, Course.CourseDescription FROM Course " +
                                 $"INNER JOIN Instructor_Course ON Course.Id = Instructor_Course.CourseId WHERE Instructor_Course.InstructorId = {Utilities.CurrentInstructor.Id}";
                //Console.WriteLine("-----------------------------------------------------------------------------");
                while (true)
                {
                    Console.WriteLine("Type the ID of the course you would like to submit final grades for");
                    Console.WriteLine("[Enter 'list' to see all your assigned courses]");
                    Console.Write("[Enter 'quit' to cancel]: ");
                    string input = Console.ReadLine();
                    int id;
                    if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        Utilities.ViewCourses(Utilities.CurrentInstructor.Id, 2);
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        selectedCourse = Utilities.SearchCourseById(assignedCoursesQuery, id);
                        if (selectedCourse != null)
                        {
                            Console.WriteLine($"Course {selectedCourse.CourseName} successfully found.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            // loop for user input on selecting a student to submit a grade for
                            while (true)
                            {
                                // check if there are registered students
                                if (Utilities.ViewRegisteredStudents(Utilities.CurrentStudent.Id) == 0) // check here
                                {
                                    Console.WriteLine("There are currently no students registered for this course.");
                                    Console.WriteLine("-----------------------------------------------------------------------------");
                                    break;
                                }
                                // otherwise, proceed to receive user input of Student to submit grade for
                                Console.WriteLine("Type the ID of the student you would like to submit a final grade for");
                                Console.WriteLine("[Enter 'list' to see all students registered for this course]");
                                Console.Write("[Enter 'quit' to cancel]: ");
                                input = Console.ReadLine();
                                if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                                {
                                    Utilities.ViewRegisteredStudents(Utilities.CurrentStudent.Id);
                                }
                                else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Cancelled, returning to course select...");
                                    Console.WriteLine("-----------------------------------------------------------------------------");
                                    break;
                                }
                                else if (Int32.TryParse(input, out id))
                                {
                                    selectedStudent = (Student)Utilities.SearchUserById(id, 3);
                                    if (selectedStudent != null)
                                    {
                                        Console.WriteLine($"Student {selectedStudent.FirstName} {selectedStudent.LastName} successfully found.");
                                        Console.WriteLine("-----------------------------------------------------------------------------");
                                        Console.Write("What letter grade would you like to give this student for this course?: ");
                                        string letterGrade = Console.ReadLine();
                                        bool status = Utilities.SubmitFinalGrade(selectedStudent, letterGrade, selectedCourse.Id, selectedCourse.CreditHours, Utilities.CourseGradeIdNumber);
                                        if (status)
                                        {
                                            Utilities.CourseGradeIdNumber++; // increment to make unique subsequent CourseGrades.Id's
                                            Console.WriteLine($"Final Grade '{letterGrade.ToUpper()}' has been submitted to for {selectedStudent.FirstName} {selectedStudent.LastName}");
                                            Console.WriteLine("-----------------------------------------------------------------------------");
                                            return true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Failed to submit final grade");
                                            Console.WriteLine("-----------------------------------------------------------------------------");
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to find registered student with ID {id}");
                                        Console.WriteLine("-----------------------------------------------------------------------------");
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again.");
                                    Console.WriteLine("-----------------------------------------------------------------------------");
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to find course with ID {id}");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                    }
                }
            }
        }

        /// <summary>
        ///     Handles a Student action specified by user input from Main. Database not yet implemented,
        ///     so data comes from Lists created in Main. The check for a valid action number also done in Main.
        ///     Action #1 = View all courses (Database SELECT, may expand later to view specific courses)
        ///     Action #2 = View all courses the Student is registered for (will later deal with a database)
        ///     Action #3 = View the Student's information (Uses its ToString method)
        ///     Action #4 = View the Student's final grades (will need database CourseGrades table)
        ///     Action #5 = Register for a Course (adds to RegisteredCourses List and RegisteredStudents List in Course)
        ///     Action #6 = Deregister a Course (removes Course from RegisteredCourses List and Registered Students List in Course)
        /// </summary>
        /// <param name="action">Number to represent what course of action for a Student to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public static bool StudentActionHandler(int action)
        {
            if (action == 1) // view courses
            {
                Utilities.ViewCoursesHandler();
                return true;
            }
            else if (action == 2) // view registered
            {
                int status = Utilities.ViewCourses(Utilities.CurrentStudent.Id, 3);
                if (status == 0)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("You are not currently registered for any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return true;
                }
                else if (status == -1)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Failed to display registered courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
                // else registered courses have successfully displayed
                return true;
            }
            else if (action == 3) // view info
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.WriteLine(Utilities.CurrentStudent);
                Console.WriteLine("-----------------------------------------------------------------------------");
                return true;
            }
            else if (action == 4) // view grades
            {
                int status = Utilities.ViewFinalGrades(Utilities.CurrentStudent.Id);
                if (status == 0)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("You have not completed any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return true;
                }
                else if (status == -1)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Failed to display grades.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
                // else grades have successfully displayed
                return true;
            }
            else if (action == 5) // register course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to register for: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    Course selectedCourse = Utilities.SearchCourseById(null, id);
                    if (selectedCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found, adding this course to your registered courses:\n{selectedCourse}");
                        int status = Utilities.RegisterCourse(Utilities.CurrentStudent.Id, selectedCourse.Id);
                        if (status == 1)
                        {
                            Console.WriteLine("Course successfully registered.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return true;
                        }
                        else if (status == -1)
                        {
                            Console.WriteLine("You are already registered for this course!");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                        else if (status == -2)
                        {
                            Console.WriteLine("You have already completed this course!");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                        else if (status == -3)
                        {
                            Console.WriteLine("Database error, failed to register for course.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                        return false;
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
            else // deregister course
            {
                // query to retrieve full course info of courses this Student is registered for
                //string query = "SELECT Course.Id, Course.StartDate, Course.EndDate, Course.CreditHours, Course.CourseName, Course.CourseDescription FROM Course " +
                //              $"INNER JOIN Student_Course ON Course.Id = Student_Course.CourseId WHERE Student_Course.StudentId = {this.Id}";
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to deregister: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    Course selectedCourse = Utilities.SearchCourseById(null, id); // TODO - edit this
                    if (selectedCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found, attempting to remove this course from your registered courses:\n{selectedCourse}");
                        int status = Utilities.DeregisterCourse(Utilities.CurrentStudent.Id, id);
                        if (status == 1)
                        {
                            Console.WriteLine("Course successfully deregistered.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return true;
                        }
                        else if (status == 0)
                        {
                            Console.WriteLine("You currently have no courses to deregister.");
                        }
                        else if (status == -1)
                        {
                            Console.WriteLine("You are not currently registered for this course.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to deregister course.");
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID '{id}'.");
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
        }

        public static bool UserAction(int option)
        {
            // Display and handle user actions based on type of user logged in
            int action;
            bool actionStatus;
            if (option == 1) // Admin
            {
                Console.WriteLine("Administrator login successful.");
                // repeat indefinitely for admin actions until quit
                while (true)
                {
                    Console.WriteLine("What would you like to do?");
                    Console.WriteLine("[Enter '1' to create a course]");
                    Console.WriteLine("[Enter '2' to view existing courses]");
                    Console.WriteLine("[Enter '3' to edit an existing course]");
                    Console.WriteLine("[Enter '4' to delete an existing course]");
                    Console.WriteLine("[Enter '5' to assign a course to an instructor]");
                    // TODO - consider adding option to view what Instructors are assigned to what Courses
                    // TODO - consider adding option to unassign Instructor from a Course
                    Console.WriteLine("[Enter '6' to quit]");

                    if (Int32.TryParse(Console.ReadLine(), out action))
                    {
                        if (action >= 1 && action <= 5)
                        {
                            actionStatus = AdminActionHandler(action);
                        }
                        else if (action == 6)
                        {
                            Console.WriteLine("Logging out...");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Please enter a number in the options above");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }
            else if (option == 2) // Instructor
            {
                Console.WriteLine("Instructor login successful.");
                // repeat indefinitely for instructor actions until quit
                while (true)
                {
                    Console.WriteLine("What would you like to do?");
                    Console.WriteLine("[Enter '1' to view existing courses]");
                    Console.WriteLine("[Enter '2' to view courses you currently teach]");
                    Console.WriteLine("[Enter '3' to submit final grades]");
                    Console.WriteLine("[Enter '4' to quit]");

                    if (Int32.TryParse(Console.ReadLine(), out action))
                    {
                        if (action >= 1 && action <= 3)
                        {
                            actionStatus = InstructorActionHandler(action);
                        }
                        else if (action == 4)
                        {
                            Console.WriteLine("Logging out...");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Please enter a number in the options above");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }
            else if (option == 3) // Student
            {
                Console.WriteLine("Student login successful.");
                // repeat indefinitely for student actions until quit
                while (true)
                {
                    Console.WriteLine("What would you like to do?");
                    Console.WriteLine("[Enter '1' to view existing courses]");
                    Console.WriteLine("[Enter '2' to view your registered courses]");
                    Console.WriteLine("[Enter '3' to view your student information]");
                    Console.WriteLine("[Enter '4' to view your final grades]");
                    Console.WriteLine("[Enter '5' to register for a course]");
                    Console.WriteLine("[Enter '6' to deregister a course]");
                    Console.WriteLine("[Enter '7' to quit]");

                    if (Int32.TryParse(Console.ReadLine(), out action))
                    {
                        if (action >= 1 && action <= 6)
                        {
                            actionStatus = StudentActionHandler(action);
                        }
                        else if (action == 7)
                        {
                            Console.WriteLine("Logging out...");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Please enter a number in the options above");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
