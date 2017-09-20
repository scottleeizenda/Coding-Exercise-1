using System;
using System.Data;
using System.Data.SqlClient;

namespace IzendaCourseManagementSystem
{
    public class Instructor : User
    {
        // Extra declarations from a User
        public DateTime HireDate { get; set; }

        public Instructor(int id, string firstName, string lastName, DateTime hireDate, string userName, string password)
            : base (id, firstName, lastName, userName, password)
        {
            HireDate = hireDate;
            UserType = UserType.Instructor;
        }
        
        /// <summary>
        ///     Submits a final grade for a Student for a Course by inserting a new row into the CourseGrades and Student_CourseGrades
        ///     tables. The specified Student will then have his/her CreditHours updated if the final grade was at least a 'C'. Then
        ///     finally the course is deregistered for the Student by calling the Student's deregister method.
        ///     TODO - consider changing return type to int to create more meaningful user response
        ///          - also consider using transaction for all the above tasks
        /// </summary>
        /// <param name="connection">Connection object for the database</param>
        /// <param name="selectedStudent">Student to submit a final grade for</param>
        /// <param name="letterGrade">The letter grade for the CourseGrade</param>
        /// <param name="courseId">The ID of the Course the grade will be submitted for</param>
        /// <param name="creditHours">The Course's credit hour value to add to the Student's total hours</param>
        /// <param name="courseGradeId">The ID to assign as a PK for a new CourseGrades row in the DB</param>
        /// <returns>Returns true if all operations successfully go through, false otherwise</returns>
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
                    int status = selectedStudent.DeregisterCourse(connection, selectedStudent.Id, courseId);
                    if (status == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
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
        /// <param name="connection">Connection to the database</param>
        /// <param name="action">Number to represent what course of action for an Instructor to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public bool InstructorActionHandler(SqlConnection connection, int action)
        {
            if (action == 1) // view courses
            {
                this.ViewCoursesHandler(connection);
                return true;
            }
            else if (action == 2) // view assigned
            {
                int status = this.ViewCourses(connection, this.Id, 2);
                if (status == -1) { return false; }
                else { return true; }
            }
            else // submit final grades
            {
                // check if instructor has any courses assigned or error in database operation
                int coursesStatus = this.ViewCourses(connection, this.Id, 2);
                if (coursesStatus == 0)
                {
                    Console.WriteLine("You are not currently assigned to teach any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
                else if (coursesStatus == -1)
                {
                    Console.WriteLine("Failed to display assigned courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }

                // otherwise, proceed to getting user input on course to submit a grade for
                Course selectedCourse;
                Student selectedStudent;
                // query to retrieve full course info of courses assigned to this Instructor
                string assignedCoursesQuery = "SELECT Course.Id, Course.StartDate, Course.EndDate, Course.CreditHours, Course.CourseName, Course.CourseDescription FROM Course " +
                                             $"INNER JOIN Instructor_Course ON Course.Id = Instructor_Course.CourseId WHERE Instructor_Course.InstructorId = {this.Id}";
                //Console.WriteLine("-----------------------------------------------------------------------------");
                while (true)
                {
                    Console.WriteLine("Type the ID of the course you would like to submit final grades for");
                    Console.WriteLine("[Enter 'list' to see all your assigned courses]");
                    Console.Write("[Enter 'quit' to cancel]: ");
                    string input = Console.ReadLine();
                    int id;
                    if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ViewCourses(connection, this.Id, 2);
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        selectedCourse = Course.SearchCourseById(connection, assignedCoursesQuery, id);
                        if (selectedCourse != null)
                        {
                            Console.WriteLine($"Course {selectedCourse.CourseName} successfully found.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            // loop for user input on selecting a student to submit a grade for
                            while (true)
                            {
                                // check if there are registered students
                                if (selectedCourse.ViewRegisteredStudents(connection) == 0)
                                {
                                    Console.WriteLine("There are currently no students registered for this course.");
                                    Console.WriteLine("-----------------------------------------------------------------------------");
                                    break;
                                }
                                // otherwise, proceed to receive user input of Student to submit grade for
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
                                    selectedStudent = (Student)User.SearchUserById(connection, id, 3);
                                    if (selectedStudent != null)
                                    {
                                        Console.WriteLine($"Student {selectedStudent.FirstName} {selectedStudent.LastName} successfully found.");
                                        Console.WriteLine("-----------------------------------------------------------------------------");
                                        Console.Write("What letter grade would you like to give this student for this course?: ");
                                        string letterGrade = Console.ReadLine();
                                        bool status = this.SubmitFinalGrade(connection, selectedStudent, letterGrade, selectedCourse.Id, selectedCourse.CreditHours, Program.courseGradeIdNumber);
                                        if (status)
                                        {
                                            Program.courseGradeIdNumber++; // increment to make unique subsequent CourseGrades.Id's
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
