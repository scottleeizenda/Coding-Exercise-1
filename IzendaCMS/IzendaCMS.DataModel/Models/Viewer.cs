using System;
using System.Collections.Generic;
using System.Linq;

namespace IzendaCMS.DataModel.Models
{
    public class Viewer
    {
        /// <summary>
        ///     Overloaded Method. Displays available courses from the Course table depending on the prefix parameter.
        ///     If prefix is null, displays all existing courses. Otherwise, displays courses filtered by the prefix.
        /// </summary>
        /// <param name="prefix">1-4 letter prefix to filter courses by (filters by CourseName), can be null</param>
        /// <returns>Returns number of rows printed from the PerformViewCourses method, otherwise returns -1</returns>
        public static int ViewCourses(string prefix)
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

            return PerformViewCourses(query);
        }

        /// <summary>
        ///     Overloaded method. Takes in userId that signifies either an Instructor's ID or a Student's ID.
        ///     Parameter userType will specify what type of ID to read userId as where '2' is for Instructor and
        ///     '3' is for Student. Intaking an Instructor ID will pass a query to the PerformViewCourses method
        ///     to print an Instructor's assigned courses. Intaking a Student ID will pass a query to print the
        ///     specified Student's registered courses.
        /// </summary>
        /// <param name="userId">ID of an Instructor or a Student, should correspond with userType param</param>
        /// <param name="userType">Int specifying which query to use, '2' for Instructor assigned courses, '3' for Student registered courses</param>
        /// <returns>Returns number of rows printed from the PerformViewCourses method, otherwise returns -1</returns>
        public static int ViewCourses(int userId, int userType)
        {
            // queries for retrieving full course info of courses assigned to an Instructor or registered courses of a Student
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

            return PerformViewCourses(query);
        }

        /// <summary>
        ///     This method works in conjunction with one of the overloaded ViewCourses methods. The ViewCourses methods
        ///     take care of the correct SELECT statement to perform, and this method accesses the database with it.
        /// </summary>
        /// <param name="query">Query received from one of the overloaded ViewCourses methods</param>
        /// <returns>
        ///     Returns the number of rows printed, including 0 if the table was empty.
        ///     Returns -1 otherwise, if a database operation goes wrong.
        /// </returns>
        private static int PerformViewCourses(string query)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    List<Course> courses = context.Courses.SqlQuery(query).ToList();
                    int numRows = courses.Count;

                    // Check if table is empty
                    if (numRows == 0)
                    {
                        return 0;
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------");
                    foreach (Course c in courses)
                    {
                        Console.WriteLine(c);
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------");

                    return numRows;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }
        }

        /// <summary>
        ///     Displays all available distinct 4-letter course prefixes from the CourseName column in the Course table.
        /// </summary>
        /// <returns>Returns true upon successfully printing, otherwise returns false if database operation goes wrong</returns>
        public static bool ViewCoursePrefix()
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    string query = "SELECT DISTINCT SUBSTRING(CourseName, 0, 5) AS CoursePrefix FROM Course";
                    List<string> prefixes = context.Database.SqlQuery<string>(query).ToList();

                    Console.WriteLine("-----------------------------------------------------------------------------");
                    foreach (string p in prefixes)
                    {
                        Console.WriteLine(p);
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------");

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }

