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
        static List<Instructor> Instructors;
        static List<Student> Students;
        static List<Administrator> Administrators;
        static List<Course> Courses;
        static Administrator CurrentAdmin;
        static Instructor CurrentInstructor;
        static Student CurrentStudent;

        public static int courseGradeIdNumber;

        /**
         * TODO - make all user's ViewCourses method have the option to view a specific course, category of Courses, or list all Courses
         */
        //public static bool ViewSelectCourses()

        /**
         * 
         */
        static int CheckCredentials(string userName, string password, int userType)
        {
            if (userType == 1) // Admin
            {
                for (int i = 0; i < Administrators.Count; i++)
                {
                    if (userName.Equals(Administrators[i].UserName, StringComparison.OrdinalIgnoreCase) &&
                        password.Equals(Administrators[i].Password))
                    {
                        return i;
                    }
                }
                return -1;
            }
            else if (userType == 2) // Instructor
            {
                for (int i = 0; i < Instructors.Count; i++)
                {
                    if (userName.Equals(Instructors[i].UserName, StringComparison.OrdinalIgnoreCase) &&
                        password.Equals(Instructors[i].Password))
                    {
                        return i;
                    }
                }
                return -1;
            }
            else if (userType == 3) // Student
            {
                for (int i = 0; i < Students.Count; i++)
                {
                    if (userName.Equals(Students[i].UserName, StringComparison.OrdinalIgnoreCase) &&
                        password.Equals(Students[i].Password))
                    {
                        return i;
                    }
                }
                return -1;
            }

            return -1;
        }

        /**
         * Asks user for what type of user to login as and attempts to login with user-specified credentials.
         * Returns -1 upon choosing to quit, returns 1-3 upon success where the number specifies user type.
         */
        static int Login()
        {
            int option, loginIndex;

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
                        return -1;
                    }
                    else if (option <= 0 || option > 4)
                    {
                        Console.WriteLine("Please enter a number in the options above");
                        continue;
                    }
                    // else continue to login code
                }
                else
                {
                    Console.WriteLine("Invalid input, please try again");
                    continue;
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
                        if (option == 1) { CurrentAdmin = Administrators[loginIndex]; }
                        else if (option == 2) { CurrentInstructor = Instructors[loginIndex]; }
                        else if (option == 3) { CurrentStudent = Students[loginIndex]; }
                        //break;
                        return option;
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
            }
        }

        static void Main(string[] args)
        {
            // Initializations of default users and Courses (for now)

            Instructors = new List<Instructor>();
            Students = new List<Student>();
            Administrators = new List<Administrator>();
            Courses = new List<Course>();
            
            Instructors.Add(new Instructor(100, "John", "Doe", DateTime.Now, "johndoe", "TheRealJohnDoe"));
            Instructors.Add(new Instructor(101, "Mary", "Moe", DateTime.Now, "marymoe", "TheRealMaryMoe"));
            Instructors.Add(new Instructor(102, "David", "Throe", DateTime.Now, "davidthroe", "TheRealDavidThroe"));
            Students.Add(new Student(200, "Larry", "Low", "larrylow", "Iamlarrylow", (float)3.3, 55));
            Students.Add(new Student(201, "Natalie", "Ngo", "nataliengo", "Iamnataliengo", (float)4.0, 100));
            Students.Add(new Student(202, "Mark", "Llark", "markllark", "Iammarkllark", (float)1.7, 3));
            Administrators.Add(new Administrator(1, "Admin", "1", DateTime.Now, "admin1", "adminOne"));
            Courses.Add(new Course(1302, DateTime.Now, DateTime.Now, 4, "CSCI1302", "Software Development"));
            Courses.Add(new Course(1730, DateTime.Now, DateTime.Now, 4, "CSCI1730", "Systems Programming"));
            Courses.Add(new Course(2670, DateTime.Now, DateTime.Now, 4, "CSCI2670", "Intro to the Theory of Computing"));
            Courses.Add(new Course(2720, DateTime.Now, DateTime.Now, 4, "CSCI2720", "Data Structures"));
            Courses.Add(new Course(3030, DateTime.Now, DateTime.Now, 3, "CSCI3030", "Computer Ethics"));
            Students[0].RegisteredCourses.Add(Courses[1]);
            Courses[1].RegisteredStudents.Add(Students[0]);
            Students[0].FinalGrades.Add(new CourseGrades(1, 1302, 'B'));
            Students[1].FinalGrades.Add(new CourseGrades(2, 1302, 'A'));
            Students[1].FinalGrades.Add(new CourseGrades(3, 1730, 'A'));
            Students[1].FinalGrades.Add(new CourseGrades(4, 2670, 'A'));
            Instructors[0].AssignedCourses.Add(Courses[0]);
            Instructors[1].AssignedCourses.Add(Courses[1]);
            Instructors[2].AssignedCourses.Add(Courses[2]);
            courseGradeIdNumber = 1;



            /****** Start of the text-based user interactions ******/
            Console.WriteLine("=============================================================");
            Console.WriteLine("||     Welcome to the Izenda Course Management System!     ||");
            Console.WriteLine("=============================================================");
            int option, action;
            // loop to allow log in as a different user upon one logging out
            while (true)
            {
                // Try login
                option = Login();
                if (option == -1)
                {
                    Console.WriteLine("Login cancelled, exiting system...");
                    break;
                }

                // Display and handle user actions based on type of user logged in
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
                        Console.WriteLine("[Enter '6' to quit]");

                        if (Int32.TryParse(Console.ReadLine(), out action))
                        {
                            if (action >= 1 && action <= 5)
                            {
                                CurrentAdmin.AdminActionHandler(Courses, Instructors, action);
                            }
                            else if (action == 6)
                            {
                                Console.WriteLine("Logging out...");
                                Console.WriteLine("-----------------------------------------------------------------------------");
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
                                CurrentInstructor.InstructorActionHandler(Courses, action);
                            }
                            else if (action == 4)
                            {
                                Console.WriteLine("Logging out...");
                                Console.WriteLine("-----------------------------------------------------------------------------");
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
                                CurrentStudent.StudentActionHandler(Courses, action);
                            }
                            else if (action == 7)
                            {
                                Console.WriteLine("Logging out...");
                                Console.WriteLine("-----------------------------------------------------------------------------");
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
}
