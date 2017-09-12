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
        ///     Submits a final grade for a Student for a Course by creating a new CourseGrade and adding it to the
        ///     Student's FinalGrades List. The Course is then deregistered from the Student's RegisteredCourses List.
        ///     A Student's CreditHours is also updated if the final grade is at least a 'C'.
        ///     Returns true upon successfully submitting the CourseGrade, false upon invalid grade input.
        /// </summary>
        public bool SubmitFinalGrade(Student selectedStudent, string letterGrade, int courseId, int courseGradeId)
        {
            if (letterGrade.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("B", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("C", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                // first update student's credit hours if final grade is at least a 'C'
                selectedStudent.FinalGrades.Add(new CourseGrades(courseGradeId, courseId, letterGrade.ToUpper()[0]));
                int index = Course.SearchCourseById(selectedStudent.RegisteredCourses, courseId);
                if (letterGrade.ToUpper() == "A" || letterGrade.ToUpper() == "B" || letterGrade.ToUpper() == "C")
                {
                    selectedStudent.CreditHours += selectedStudent.RegisteredCourses[index].CreditHours;
                    selectedStudent.Level = selectedStudent.CalculateLevel(selectedStudent.CreditHours);
                }

                // now deregister student regardless of what grade received
                selectedStudent.DeregisterCourse(index);
                return true;
            }
            return false;
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
        public bool InstructorActionHandler(List<Course> courses, SqlConnection connection, int action)
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
                // Check for empty assigned courses list
                if (this.AssignedCourses.Count == 0)
                {
                    Console.WriteLine("You currently have no courses assigned to you, so no final grades to submit.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }

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
                        //this.ViewCourses(this.AssignedCourses);
                        Console.WriteLine("DB needs to be implemented");
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        //int index = this.SearchAssignedCourseById(id);
                        int index = Course.SearchCourseById(this.AssignedCourses, id);
                        if (index >= 0)
                        {
                            selectedCourse = this.AssignedCourses[index];
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
                                    selectedCourse.ViewRegisteredStudents();
                                }
                                else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Cancelled, returning to course select...");
                                    Console.WriteLine("-----------------------------------------------------------------------------");
                                    break;
                                }
                                else if (Int32.TryParse(input, out id))
                                {
                                    index = selectedCourse.SearchStudentById(id);
                                    if (index >= 0)
                                    {
                                        selectedStudent = selectedCourse.RegisteredStudents[index];
                                        Console.WriteLine($"Student {selectedStudent.FirstName} {selectedStudent.LastName} successfully found.");
                                        Console.WriteLine("-----------------------------------------------------------------------------");
                                        Console.Write("What letter grade would you like to give this student for this course?: ");
                                        string letterGrade = Console.ReadLine();
                                        bool status = this.SubmitFinalGrade(selectedStudent, letterGrade, selectedCourse.Id, Program.courseGradeIdNumber);
                                        Program.courseGradeIdNumber++; // increment just to make unique subsequent CourseGrades.Id's
                                        if (status)
                                        {
                                            Console.WriteLine($"Final Grade '{letterGrade.ToUpper()}' has been submitted to for {selectedStudent.FirstName} {selectedStudent.LastName}");
                                            Console.WriteLine("-----------------------------------------------------------------------------");
                                            return true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Failed to submit final grade '{letterGrade.ToUpper()}', please use letters A-D or F");
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