        /// <summary>
        ///     Handles user input in how he/she wants to display the courses currently in the database Course table.
        /// </summary>
        /// <param name="connection">Connection object to the database</param>
        /// <returns>Returns true upon selection to return to main menu</returns>
        public static bool ViewCoursesHandler()
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
                        ViewCourses(null);
                    }
                    else if (input == 2) // view prefix-filtered
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        Console.WriteLine("Here is a list of available course prefixes:");
                        ViewCoursePrefix();
                        Console.Write("Enter the Course prefix you would like to search for: ");
                        string prefix = Console.ReadLine();
                        int status = ViewCourses(prefix);
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
                            Course course = Utilities.SearchCourseById(null, courseId);
                            if (course == null) { Console.WriteLine($"No course found under ID of '{courseId}'"); }
                            else { Console.WriteLine(course); }
                            //Console.WriteLine(SearchCourseById(null, courseId));
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID input");
                            continue;
                        }
                    }
                    else if (input == 4) // go back to previous menu
                    {
                        return true;
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
        ///     Displays all rows of a certain type of user from the database. Displays info from their respective
        ///     ToString methods.
        /// </summary>
        /// <param name="userType">Determines the query to determine which DB table to search in</param>
        /// <returns>
        ///     Returns the number of rows printed, including 0 if table was empty.
        ///     Returns -1 otherwise, if a database operation went wrong or invalid userType.
        /// </returns>
        public static int ViewUsers(int userType)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                /*string query;
                if (userType == 1) { query = "SELECT * FROM Administrator"; }
                else if (userType == 2) { query = "SELECT * FROM Instructor"; }
                else if (userType == 3) { query = "SELECT * FROM Student"; }
                else { return -1; }*/

                // TODO - find better way to condense

                try
                {
                    if (userType == 1)
                    {
                        List<Administrator> admins = context.Administrators.ToList();
                        // check for empty table
                        int numRows = admins.Count;
                        if (numRows == 0) { return 0; }

                        Console.WriteLine("-----------------------------------------------------------------------------");
                        foreach (Administrator a in admins)
                        {
                            Console.WriteLine(a);
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");

                        return numRows;
                    }
                    else if (userType == 2)
                    {
                        List<Instructor> instructors = context.Instructors.ToList();
                        // check for empty table
                        int numRows = instructors.Count;
                        if (numRows == 0) { return 0; }

                        Console.WriteLine("-----------------------------------------------------------------------------");
                        foreach (Instructor i in instructors)
                        {
                            Console.WriteLine(i);
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");

                        return numRows;
                    }
                    else if (userType == 3)
                    {
                        List<Student> students = context.Students.ToList();
                        // check for empty table
                        int numRows = students.Count;
                        if (numRows == 0) { return 0; }

                        Console.WriteLine("-----------------------------------------------------------------------------");
                        foreach (Student s in students)
                        {
                            Console.WriteLine(s);
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");

                        return numRows;
                    }
                    else { return -1; } // invalid user type
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }
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
        public static int ViewFinalGrades(int studentId)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    // do method with LINQ join statement
                    /*var finalGrades = (from cg in context.CourseGrades
                                       join scg in context.Student_CourseGrades on cg.Id equals scg.CourseGradesId
                                       where scg.StudentId == studentId
                                       select new
                                       {
                                           courseId = cg.CourseId,
                                           finalGrade = cg.FinalGrade,
                                       });
                    int numRows = finalGrades.ToList().Count;

                    // Check if table is empty
                    if (numRows == 0) { return 0; }

                    // Otherwise display all rows
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    foreach (var grade in finalGrades)
                    {
                        // Lookup and show Course information from CourseId
                        Course currentCourse = Utilities.SearchCourseById(null, grade.courseId);
                        Console.WriteLine($"{currentCourse.CourseName} --> {grade.finalGrade}");
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------");

                    return numRows;*/

                    // do method with explicity-loaded related data
                    Student currentStudent = context.Students.Find(studentId);
                    context.Entry(currentStudent).Collection(s => s.Student_CourseGrades).Load();

                    // check if table is empty
                    int numRows = currentStudent.Student_CourseGrades.Count;
                    if (numRows == 0) { return 0; }

                    // otherwise display the rows
                    foreach (Student_CourseGrades scg in currentStudent.Student_CourseGrades)
                    {
                        // lookup necessary info to display more meaningful info than ID numbers
                        CourseGrade cg = context.CourseGrades.Find(scg.CourseGradesId);
                        Course c = Utilities.SearchCourseById(null, cg.CourseId);
                        Console.WriteLine($"{c.CourseName} --> {cg.FinalGrade}");
                    }

                    return numRows;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }
        }

        /// <summary>
        ///     Displays all the Students that are registered for a specific course by displaying the rows in the
        ///     Student_Course table where CourseId matches the Course calling object's Id. The displayed information
        ///     each Student's info from their ToString() method. Returns true upon successfully displaying all
        ///     entries. Otherwise, returns false.
        /// </summary>
        /// <param name="connection">Connection object to the database</param>
        /// <returns>
        ///     Returns number of rows printed, including 0 if table is empty
        ///     Returns -1 otherwise, if a database operation went wrong
        /// </returns>
        public static int ViewRegisteredStudents(int courseId)
        {
            // SELECT only the students registered for the specified course
            string query = $"SELECT * FROM Student_Course WHERE CourseID = {courseId}";

            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    List<Student_Course> registeredStudents = context.Student_Course.SqlQuery(query).ToList();
                    int numRows = registeredStudents.Count;
                    // check if empty
                    if (numRows == 0)
                    {
                        return 0;
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------");
                    foreach (Student_Course sc in registeredStudents)
                    {
                        // Search for student to display full student info rather than just ID
                        Console.WriteLine((Student)Utilities.SearchUserById(sc.StudentId, 3));
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------");

                    return numRows;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }
        }

        /// <summary>
        ///     Displays all Instructor-to-Course assignments from the Instructor_Course table
        /// </summary>
        /// <returns>Returns the number of rows displayed, or -1 if database operation goes wrong</returns>
        public static int ViewAssignedInstructors()
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    List<Instructor_Course> assignedInstructors = context.Instructor_Course.ToList();
                    // TODO - consider sorting the list
                    // check for empty table
                    int numRows = assignedInstructors.Count;
                    if (numRows == 0) { return 0; }

                    Console.WriteLine("-----------------------------------------------------------------------------");
                    foreach (Instructor_Course ic in assignedInstructors)
                    {
                        // Search for instructor and course to display more meaningful info than ID numbers
                        Instructor currentInstructor = (Instructor)Utilities.SearchUserById(ic.InstructorId, 2);
                        Course currentCourse = Utilities.SearchCourseById(null, ic.CourseId);
                        Console.WriteLine($"[{currentInstructor.Id}] {currentInstructor.FirstName} {currentInstructor.LastName} --> [{currentCourse.Id}] {currentCourse.CourseName}");
                    }
                    Console.WriteLine("-----------------------------------------------------------------------------");

                    return numRows;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }
        }
    }
}
