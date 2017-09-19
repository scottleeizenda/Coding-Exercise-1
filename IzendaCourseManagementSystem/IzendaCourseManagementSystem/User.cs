using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace IzendaCourseManagementSystem
{
    public class User
    {
        // Declarations & Getters/Setters
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        
        public User(int id, string firstName, string lastName, string userName, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Password = password;
            UserType = "Undefined";
        }

        /// <summary>
        ///     Overloaded Method. Displays available courses from the Course table depending on the prefix parameter.
        ///     If prefix is null, displays all existing courses. Otherwise, displays courses filtered by the prefix.
        ///     Returns the number of rows displayed, or returns -1 if database operation goes wrong.
        /// </summary>
        public int ViewCourses(SqlConnection connection, string prefix)
        {
            // if prefix param is null, show all courses. else show courses filtered by prefix
            string query;
            if (prefix == null)
            {
                query = "SELECT * FROM Course";
            }
            else
            {
                query = $"SELECT * FROM Course WHERE CourseName LIKE '{prefix}%'";
            }

            return PerformViewCourses(connection, query);
        }

        /// <summary>
        ///     Overloaded method. Takes in userId that signifies either an Instructor's ID or a Student's ID.
        ///     Parameter userType will specify what type of ID to read userId as where '2' is for Instructor and
        ///     '3' is for Student. Intaking an Instructor ID will pass a query to the PerformViewCourses method
        ///     to print an Instructor's assigned courses. Intaking a Student ID will pass a query to print the
        ///     specified Student's registered courses. Returns -1 upon invalid userType, otherwise returns value
        ///     from PerformViewCourses method.
        /// </summary>
        public int ViewCourses(SqlConnection connection, int userId, int userType)
        {
            // if prefix param is null, show all courses. else show courses filtered by prefix
            string query;
            if (userType == 2) // Instructor viewing assigned courses
            {
                query = "SELECT Course.Id, Course.StartDate, Course.EndDate, Course.CreditHours, Course.CourseName, Course.CourseDescription FROM Course " +
                       $"INNER JOIN Instructor_Course ON Course.Id = Instructor_Course.CourseId WHERE Instructor_Course.InstructorId = {userId}";
            }
            else if (userType == 3) // Student viewing registered courses
            {
                query = "SELECT Course.Id, Course.StartDate, Course.EndDate, Course.CreditHours, Course.CourseName, Course.CourseDescription FROM Course " +
                       $"INNER JOIN Student_Course ON Course.Id = Student_Course.CourseId WHERE Student_Course.StudentId = {userId}";
            }
            else // invalid userType
            {
                return -1;
            }

            return PerformViewCourses(connection, query);
        }

        /// <summary>
        ///     This method works in conjunction with one of the overloaded ViewCourses methods. The ViewCourses
        ///     methods take care of the correct SELECT statement to perform, and this method accesses the database
        ///     with it. Displays all Course column values from the produced table and returns the number of rows
        ///     printed. Returns -1 if any database operation goes wrong.
        /// </summary>
        private static int PerformViewCourses(SqlConnection connection, string query)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Course");
                DataTable table = set.Tables["Course"];
                int numRows = table.Rows.Count;

                // Check if table is empty
                if (numRows == 0)
                {
                    return 0;
                }

                Console.WriteLine("-----------------------------------------------------------------------------");
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine($"Course ID: {row["Id"]}");
                    Console.WriteLine($"Start Date: {row["StartDate"]}");
                    Console.WriteLine($"End Date: {row["EndDate"]}");
                    Console.WriteLine($"Credit Hours: {row["CreditHours"]}");
                    Console.WriteLine($"Course Name: {row["CourseName"]}");
                    Console.WriteLine($"Course Description: {row["CourseDescription"]}\n");
                }
                Console.WriteLine("-----------------------------------------------------------------------------");

                return numRows;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        ///     Displays all available distinct 4-letter course prefixes from the CourseName column in the Course table.
        ///     Returns true upon succesfully displaying, returns false otherwise.
        /// </summary>
        public bool ViewCoursePrefix(SqlConnection connection)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT DISTINCT SUBSTRING(CourseName, 0, 5) AS CoursePrefix FROM Course", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Prefix");
                DataTable table = set.Tables[0];

                Console.WriteLine("-----------------------------------------------------------------------------");
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine(row["CoursePrefix"].ToString());
                }
                Console.WriteLine("-----------------------------------------------------------------------------");

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Handles user input in how he/she wants to display the courses currently in the database Course table.
        /// </summary>
        public void ViewCoursesHandler(SqlConnection connection)
        {
            Console.WriteLine("-----------------------------------------------------------------------------");
            while (true)
            {
                Console.WriteLine("[Enter '1' to view all existing courses]");
                Console.WriteLine("[Enter '2' to view all existing courses under a given course prefix]");
                Console.WriteLine("[Enter '3' to search for a specific course by ID]");
                Console.WriteLine("[Enter '4' to go back to previous menu]:");
                int input;
                if (Int32.TryParse(Console.ReadLine(), out input))
                {
                    if (input == 1) // view all
                    {
                        this.ViewCourses(connection, null);
                    }
                    else if (input == 2) // view prefix-filtered
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        Console.WriteLine("Here is a list of available course prefixes:");
                        this.ViewCoursePrefix(connection);
                        Console.Write("Enter the Course prefix you would like to search for: ");
                        string prefix = Console.ReadLine();
                        int status = this.ViewCourses(connection, prefix);
                        if (status == 0)
                        {
                            Console.WriteLine($"There are no courses under the prefix '{prefix}'");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                    }
                    else if (input == 3) // view specific by id
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        Console.Write("Enter the ID of the course you would like to look up: ");
                        int courseId;
                        if (Int32.TryParse(Console.ReadLine(), out courseId))
                        {
                            Console.WriteLine(Course.SearchCourseById(connection, null, courseId));
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID input");
                            continue;
                        }
                    }
                    else if (input == 4) // go back to previous menu
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        Console.WriteLine("Invalid option, please enter one of the numbers listed");
                    }
                }
                else
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Invalid input, please try again");
                }
            }
        }

        /// <summary>
        ///     Searches through the table of a certain user type specified by the userType parameter and finds a user with a
        ///     matching ID as the id parameter. Returns a User with the same field values from the table row and should be
        ///     casted as the correct specific type of user by the caller (i.e cast Administrator, Instructor, or Student).
        ///     Otherwise, return null upon invalid userType or if a database operation goes wrong.
        /// </summary>
        public static User SearchUserById(SqlConnection connection, int id, int userType)
        {
            // set up string with user type to paste into SqlDataAdapter
            string user; // this will be equal to the DB table that needs to be accessed
            if (userType == 1) { user = "Administrator"; }
            else if (userType == 2) { user = "Instructor"; }
            else if (userType == 3) { user = "Student"; }
            else { return null; }

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {user}", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, user);
                set.Tables[user].Constraints.Add("Id_PK", set.Tables[user].Columns["Id"], true);

                DataRow row = set.Tables[user].Rows.Find(id);

                if (userType == 1)
                {
                    DateTime hireDate = DateTime.Parse(row["HireDate"].ToString());
                    return new Administrator(id, row["FirstName"].ToString(), row["LastName"].ToString(), hireDate, row["UserName"].ToString(), row["Password"].ToString());
                }
                else if (userType == 2)
                {
                    DateTime hireDate = DateTime.Parse(row["HireDate"].ToString());
                    return new Instructor(id, row["FirstName"].ToString(), row["LastName"].ToString(), hireDate, row["UserName"].ToString(), row["Password"].ToString());
                }
                else if (userType == 3)
                {
                    float gpa = float.Parse(row["GPA"].ToString());
                    int hours = Int32.Parse(row["CreditHours"].ToString());
                    return new Student(id, row["FirstName"].ToString(), row["LastName"].ToString(), row["UserName"].ToString(), row["Password"].ToString(), gpa, hours);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
