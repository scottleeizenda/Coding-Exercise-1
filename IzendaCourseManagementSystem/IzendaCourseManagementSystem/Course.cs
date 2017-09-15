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
        public List<Student> RegisteredStudents { get; set; } // this assumes one set of students per course

        public Course(int id, DateTime startDate, DateTime endDate, int creditHours, string courseName, string courseDescription)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            CreditHours = creditHours;
            CourseName = courseName;
            CourseDescription = courseDescription;
            RegisteredStudents = new List<Student>();
        }
        
        /// <summary>
        ///     Searches through the Course table to find a Course with an ID matching the param id. Returns a new Course
        ///     object with the same field values from the database upon success. Otherwise, returns null.
        /// </summary>
        public static Course SearchCourseById(SqlConnection connection, int id)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Course", connection);
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
        ///     Searches through the Student table to find a Student with an ID matching the param id. Returns a new Student
        ///     object with the same field values from the database upon success. Otherwise, returns null.
        /// </summary>
        public static Student SearchStudentById(SqlConnection connection, int id)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Student", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Student");
                set.Tables["Student"].Constraints.Add("Id_PK", set.Tables["Student"].Columns["Id"], true);

                DataRow row = set.Tables["Student"].Rows.Find(id);
                float gpa = float.Parse(row["GPA"].ToString());
                int hours = Int32.Parse(row["CreditHours"].ToString());

                return new Student(id, row["FirstName"].ToString(), row["LastName"].ToString(), row["UserName"].ToString(), row["Password"].ToString(), gpa, hours);
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
        public bool ViewRegisteredStudents(SqlConnection connection)
        {
            // TODO - check if table is empty for better user response

            // SELECT only the students registered for the specified course
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Student_Course WHERE CourseID = {this.Id}", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Student_Course");
                DataTable table = set.Tables["Student_Course"];

                Console.WriteLine("-----------------------------------------------------------------------------");
                foreach (DataRow row in table.Rows)
                {
                    // Lookup and show full Student information from StudentId
                    int currentStudentId = int.Parse(row["StudentId"].ToString());
                    //Console.WriteLine(SearchStudentById(connection, currentStudentId));
                    Console.WriteLine(User.SearchUserById(connection, currentStudentId, 3));
                }
                Console.WriteLine("-----------------------------------------------------------------------------");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"Course ID: {Id}\nStart Date: {StartDate}\nEnd Date: {EndDate}\nCredit Hours: {CreditHours}\nCourse Name: {CourseName}\nCourse Description: {CourseDescription}\n";
        }
    }
}
