using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IzendaCourseManagementSystem
{
    public class Student : User
    {
        // Extra declarations from a User
        public float GPA { get; set; }
        public int CreditHours { get; set; }
        public Level Level { get; set; }

        public Student(int id, string firstName, string lastName, string userName, string password, float gpa, int creditHours)
            : base(id, firstName, lastName, userName, password)
        {
            GPA = gpa;
            CreditHours = creditHours;
            Level = CalculateLevel(creditHours);
            UserType = UserType.Student;
        }
        
        /// <summary>
        ///     Takes in the number of CreditHours for a Student and returns a string representing
        ///     the Student's grade Level. Returns null if hours is negative.
        /// </summary>
        /// <param name="hours">The total credit hours to convert to a certain grade level</param>
        /// <returns>A Level enum representing the Student's grade level</returns>
        public Level CalculateLevel(int hours)
        {
            if (hours >= 0 && hours < 30) { return Level.Freshman; }
            else if (hours >= 30 && hours < 60) { return Level.Sophomore; }
            else if (hours >= 60 && hours < 90) { return Level.Junior; }
            else if (hours >= 90) { return Level.Senior; }
            else { return Level.Undefined; }
        }
        
        /// <summary>
        ///     Displays all CourseGrades the student has earned. Accesses this information through an INNER JOIN query with
        ///     the CourseGrades and Student_CourseGrades tables.
        /// </summary>
        /// <param name="connection">Connection object for the database</param>
        /// <returns>
        ///     Returns the number of rows that were successfully displayed if nothing goes wrong, including 0 for empty table.
        ///     Returns -1 otherwise if a database operation goes wrong.
        /// </returns>
        public int ViewFinalGrades(SqlConnection connection)
        {
            try
            {
                string query = "SELECT CourseGrades.CourseId, CourseGrades.FinalGrade, Student_CourseGrades.StudentId FROM CourseGrades " +
                              $"INNER JOIN Student_CourseGrades ON CourseGrades.Id = Student_CourseGrades.CourseGradesId WHERE Student_CourseGrades.StudentId = {this.Id}";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Student_CourseGrades_JOIN");
                DataTable table = set.Tables["Student_CourseGrades_JOIN"];
                int numRows = table.Rows.Count;

                // Check if table is empty
                if (numRows == 0)
                {
                    return 0;
                }

                // Otherwise display all rows
                Console.WriteLine("-----------------------------------------------------------------------------");
                foreach (DataRow row in table.Rows)
                {
                    // Lookup and show Course information from CourseId
                    int currentCourseId = int.Parse(row["CourseId"].ToString());
                    Course currentCourse = Course.SearchCourseById(connection, null, currentCourseId);
                    Console.WriteLine($"{currentCourse.CourseName} --> {row["FinalGrade"]}");
                }
                Console.WriteLine("-----------------------------------------------------------------------------");

                return numRows;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }

        /// <summary>
        ///     Adds the Course specified by the param courseId to the Student_Course table. Courses that a Student is already registered
        ///     for or already completed will not be added.
        /// </summary>
        /// <param name="connection">Connection object to the database</param>
        /// <param name="courseId">The ID of the Course to register for</param>
        /// <returns>
        ///     Returns 1 if all checks are passed and Course is successfully registered.
        ///     Returns -1 if a duplicate course is found in a Student's registered courses (Student_Course table).
        ///     Returns -2 if the Student has already completed the Course (checked from Student_CourseGrades table).
        ///     Returns -3 if a database operation goes wrong at any point.
        /// </returns>
        public int RegisterCourse(SqlConnection connection, int courseId)
        {
            // Consider improvement by allowing student to take course again if student made below a 'C',
            // would need an UPDATE in CourseGrades
            
            try
            {
                // Check if student is already registered for this course
                SqlDataAdapter checkAdapter = new SqlDataAdapter($"SELECT StudentId, CourseId FROM Student_Course WHERE StudentId = {this.Id}", connection);
                SqlCommandBuilder checkBuilder = new SqlCommandBuilder(checkAdapter);
                DataSet checkSet = new DataSet();
                checkAdapter.Fill(checkSet, "RegisteredCourses");
                DataTable checkTable = checkSet.Tables["RegisteredCourses"];
                foreach (DataRow dr in checkTable.Rows)
                {
                    if (int.Parse(dr["CourseId"].ToString()) == courseId)
                    {
                        return -1;
                    }
                }

                // Check if student has already taken this course
                string query = "SELECT CourseGrades.CourseId, CourseGrades.FinalGrade, Student_CourseGrades.StudentId FROM CourseGrades " +
                              $"INNER JOIN Student_CourseGrades ON CourseGrades.Id = Student_CourseGrades.CourseGradesId WHERE Student_CourseGrades.StudentId = {this.Id}";
                checkAdapter = new SqlDataAdapter(query, connection);
                checkBuilder = new SqlCommandBuilder(checkAdapter);
                checkAdapter.Fill(checkSet, "Student_CourseGrades_JOIN");
                checkTable = checkSet.Tables["Student_CourseGrades_JOIN"];
                foreach (DataRow dr in checkTable.Rows)
                {
                    if (int.Parse(dr["CourseId"].ToString()) == courseId)
                    {
                        return -2;
                    }
                }

                // Otherwise, good to register for course by inserting new row
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Student_Course", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Student_Course");
                
                DataRow row = set.Tables["Student_Course"].NewRow();
                row["Id"] = Program.registerCourseIdNumber;
                Program.registerCourseIdNumber++;
                row["StudentId"] = this.Id;
                row["CourseId"] = courseId;
                set.Tables["Student_Course"].Rows.Add(row);
                adapter.Update(set.Tables[0]);

                return 1;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return -3;
            }
        }

        /// <summary>
        ///     Removes the Course specified by the courseId param from the Student_Course table.
        /// </summary>
        /// <param name="connection">Connection object to the database</param>
        /// <param name="studentId">ID of the Student that will be deregistering a Course</param>
        /// <param name="courseId">ID of the Course the Student will deregister</param>
        /// <returns>
        ///     Returns 1 upon successfully deleting row.
        ///     Returns 0 if the table is empty (Student not registered for any courses).
        ///     Returns -1 if course is not found in the DataSet (Student not registered for specified course).
        ///     Returns -2 if a database operation went wrong.
        /// </returns>
        public int DeregisterCourse(SqlConnection connection, int studentId, int courseId)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM Student_Course WHERE StudentID = {studentId}", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Student_Course");
                set.Tables["Student_Course"].Constraints.Add("Id_PK", set.Tables["Student_Course"].Columns["Id"], true);
                DataTable table = set.Tables["Student_Course"];
                int numRows = table.Rows.Count;
                // nothing to delete if table is empty
                if (numRows == 0)
                {
                    return 0;
                }

                int currentCourseId;
                foreach (DataRow row in table.Rows)
                {
                    currentCourseId = int.Parse(row["CourseId"].ToString());
                    // if the traversing CourseId == CourseId of the course to deregister
                    if (currentCourseId == courseId)
                    {
                        int id = int.Parse(row["Id"].ToString());
                        set.Tables["Student_Course"].Rows.Find(id).Delete();
                        adapter.Update(set.Tables["Student_Course"]);
                        return 1;
                    }
                }
                // if loop finishes, course not found in registered courses and no delete done
                return -1;
            }
            catch (InvalidOperationException ioe)
            {
                Console.Write(ioe);
                return -2;
            }
            catch (ArgumentNullException ane)
            {
                Console.Write(ane);
                return -2;
            }
            catch (DBConcurrencyException dbce)
            {
                Console.Write(dbce);
                return -2;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return -2;
            }
        }

        /// <summary>
        ///     Handles a Student action specified by user input from Main. Database not yet implemented,
        ///     so data comes from Lists created in Main. The check for a valid action number also done in Main.
        ///     Action #1 = View all courses (Database SELECT, may expand later to view specific courses)
        ///     Action #2 = View all courses the Student is registered for (will later deal with a database)
        ///     Action #3 = View the Student's information (Uses its ToString method)
        ///     Action #4 = View the Student's final grades (will need database CourseGrades table)
        ///     Action #5 = Register for a Course (adds to RegisteredCourses List and RegisteredStudents List in Course)
        ///     Action #6 = Deregister a Course (removes Course from RegisteredCourses List and Registered Students List in Course)
        /// </summary>
        /// <param name="connection">Connection to the database</param>
        /// <param name="action">Number to represent what course of action for a Student to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public bool StudentActionHandler(SqlConnection connection, int action)
        {
            if (action == 1) // view courses
            {
                this.ViewCoursesHandler(connection);
                return true;
            }
            else if (action == 2) // view registered
            {
                //int status = this.ViewRegisteredCourses(connection);
                int status = this.ViewCourses(connection, this.Id, 3);
                if (status == 0)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("You are not currently registered for any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return true;
                }
                else if (status == -1)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Failed to display registered courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
                // else registered courses have successfully displayed
                return true;
            }
            else if (action == 3) // view info
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.WriteLine(this);
                Console.WriteLine("-----------------------------------------------------------------------------");
                return true;
            }
            else if (action == 4) // view grades
            {
                int status = this.ViewFinalGrades(connection);
                if (status == 0)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("You have not completed any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return true;
                }
                else if (status == -1)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Failed to display grades.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
                // else grades have successfully displayed
                return true;
            }
            else if (action == 5) // register course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to register for: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    Course selectedCourse = Course.SearchCourseById(connection, null, id);
                    if (selectedCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found, adding this course to your registered courses:\n{selectedCourse}");
                        int status = this.RegisterCourse(connection, selectedCourse.Id);
                        if (status == 1)
                        {
                            Console.WriteLine("Course successfully registered.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return true;
                        }
                        else if (status == -1)
                        {
                            Console.WriteLine("You are already registered for this course!");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                        else if (status == -2)
                        {
                            Console.WriteLine("You have already completed this course!");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                        else if (status == -3)
                        {
                            Console.WriteLine("Database error, failed to register for course.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id}");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
            }
            else // deregister course
            {
                // query to retrieve full course info of courses this Student is registered for
                //string query = "SELECT Course.Id, Course.StartDate, Course.EndDate, Course.CreditHours, Course.CourseName, Course.CourseDescription FROM Course " +
                //              $"INNER JOIN Student_Course ON Course.Id = Student_Course.CourseId WHERE Student_Course.StudentId = {this.Id}";
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to deregister: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    Course selectedCourse = Course.SearchCourseById(connection, null, id);
                    if (selectedCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found, attempting to remove this course from your registered courses:\n{selectedCourse}");
                        int status = this.DeregisterCourse(connection, this.Id, id);
                        if (status == 1)
                        {
                            Console.WriteLine("Course successfully deregistered.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return true;
                        }
                        else if (status == 0)
                        {
                            Console.WriteLine("You currently have no courses to deregister.");
                        }
                        else if (status == -1)
                        {
                            Console.WriteLine("You are not currently registered for this course.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to deregister course.");
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID '{id}'.");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID input");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
            }
        }

        public override string ToString()
        {
            return $"Student Id: {Id}\nName: {LastName}, {FirstName}\nCredit Hours Earned: {CreditHours}\nGPA: {GPA}\nGrade Level: {Level}\n";
        }
    }
}
