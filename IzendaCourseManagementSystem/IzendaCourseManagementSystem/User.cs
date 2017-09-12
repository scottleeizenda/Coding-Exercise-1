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
        ///     Displays all available courses in the param courses List.
        ///     If the list is empty, returns false. Otherwise, successfully displays and returns true.
        /// </summary>
        /*public bool ViewCourses(List<Course> courses)
        {
            if (!courses.Any())
            {
                return false;
            }

            Console.WriteLine("-----------------------------------------------------------------------------");
            foreach (Course c in courses)
            {
                Console.WriteLine(c);
            }
            Console.WriteLine("-----------------------------------------------------------------------------");
            return true;
        }*/

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
    }
}
