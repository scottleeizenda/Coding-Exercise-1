using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace IzendaCourseManagementSystem
{
    public class Course
    {
        // Declarations & Getters/Setters
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreditHours { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }

        public Course(int id, DateTime startDate, DateTime endDate, int creditHours, string courseName, string courseDescription)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            CreditHours = creditHours;
            CourseName = courseName;
            CourseDescription = courseDescription;
        }
        
        /// <summary>
        ///     Searches through the Course table to find a Course with an ID matching the param id. If parameter 'query' is null,
        ///     searches through the entire Course table. Otherwise, uses the query to subset the Course table to search through.
        ///     Returns a new Course object with the same field values from the database upon success. Otherwise, returns null.
        /// </summary>
        public static Course SearchCourseById(SqlConnection connection, string query, int id)
        {
            if (query == null) { query = "SELECT * FROM Course"; }

            try
            {
                //SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Course", connection);
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Course");
                set.Tables["Course"].Constraints.Add("Id_PK", set.Tables["Course"].Columns["Id"], true);

                DataRow row = set.Tables["Course"].Rows.Find(id);
                DateTime startDate = DateTime.Parse(row["StartDate"].ToString());
                DateTime endDate = DateTime.Parse(row["EndDate"].ToString());
                int hours = Int32.Parse(row["CreditHours"].ToString());

                return new Course(id, startDate, endDate, hours, row["CourseName"].ToString(), row["CourseDescription"].ToString());
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        ///     Displays all the Students that are registered for a specific course by displaying the rows in the
        ///     Student_Course table where CourseId matches the Course calling object's Id. The displayed information
        ///     each Student's info from their ToString() method. Returns true upon successfully displaying all
        ///     entries. Otherwise, returns false.
        /// </summary>
        public int ViewRegisteredStudents(SqlConnection connection)
        {
            // SELECT only the students registered for the specified course
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Student_Course WHERE CourseID = {this.Id}", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Student_Course");
                DataTable table = set.Tables["Student_Course"];
                int numRows = table.Rows.Count;
                // check if empty
                if (numRows == 0)
                {
                    return 0;
                }

                Console.WriteLine("-----------------------------------------------------------------------------");
                foreach (DataRow row in table.Rows)
                {
                    // Lookup and show full Student information from StudentId
                    int currentStudentId = int.Parse(row["StudentId"].ToString());
                    Console.WriteLine(User.SearchUserById(connection, currentStudentId, 3));
                }
                Console.WriteLine("-----------------------------------------------------------------------------");

                return numRows;
            }
            catch
            {
                return -1;
            }
        }

        public override string ToString()
        {
            return $"Course ID: {Id}\nStart Date: {StartDate}\nEnd Date: {EndDate}\nCredit Hours: {CreditHours}\nCourse Name: {CourseName}\nCourse Description: {CourseDescription}\n";
        }
    }
}
