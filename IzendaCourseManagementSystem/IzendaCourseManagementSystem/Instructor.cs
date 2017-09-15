using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace IzendaCourseManagementSystem
{
    public class Instructor : User
    {
        // Extra declarations from a User
        public DateTime HireDate { get; set; }
        public List<Course> AssignedCourses { get; set; }

        public Instructor(int id, string firstName, string lastName, DateTime hireDate, string userName, string password)
            : base (id, firstName, lastName, userName, password)
        {
            HireDate = hireDate;
            UserType = "Instructor";
            AssignedCourses = new List<Course>();
        }

        /// <summary>
        ///     Displays the Courses that an Instructor is assigned to teach through the Instructor_Course table.
        ///     SELECT's and shows only the Courses for the specified Instructor from the instructorId param.
        ///     Returns true upon successfully displaying. Otherwise, returns false if anything goes wrong.
        /// </summary>
        public bool ViewAssignedCourses(SqlConnection connection, int instructorId)
        {
            // SELECT only the courses the specified Instructor teaches
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Instructor_Course WHERE InstructorID = {instructorId}", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Instructor_Course");
                DataTable table = set.Tables["Instructor_Course"];
                
                Console.WriteLine("-----------------------------------------------------------------------------");
                foreach (DataRow row in table.Rows)
                {
                    // Lookup and show full Course information from CourseId
                    int currentCourseId = int.Parse(row["CourseId"].ToString());
                    Console.WriteLine(Course.SearchCourseById(connection, currentCourseId));
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
        ///     Submits a final grade for a Student for a Course by inserting a new row into the CourseGrades and Student_CourseGrades
        ///     tables. The specified Student will then have his/her CreditHours updated if the final grade was at least a 'C'. Then
        ///     finally the course is deregistered for the Student by calling the Student's deregister method. Returns true upon
        ///     successfully completing all these tasks. Otherwise, returns false
        ///     TODO - consider changing return type to int to create more meaningful user response
        ///          - also consider using transaction for all the above tasks
        /// </summary>
        public bool SubmitFinalGrade(SqlConnection connection, Student selectedStudent, string letterGrade, int courseId, int creditHours, int courseGradeId)
        {
            if (letterGrade.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("B", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("C", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                // set up tools to manipulate database
                SqlDataAdapter courseGradesAdapter = new SqlDataAdapter("SELECT * FROM CourseGrades", connection);
                SqlDataAdapter studentAdapter = new SqlDataAdapter("SELECT * FROM Student", connection);
                SqlDataAdapter studentFinalGradesAdapter = new SqlDataAdapter("SELECT * FROM Student_CourseGrades", connection);
                SqlCommandBuilder courseBuilder = new SqlCommandBuilder(courseGradesAdapter);
                SqlCommandBuilder studentBuilder = new SqlCommandBuilder(studentAdapter);
                SqlCommandBuilder studentFinalGradesBuilder = new SqlCommandBuilder(studentFinalGradesAdapter);
                DataSet set = new DataSet();
                courseGradesAdapter.Fill(set, "CourseGrades");
                studentAdapter.Fill(set, "Student");
                studentFinalGradesAdapter.Fill(set, "Student_CourseGrades");

                // TODO - multiple database actions here, consider using Transaction
                
                try
                {
                    // INSERT new row into CourseGrades
                    DataRow row = set.Tables["CourseGrades"].NewRow();
                    row["Id"] = courseGradeId;
                    row["CourseId"] = courseId;
                    row["FinalGrade"] = letterGrade.ToUpper();
                    set.Tables["CourseGrades"].Rows.Add(row);
                    courseGradesAdapter.Update(set.Tables["CourseGrades"]);

                    // INSERT new row into Student_CourseGrades
                    row = set.Tables["Student_CourseGrades"].NewRow();
                    row["Id"] = courseGradeId;
                    row["StudentId"] = selectedStudent.Id;
                    row["CourseGradesId"] = courseGradeId;
                    set.Tables["Student_CourseGrades"].Rows.Add(row);
                    studentFinalGradesAdapter.Update(set.Tables["Student_CourseGrades"]);

                    // UPDATE Student's CreditHours if final grade is at least a 'C'
                    if (letterGrade.ToUpper() == "A" || letterGrade.ToUpper() == "B" || letterGrade.ToUpper() == "C")
                    {
                        set.Tables["Student"].Constraints.Add("Id_PK", set.Tables["Student"].Columns["Id"], true);
                        row = set.Tables["Student"].Rows.Find(selectedStudent.Id);
                        row.BeginEdit();
                        int newHours = int.Parse(row["CreditHours"].ToString()) + creditHours;
                        row["CreditHours"] = newHours;
                        row.EndEdit();
                        studentAdapter.Update(set.Tables["Student"]);

                        // TODO - recalculate student level and GPA
                    }

                    // now deregister student regardless of what grade received
                    Console.WriteLine("Attempting to deregister course...");
                    return selectedStudent.DeregisterCourse(connection, selectedStudent.Id, courseId);

                    //return true;
                }
                catch
                {
                    return false;
                }
            }
            return false; // invalid letterGrade
        }

        /// <summary>
        ///     Handles an Instructor action specified by user input from Main. Database not yet implemented,
        ///     so data comes from Lists created in Main. The check for a valid action number also done in Main.
        ///     Action #1 = View all courses (Database SELECT, may expand later to view specific courses)
        ///     Action #2 = View all courses the Instructor is assigned to teach (will later deal with a database)
        ///     Action #3 = Submit a final grade for a Student (Deregisters Student from Course upon submission,
        ///                 and updates the Student's CreditHours if final grade is at least a 'C')
        /// </summary>
        /// <param name="courses">List of all existing courses</param>
        /// <param name="action">Number to represent what course of action for an Instructor to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public bool InstructorActionHandler(SqlConnection connection, int action)
        {
            if (action == 1) // view courses
            {
                bool status = this.ViewCourses(connection);
                if (!status)
                {
                    Console.WriteLine("There are currently no existing courses.");
                }
                return status;
            }
            else if (action == 2) // view assigned
            {
                bool status = this.ViewAssignedCourses(connection, this.Id);
                if (!status)
                {
                    Console.WriteLine("You are not currently assigned to teach any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }
                return status;
            }
            else // submit final grades
            {
                // TODO - Check for empty assigned courses list

                Course selectedCourse;
                Student selectedStudent;
                Console.WriteLine("-----------------------------------------------------------------------------");
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
                        this.ViewAssignedCourses(connection, this.Id);
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        selectedCourse = Course.SearchCourseById(connection, id);
                        if (selectedCourse != null)
                        {
                            Console.WriteLine($"Course {selectedCourse.CourseName} successfully found.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            // loop for user input on selecting a student to submit a grade for
                            while (true)
                            {
                                Console.WriteLine("Type the ID of the student you would like to submit a final grade for");
                                Console.WriteLine("[Enter 'list' to see all students registered for this course]");
                                Console.Write("[Enter 'quit' to cancel]: ");
                                input = Console.ReadLine();
                                if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                                {
                                    selectedCourse.ViewRegisteredStudents(connection);
                                }
                                else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Cancelled, returning to course select...");
                                    Console.WriteLine("-----------------------------------------------------------------------------");
                                    break;
                                }
                                else if (Int32.TryParse(input, out id))
                                {
                                    selectedStudent = Course.SearchStudentById(connection, id);
                                    if (selectedStudent != null)
                                    {
                                        Console.WriteLine($"Student {selectedStudent.FirstName} {selectedStudent.LastName} successfully found.");
                                        Console.WriteLine("-----------------------------------------------------------------------------");
                                        Console.Write("What letter grade would you like to give this student for this course?: ");
                                        string letterGrade = Console.ReadLine();
                                        bool status = this.SubmitFinalGrade(connection, selectedStudent, letterGrade, selectedCourse.Id, selectedCourse.CreditHours, Program.courseGradeIdNumber);
                                        Program.courseGradeIdNumber++; // increment to make unique subsequent CourseGrades.Id's
                                        if (status)
                                        {
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

        public override string ToString()
        {
            return $"Instructor ID: {Id}\nName: {LastName}, {FirstName}\nHire Date: {HireDate}\nUser Name: {UserName}\nPassword: {Password}\n";
        }
    }
}
