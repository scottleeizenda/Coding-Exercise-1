using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace IzendaCourseManagementSystem
{
    class Program
    {
        // Declarations
        static Administrator CurrentAdmin;
        static Instructor CurrentInstructor;
        static Student CurrentStudent;

        public static int courseGradeIdNumber;
        public static int assignInstructorIdNumber;
        public static int registerCourseIdNumber;
        public static string connString;
        public static SqlConnection connection;

        /**
         * TODO - make all user's ViewCourses method have the option to view a specific course, category of Courses, or list all Courses
         */
        //public static bool ViewSelectCourses()
        
        /// <summary>
        ///     Accesses the database to check inputted credentials with the ones stored in the database. Which table is
        ///     accessed is based on the userType parameter. User name matching is NOT case-sensitive, but password matching
        ///     IS case-sensitive, as per standard. Returns -1 upon failing to find matching credentials. Otherwise, returns
        ///     the ID of the User.
        /// </summary>
        static int CheckCredentials(string userName, string password, int userType)
        {
            SqlDataAdapter adapter;
            DataSet set;
            DataTable table;

            // figure out which database table to look through
            if (userType == 1) // Admin
            {
                adapter = new SqlDataAdapter("SELECT * FROM Administrator", connection);
                set = new DataSet();
                adapter.Fill(set, "Administrator");
                table = set.Tables["Administrator"];
            }
            else if (userType == 2) // Instructor
            {
                adapter = new SqlDataAdapter("SELECT * FROM Instructor", connection);
                set = new DataSet();
                adapter.Fill(set, "Instructor");
                table = set.Tables["Instructor"];
            }
            else if (userType == 3) // Student
            {
                adapter = new SqlDataAdapter("SELECT * FROM Student", connection);
                set = new DataSet();
                adapter.Fill(set, "Student");
                table = set.Tables["Student"];
            }
            else
            {
                return -1;
            }

            // check for match in username and password
            foreach (DataRow row in table.Rows)
            {
                if (userName.Equals(row["UserName"].ToString(), StringComparison.OrdinalIgnoreCase) &&
                    password.Equals(row["Password"].ToString()))
                {
                    return int.Parse(row["Id"].ToString());
                }
            }

            return -1;
        }
        
        /// <summary>
        ///     Interacts with user for a number of options. First it will prompt user to choose what type of user he/she
        ///     would like to login as. Then it'll proceed to ask user for a username and password, which will then jump to
        ///     the CheckCredentials method to check in with the data for a match in the corresponding table (Administrator,
        ///     Instructor, or Student tables, based on previous option). Returns -1 upon user entering the quit (4) option.
        ///     Otherwise, return the option that was selected in the beginning that specified user type.
        /// </summary>
        static int Login()
        {
            int option, loginId;

            // infinite loop to retry in case of invalid input and handle log in
            while (true)
            {
                Console.WriteLine("What type of user would you like to log in as?");
                Console.WriteLine("[Enter '1' for Administrator, '2' for Instructor, '3' for Student, '4' to quit]:");
                if (Int32.TryParse(Console.ReadLine(), out option))
                {
                    if (option == 4) // quit
                    {
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
                    loginId = CheckCredentials(userName, password, option);
                    if (loginId != -1) // set up current user for navigation if login success
                    {
                        if (option == 1)
                        {
                            //CurrentAdmin = Administrator.SearchAdministratorById(connection, loginId);
                            CurrentAdmin = (Administrator)User.SearchUserById(connection, loginId, 1);
                        }
                        else if (option == 2)
                        {
                            //CurrentInstructor = Administrator.SearchInstructorById(connection, loginId);
                            CurrentInstructor = (Instructor)User.SearchUserById(connection, loginId, 2);
                        }
                        else if (option == 3)
                        {
                            //CurrentStudent = Course.SearchStudentById(connection, loginId);
                            CurrentStudent = (Student)User.SearchUserById(connection, loginId, 3);
                        }
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
            /***** Set up Database Tools and ID number generation *****/
            connString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=IzendaCourseManagementSystem;Data Source=SLEE-PC";
            connection = new SqlConnection(connString);
            connection.Open();
            //Console.WriteLine("Database Connection Successful!");
            SqlDataAdapter courseGradesIdAdapter = new SqlDataAdapter("SELECT MAX(Id) AS MostRecentId FROM CourseGrades", connection);
            SqlDataAdapter assignInstructorIdAdapter = new SqlDataAdapter("SELECT MAX(Id) AS MostRecentId FROM Instructor_Course", connection);
            SqlDataAdapter registerCourseIdAdapter = new SqlDataAdapter("SELECT MAX(Id) AS MostRecentId FROM Student_Course", connection);
            DataSet set = new DataSet();
            courseGradesIdAdapter.Fill(set, "CourseGradesMaxId");
            assignInstructorIdAdapter.Fill(set, "Instructor_CourseMaxId");
            registerCourseIdAdapter.Fill(set, "Student_CourseMaxId");
            // set current courseGradeIdNumber to work up from
            DataTable table = set.Tables["CourseGradesMaxId"];
            DataRow row = table.Rows[0];
            courseGradeIdNumber = int.Parse(row["MostRecentId"].ToString()) + 1;
            // set current assignInstructorIdNumber to work up from
            table = set.Tables["Instructor_CourseMaxId"];
            row = table.Rows[0];
            assignInstructorIdNumber = int.Parse(row["MostRecentId"].ToString()) + 1;
            // set current registerCourseIdNumber to work up from
            table = set.Tables["Student_CourseMaxId"];
            row = table.Rows[0];
            registerCourseIdNumber = int.Parse(row["MostRecentId"].ToString()) + 1;

            //Console.WriteLine($"{courseGradeIdNumber}, {assignInstructorIdNumber}, {registerCourseIdNumber}");

            /****** Start of the text-based user interactions ******/
            Console.WriteLine("=============================================================");
            Console.WriteLine("||     Welcome to the Izenda Course Management System!     ||");
            Console.WriteLine("=============================================================");
            int option, action;
            bool actionStatus;
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
                        // TODO - consider adding option to unassign Instructor from a Course
                        Console.WriteLine("[Enter '6' to quit]");

                        if (Int32.TryParse(Console.ReadLine(), out action))
                        {
                            if (action >= 1 && action <= 5)
                            {
                                actionStatus = CurrentAdmin.AdminActionHandler(connection, action);
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
                                actionStatus = CurrentInstructor.InstructorActionHandler(connection, action);
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
                                actionStatus = CurrentStudent.StudentActionHandler(connection, action);
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
