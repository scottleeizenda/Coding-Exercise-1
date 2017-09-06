using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCourseManagementSystem
{
    class Program
    {
        // Declarations
        static List<Instructor> instructors;
        static List<Student> students;
        static List<Administrator> administrators;
        static List<Course> courses;

        static int courseGradeIdNumber;

        /**
         * Performs an Administrator action based on the param, the check for valid action
         * number already done in main
         */
        static bool AdminActionHandler(Administrator currentAdmin, int action)
        {
            if (action == 1) // create course
            {
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

                bool status = currentAdmin.CreateCourse(courses, courseFields);
                if (status)
                {
                    Console.WriteLine("Course successfully created.");
                }
                else
                {
                    Console.WriteLine("Failed to create course.");
                }
                return status;
            }
            else if (action == 2) // view courses
            {
                bool status = currentAdmin.ViewCourses(courses);
                if(!status)
                {
                    Console.WriteLine("There are currently no existing courses.");
                }
                return status;
            }
            else if (action == 3) // edit course
            {
                Console.Write("Enter the ID of the course you would like to edit: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    int index = Course.SearchCourseById(courses, id);
                    if (index >= 0)
                    {
                        Console.WriteLine($"Course Successfully Found:\n{courses[index]}");
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

                        bool status = currentAdmin.UpdateCourse(courses, index, courseFields);
                        if (status)
                        {
                            Console.WriteLine("Course successfully updated.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to update course.");
                        }
                        return status;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id}");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
                    return false;
                }
            }
            else if (action == 4) // delete course
            {
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
                        if (choice.Equals("Y", StringComparison.OrdinalIgnoreCase))
                        {
                            bool status = currentAdmin.DeleteCourse(courses, index);
                            Console.WriteLine($"Successfully deleted course with ID {id}");
                            return status; // should always be true at this point (for now)
                        }
                        else if (choice.Equals("N", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Cancelling deletion of course with ID {id}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice, returning to menu...");
                        }
                        return false; // if no successful delete occurs, return false no matter what
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id}");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
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
                while(true)
                {
                    Console.WriteLine("Type the ID of the instructor you would like to assign a course to");
                    Console.WriteLine("[Enter 'list' to see all the available instructors]");
                    Console.Write("[Enter 'quit' to cancel]: ");
                    string input = Console.ReadLine();
                    int id;
                    if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        currentAdmin.ViewInstructors(instructors);
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        int index = currentAdmin.SearchInstructorById(instructors, id);
                        if (index >= 0)
                        {
                            selectedInstructor = instructors[index];
                            Console.WriteLine($"Instructor {selectedInstructor.FirstName} {selectedInstructor.LastName} successfully found.");
                            // loop for user input on selecting a course to assign
                            while (true)
                            {
                                Console.WriteLine("Type the ID of the course you would like to assign to this instructor");
                                Console.WriteLine("[Enter 'list' to see all the available courses]");
                                Console.Write("[Enter 'quit' to cancel]: ");
                                input = Console.ReadLine();
                                if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                                {
                                    currentAdmin.ViewCourses(courses);
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
                                        bool status = currentAdmin.AssignInstructor(selectedCourse, selectedInstructor);
                                        if (status)
                                        {
                                            Console.WriteLine($"Instructor {selectedInstructor.FirstName} {selectedInstructor.LastName} has been assigned to teach course {selectedCourse.CourseName}");
                                            return true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Failed to assign {selectedInstructor.FirstName} {selectedInstructor.LastName} to teach the course {selectedCourse.CourseName}");
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

        /**
         * Performs an Instructor action based on the param, the check for valid action
         * number already done in main
         */
        static bool InstructorActionHandler(Instructor currentInstructor, int action)
        {
            if (action == 1) // view courses
            {
                bool status = currentInstructor.ViewCourses(courses);
                if (!status)
                {
                    Console.WriteLine("There are currently no existing courses.");
                }
                return status;
            }
            else if (action == 2) // view assigned
            {
                bool status = currentInstructor.ViewAssignedCourses();
                if (!status)
                {
                    Console.WriteLine("You are not currently assigned to teach any courses.");
                }
                return status;
            }
            else // submit final grades
            {
                // Check for empty assigned courses list
                if (currentInstructor.AssignedCourses.Count == 0)
                {
                    Console.WriteLine("You currently have no courses assigned to you, so no final grades to submit.");
                    return false;
                }

                Course selectedCourse;
                Student selectedStudent;
                // loop for user input on selecting an assigned course to submit grades for
                while (true)
                {
                    Console.WriteLine("Type the ID of the course you would like to submit final grades for");
                    Console.WriteLine("[Enter 'list' to see all your assigned courses]");
                    Console.Write("[Enter 'quit' to cancel]: ");
                    string input = Console.ReadLine();
                    int id;
                    if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        currentInstructor.ViewAssignedCourses();
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        int index = currentInstructor.SearchAssignedCourseById(id);
                        if (index >= 0)
                        {
                            selectedCourse = currentInstructor.AssignedCourses[index];
                            Console.WriteLine($"Course {selectedCourse.CourseName} successfully found.");
                            // loop for user input on selecting a student to submit a grade for
                            while (true)
                            {
                                Console.WriteLine("Type the ID of the student you would like to submit a final grade for");
                                Console.WriteLine("[Enter 'list' to see all students registered for this course]");
                                Console.Write("[Enter 'quit' to cancel]: ");
                                input = Console.ReadLine();
                                if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                                {
                                    selectedCourse.ViewRegisteredStudents();
                                }
                                else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Cancelled, returning to course select...");
                                    break;
                                }
                                else if (Int32.TryParse(input, out id))
                                {
                                    index = selectedCourse.SearchStudentById(id);
                                    if (index >= 0)
                                    {
                                        // TODO - implement a way to be able to submit one final grade per course

                                        selectedStudent = selectedCourse.RegisteredStudents[index];
                                        Console.WriteLine($"Student {selectedStudent.FirstName} {selectedStudent.LastName} successfully found.");
                                        Console.Write("What letter grade would you like to give this student for this course?: ");
                                        string letterGrade = Console.ReadLine();
                                        bool status = currentInstructor.SubmitFinalGrade(selectedStudent, letterGrade, selectedCourse.Id, courseGradeIdNumber);
                                        courseGradeIdNumber++; // increment just to make unique subsequent CourseGrades.Id's
                                        if (status)
                                        {
                                            Console.WriteLine($"Final Grade '{letterGrade.ToUpper()}' has been submitted to for {selectedStudent.FirstName} {selectedStudent.LastName}");
                                            return true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Failed to submit final grade '{letterGrade.ToUpper()}', please use letters A-D or F");
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to find registered student with ID {id}");
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
                            Console.WriteLine($"Failed to find course with ID {id}");
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

        /**
         * Performs a Student action based on the param, the check for valid action
         * number already done in main
         */
        static bool StudentActionHandler(Student currentStudent, int action)
        {
            if (action == 1) // view courses
            {
                bool status = currentStudent.ViewCourses(courses);
                if (!status)
                {
                    Console.WriteLine("There are currently no existing courses.");
                }
                return status;
            }
            else if (action == 2) // view registered
            {
                bool status = currentStudent.ViewRegisteredCourses();
                if (!status)
                {
                    Console.WriteLine("You are not currently registered for any courses.");
                }
                return status;
            }
            else if (action == 3) // view info
            {
                Console.WriteLine(currentStudent);
                return true;
            }
            else if (action == 4) // view grades
            {
                bool status = currentStudent.ViewFinalGrades();
                if (!status)
                {
                    Console.WriteLine("You have not completed any courses.");
                }
                return status;
            }
            else if (action == 5) // register course
            {
                Console.Write("Enter the ID of the course you would like to register for: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    int index = Course.SearchCourseById(courses, id);
                    if (index >= 0)
                    {
                        Console.WriteLine($"Course Successfully Found, adding this course to your registered courses:\n{courses[index]}");
                        courses[index].RegisteredStudents.Add(currentStudent); // add student to RegisteredStudents list in Course
                        bool status = currentStudent.RegisterCourse(courses, index);
                        if (status)
                        {
                            Console.WriteLine("Course successfully registered.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to register course.");
                        }
                        return status;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id}");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
                    return false;
                }
            }
            else // deregister course
            {
                if (currentStudent.RegisteredCourses.Count == 0)
                {
                    Console.WriteLine("You are not currently registered for any courses");
                    return false;
                }

                Console.Write("Enter the ID of the course you would like to deregister: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    int index = currentStudent.SearchRegisteredCourse(id);
                    if (index >= 0)
                    {
                        Console.WriteLine($"Course Successfully Found, removing this course from your registered courses:\n{courses[index]}");
                        courses[index].RegisteredStudents.RemoveAt(index); // remove student from RegisteredStudents list in Course
                        bool status = currentStudent.DeregisterCourse(index);
                        if (status)
                        {
                            Console.WriteLine("Course successfully deregistered.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to deregister course.");
                        }
                        return status;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id} in your registered courses");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
                    return false;
                }
            }
        }

        /**
         * 
         */
        public static int CheckCredentials(string userName, string password, int userType)
        {
            if (userType == 1) // Admin
            {
                for (int i = 0; i < administrators.Count; i++)
                {
                    if (userName.Equals(administrators[i].UserName, StringComparison.OrdinalIgnoreCase) &&
                        password.Equals(administrators[i].Password))
                    {
                        return i;
                    }
                    return -1;
                }
            }
            else if (userType == 2) // Instructor
            {
                for (int i = 0; i < instructors.Count; i++)
                {
                    if (userName.Equals(instructors[i].UserName, StringComparison.OrdinalIgnoreCase) &&
                        password.Equals(instructors[i].Password))
                    {
                        return i;
                    }
                    return -1;
                }
            }
            else if (userType == 3) // Student
            {
                for (int i = 0; i < students.Count; i++)
                {
                    if (userName.Equals(students[i].UserName, StringComparison.OrdinalIgnoreCase) &&
                        password.Equals(students[i].Password))
                    {
                        return i;
                    }
                    return -1;
                }
            }

            return -1;
        }

        static void Main(string[] args)
        {
            // Initializations of default users and courses (for now)

            instructors = new List<Instructor>();
            students = new List<Student>();
            administrators = new List<Administrator>();
            courses = new List<Course>();
            
            instructors.Add(new Instructor(100, "John", "Doe", DateTime.Now, "johndoe", "TheRealJohnDoe"));
            instructors.Add(new Instructor(101, "Mary", "Moe", DateTime.Now, "marymoe", "TheRealMaryMoe"));
            instructors.Add(new Instructor(102, "David", "Throe", DateTime.Now, "davidjoe", "TheRealDavidThroe"));
            students.Add(new Student(200, "Larry", "Low", "larrylow", "Iamlarrylow", (float)3.3, 55));
            students.Add(new Student(201, "Natalie", "Ngo", "nataliengo", "Iamnataliengo", (float)4.0, 100));
            students.Add(new Student(202, "Mark", "Llark", "markllark", "Iammarkllark", (float)1.7, 3));
            administrators.Add(new Administrator(1, "Admin", "1", DateTime.Now, "admin1", "adminOne"));
            courses.Add(new Course(1302, DateTime.Now, DateTime.Now, 4, "CSCI1302", "Software Development"));
            courses.Add(new Course(1730, DateTime.Now, DateTime.Now, 4, "CSCI1730", "Systems Programming"));
            courses.Add(new Course(2670, DateTime.Now, DateTime.Now, 4, "CSCI2670", "Intro to the Theory of Computing"));
            students[0].RegisteredCourses.Add(courses[1]);
            courses[1].RegisteredStudents.Add(students[0]);
            students[0].FinalGrades.Add(new CourseGrades(1, 1302, 'B'));
            students[1].FinalGrades.Add(new CourseGrades(2, 1302, 'A'));
            students[1].FinalGrades.Add(new CourseGrades(3, 1730, 'A'));
            students[1].FinalGrades.Add(new CourseGrades(4, 2670, 'A'));
            instructors[0].AssignedCourses.Add(courses[0]);
            instructors[1].AssignedCourses.Add(courses[1]);
            instructors[2].AssignedCourses.Add(courses[2]);
            courseGradeIdNumber = 1;



            /****** Start of the text-based user interactions ******/

            Console.WriteLine("Welcome to the Izenda Course Management System!");
            int option, loginIndex;
            Administrator currentAdmin;
            Instructor currentInstructor;
            Student currentStudent;
            // infinite loop to retry in case of invalid input and handle log in
            while (true)
            {
                Console.WriteLine("What type of user would you like to log in as?");
                Console.WriteLine("[Enter '1' for Administrator, '2' for Instructor, '3' for Student, '4' to quit]:");
                if (Int32.TryParse(Console.ReadLine(), out option))
                {
                    if (option == 4) // quit
                    {
                        Console.WriteLine("Log in cancelled, exiting system...");
                        System.Environment.Exit(0);
                    }
                    else if (option <= 0 || option > 4)
                    {
                        Console.WriteLine("Please enter a number in the options above");
                        continue;
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input, please try again");
                }

                // login
                while (true)
                {
                    Console.Write("User Name: ");
                    string userName = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    loginIndex = CheckCredentials(userName, password, option);
                    if (loginIndex >= 0) // set up current user for navigation
                    {
                        if (option == 1) { currentAdmin = administrators[loginIndex]; }
                        else if (option == 2) { currentInstructor = instructors[loginIndex]; }
                        else if (option == 3) { currentStudent = students[loginIndex]; }
                        break;
                    }
                    else
                    {
                        Console.Write("Invalid user name or password, would you like to try again? [Y/N]: ");
                        string res = Console.ReadLine();
                        if (res.Equals("Y", StringComparison.OrdinalIgnoreCase)) { continue; }
                        else if (res.Equals("N", StringComparison.OrdinalIgnoreCase)) { System.Environment.Exit(0); }
                        else
                        {
                            Console.Write("Invalid response, returning to login menu...");
                            break;
                        }
                    }
                }
                if (loginIndex >= 0) { break; }
            }
            
            // TODO - fix this temp solution
            currentAdmin = administrators[0];
            currentInstructor = instructors[1];
            currentStudent = students[0];

            // Display and handle user actions based on previous option

            int action;
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
                    Console.WriteLine("[Enter '6' to quit]");

                    if (Int32.TryParse(Console.ReadLine(), out action))
                    {
                        if (action >= 1 && action <= 5)
                        {
                            AdminActionHandler(currentAdmin, action);
                        }
                        else if (action == 6)
                        {
                            Console.WriteLine("Logging out...");
                            break;
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
                            InstructorActionHandler(currentInstructor, action);
                        }
                        else if (action == 4)
                        {
                            Console.WriteLine("Logging out...");
                            break;
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
                            StudentActionHandler(currentStudent, action);
                        }
                        else if (action == 7)
                        {
                            Console.WriteLine("Logging out...");
                            break;
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
        }
    }
}
