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
        ///     Displays all available courses in the Course table.
        ///     If the query succeeds, returns true. Otherwise, returns false.
        /// </summary>
        public bool ViewCourses(SqlConnection connection)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Course", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Course");
                DataTable table = set.Tables[0];

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

                return true;
            }
            catch
            {
                return false;
            }
        }
        // how to more naturally combine the two ViewCourses?
        public int ViewCourses(SqlConnection connection, string prefix)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Course WHERE CourseName LIKE '{prefix}%'", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Course");
                DataTable table = set.Tables[0];
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
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
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
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
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
