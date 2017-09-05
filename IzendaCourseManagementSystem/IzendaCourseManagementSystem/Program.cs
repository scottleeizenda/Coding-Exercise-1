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
                // TODO
                Console.WriteLine("Not yet implemented, sorry. T.T");
                return true;
            }
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
            instructors.Add(new Instructor(102, "David", "Joe", DateTime.Now, "davidjoe", "TheRealDavidJoe"));
            students.Add(new Student(200, "Larry", "Low", "larrylow", "Iamlarrylow", (float)3.3, 55));
            students.Add(new Student(201, "Natalie", "Ngo", "nataliengo", "Iamnataliengo", (float)4.0, 100));
            students.Add(new Student(202, "Mark", "Llark", "markllark", "Iammarkllark", (float)1.7, 3));
            administrators.Add(new Administrator(1, "Admin", "1", DateTime.Now, "admin1", "adminOne"));
            courses.Add(new Course(1302, DateTime.Now, DateTime.Now, 4, "CSCI1302", "Software Development"));
            courses.Add(new Course(1730, DateTime.Now, DateTime.Now, 4, "CSCI1730", "Systems Programming"));
            courses.Add(new Course(2670, DateTime.Now, DateTime.Now, 4, "CSCI2670", "Intro to the Theory of Computing"));



            /****** Start of the text-based user interactions ******/

            Console.WriteLine("Welcome to the Izenda Course Management System!");
            int option;
            // infinite loop to retry in case of invalid input
            while (true)
            {
                Console.WriteLine("What type of user would you like to log in as?");
                Console.WriteLine("[Enter '1' for Administrator, '2' for Instructor, '3' for Student, '4' to quit]:");
                if (Int32.TryParse(Console.ReadLine(), out option))
                {
                    if (option == 4) // quit
                    {
                        Console.WriteLine("Log in cancelled.");
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
            }

            // TODO - Add log in prompt
            //temp solution
            Administrator currentAdmin = administrators[0];
            Instructor currentInstructor = instructors[0];
            Student currentStudent = students[0];

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
                            // TODO - handle instructor action
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
                    Console.WriteLine("[Enter '6' to quit]");

                    if (Int32.TryParse(Console.ReadLine(), out action))
                    {
                        if (action >= 1 && action <= 5)
                        {
                            // TODO - handle student action
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
        }
    }
}
