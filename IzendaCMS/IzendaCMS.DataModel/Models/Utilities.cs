using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCMS.DataModel.Models
{
    public class Utilities
    {
        // Declarations
        public static Administrator CurrentAdmin;
        public static Instructor CurrentInstructor;
        public static Student CurrentStudent;
        public static int CourseGradeIdNumber;
        public static int AssignInstructorIdNumber;
        public static int RegisterCourseIdNumber;

        /// <summary>
        ///    Checks if all fields in courseFields are valid, then proceeds to create and add a
        ///    new Course in the Course table of the database (INSERT INTO).
        /// </summary>
        /// <param name="courseFields">Array of strings to fill in for column values of the new Course row</param>
        /// <returns>Returns true upon successful INSERT INTO the database, otherwise returns false</returns>
        public static bool CreateCourse(string[] courseFields)
        {
            // courseFields should be exactly 6 elements, matches exactly with Course columns in DB
            if (courseFields.Length != 6)
            {
                return false;
            }
            
            int id, hours;
            DateTime startDate, endDate;

            // return false upon an invalid input field
            if (!Int32.TryParse(courseFields[0], out id))
            {
                return false;
            }
            if (!DateTime.TryParse(courseFields[1], out startDate))
            {
                return false;
            }
            if (!DateTime.TryParse(courseFields[2], out endDate))
            {
                return false;
            }
            if (!Int32.TryParse(courseFields[3], out hours))
            {
                return false;
            }
            // make sure "CourseName" and "CourseDescription" fields aren't too big for DB
            if (courseFields[4].Length > 50)
            {
                return false;
            }
            if (courseFields[5].Length > 100)
            {
                return false;
            }

            // if all fields good, proceed to INSERT INTO Course table
            Course course = new Course
            {
                Id = id,
                StartDate = startDate,
                EndDate = endDate,
                CreditHours = hours,
                CourseName = courseFields[4],
                CourseDescription = courseFields[5]
            };
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    context.Courses.Add(course);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        ///     Checks if all fields in courseFields are valid, where a blank ("") element means to keep that field the same as before.
        ///     If no problems with the fields, proceeds to do an UPDATE to the Course table in DB with the new specified fields.
        /// </summary>
        /// <param name="currentId">ID of the Course to perform an UPDATE on</param>
        /// <param name="courseFields">Array of strings containing column values to change the values in a Course row</param>
        /// <returns>Returns true upon a successful UPDATE, otherwise returns false if database operation goes wrong or invalid course field.</returns>
        public static bool UpdateCourse(int currentId, string[] courseFields)
        {
            // courseFields should be exactly 6 elements, matches exactly with Course columns in DB
            if (courseFields.Length != 6)
            {
                return false;
            }
            
            int hours;
            DateTime startDate, endDate;
            Course currentCourse = SearchCourseById(null, currentId);

            // check all fields' validity first before attempting to update database, return false upon an invalid input field

            // if blank courseField detected, keep field same as the one before. Otherwise, check for validity of non-string fields
            // skip courseFields[0] because Primary Key Id shouldn't be changed
            if (courseFields[1] == "")
            {
                startDate = currentCourse.StartDate??DateTime.Now;
            }
            else if (!DateTime.TryParse(courseFields[1], out startDate))
            {
                return false;
            }
            if (courseFields[2] == "")
            {
                endDate = currentCourse.EndDate??DateTime.Now;
            }
            else if (!DateTime.TryParse(courseFields[2], out endDate))
            {
                return false;
            }
            if (courseFields[3] == "")
            {
                hours = currentCourse.CreditHours;
            }
            else if (!Int32.TryParse(courseFields[3], out hours))
            {
                return false;
            }
            // string fields don't need the TryParse, but will check if string length will fit into database
            if (courseFields[4] == "")
            {
                courseFields[4] = currentCourse.CourseName;
            }
            else if (courseFields[4].Length > 50)
            {
                return false;
            }
            if (courseFields[5] == "")
            {
                courseFields[5] = currentCourse.CourseDescription;
            }
            else if (courseFields[5].Length > 100)
            {
                return false;
            }

            // All fields should be valid at this point, update to database now
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    Course course = context.Courses.Find(currentId);
                    course.StartDate = startDate;
                    course.EndDate = endDate;
                    course.CreditHours = hours;
                    course.CourseName = courseFields[4];
                    course.CourseDescription = courseFields[5];

                    context.Courses.Attach(course);
                    context.Entry(course).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

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
        ///     Method used mainly for Administrator actions 'CreateCourse' and 'UpdateCourse'. Received user input
        ///     for course fields to fill out for a row in the Course table. Console output messages differ slightly
        ///     depending on actionType parameter which just signifies what database action is the messages for.
        /// </summary>
        /// <param name="actionType">Int specifying an insert (1) or an update update (2) action</param>
        /// <returns>Array of strings containing the column values for a Course</returns>
        public static string[] ReceiveCourseFields(int actionType)
        {
            string extra;
            if (actionType == 2) { extra = "new "; }
            else { extra = ""; }

            if (actionType == 2) { Console.WriteLine("[Leave a field blank if you wish to keep it the same as before]"); }
            string[] courseFields = new string[6];

            // if updating, don't allow changing Id (PK)
            if (actionType == 1)
            {
                Console.Write($"Enter the course ID: ");
                courseFields[0] = Console.ReadLine();
            }
            else
            {
                courseFields[0] = "";
            }

            Console.Write($"Enter the {extra}course start date: ");
            courseFields[1] = Console.ReadLine();
            Console.Write($"Enter the {extra}course end date: ");
            courseFields[2] = Console.ReadLine();
            Console.Write($"Enter the {extra}course credit hours: ");
            courseFields[3] = Console.ReadLine();
            Console.Write($"Enter the {extra}course name: ");
            courseFields[4] = Console.ReadLine();
            Console.Write($"Enter the {extra}course description: ");
            courseFields[5] = Console.ReadLine();

            return courseFields;
        }
        
        /// <summary>
        ///     Deletes a Course from the Course table in the database.
        /// </summary>
        /// <param name="courseId">ID of the Course to delete from database table</param>
        /// <returns>Returns true upon a successful DELETE, otherwise returns false if database operation goes wrong.</returns>
        public static bool DeleteCourse(int courseId)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    // TODO - decide how to handle deletion of Course with foreign key references to it

                    Course course = Utilities.SearchCourseById(null, courseId);
                    context.Entry(course).State = System.Data.Entity.EntityState.Deleted;
                    context.SaveChanges();

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
                        if (numRows == 0)
                        {
                            return 0;
                        }

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
                        if (numRows == 0)
                        {
                            return 0;
                        }

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
                        if (numRows == 0)
                        {
                            return 0;
                        }

                        Console.WriteLine("-----------------------------------------------------------------------------");
                        foreach (Student s in students)
                        {
                            Console.WriteLine(s);
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");

                        return numRows;
                    }
                    else
                        return -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }
        }

        /// <summary>
        ///     Assigns an Instructor to a Course by adding an entry to the Instructor_Course associative table.
        ///     The table has a pairing of an Instructor's ID and Course's ID.
        /// </summary>
        /// <param name="selectedCourse">Course that an Instructor will be assigned to teach</param>
        /// <param name="selectedInstructor">Instructor that will be assign to teach a Course</param>
        /// <returns>Returns true upon a successful INSERT, otherwise returns false from database operation gone wrong</returns>
        public static bool AssignInstructor(Course selectedCourse, Instructor selectedInstructor)
        {
            // TODO? - Prevent any duplicate entries? May not be necessary if Instructor is allowed to teach multiple
            //         classes of the same Course.
            
            // construct new Instructor_Course object for new row into respective table
            Instructor_Course assignedCourse = new Instructor_Course
            {
                Id = AssignInstructorIdNumber,
                InstructorId = selectedInstructor.Id,
                CourseId = selectedCourse.Id
            };

            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    context.Instructor_Course.Add(assignedCourse);
                    context.SaveChanges();

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
        ///     Submits a final grade for a Student for a Course by inserting a new row into the CourseGrades and Student_CourseGrades
        ///     tables. The specified Student will then have his/her CreditHours updated if the final grade was at least a 'C'. Then
        ///     finally the course is deregistered for the Student by calling the Student's deregister method.
        ///     TODO - consider changing return type to int to create more meaningful user response
        ///          - also consider using transaction for all the above tasks
        /// </summary>
        /// <param name="selectedStudent">Student to submit a final grade for</param>
        /// <param name="letterGrade">The letter grade for the CourseGrade</param>
        /// <param name="courseId">The ID of the Course the grade will be submitted for</param>
        /// <param name="creditHours">The Course's credit hour value to add to the Student's total hours</param>
        /// <param name="courseGradeId">The ID to assign as a PK for a new CourseGrades row in the DB</param>
        /// <returns>Returns true if all operations successfully go through, false otherwise</returns>
        public static bool SubmitFinalGrade(Student selectedStudent, string letterGrade, int courseId, int creditHours, int courseGradeId)
        {
            if (letterGrade.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("B", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("C", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("D", StringComparison.OrdinalIgnoreCase) ||
                letterGrade.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                using (IzendaCMSContext context = new IzendaCMSContext())
                {
                    try
                    {
                        // INSERT new row into CourseGrades
                        context.CourseGrades.Add(new CourseGrade(Utilities.CourseGradeIdNumber, courseId, letterGrade.ToUpper()));

                        // INSERT new row into Student_CourseGrades
                        context.Student_CourseGrades.Add(new Student_CourseGrades(courseGradeId, selectedStudent.Id, courseGradeId));

                        // UPDATE Student's CreditHours if final grade is at least a 'C'
                        if (letterGrade.ToUpper() == "A" || letterGrade.ToUpper() == "B" || letterGrade.ToUpper() == "C")
                        {
                            selectedStudent.CreditHours += creditHours;
                            context.Students.Attach(selectedStudent);
                            context.Entry(selectedStudent).State = System.Data.Entity.EntityState.Modified;

                            // TODO - recalculate student level and GPA
                        }

                        // now deregister student regardless of what grade received
                        Console.WriteLine("Student info updated, now attempting to deregister student from this course...");
                        int status = Utilities.DeregisterCourse(selectedStudent.Id, courseId);
                        if (status == 1)
                        {
                            context.SaveChanges();
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
            }
            return false; // invalid letterGrade
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
                    //string query = "SELECT CourseGrades.CourseId, CourseGrades.FinalGrade, Student_CourseGrades.StudentId FROM CourseGrades " +
                    //              $"INNER JOIN Student_CourseGrades ON CourseGrades.Id = Student_CourseGrades.CourseGradesId WHERE Student_CourseGrades.StudentId = {studentId}";
                    var finalGrades = (from cg in context.CourseGrades
                                       join scg in context.Student_CourseGrades on cg.Id equals scg.CourseGradesId
                                       where scg.StudentId == studentId
                                       select new
                                       {
                                           courseId = cg.CourseId,
                                           finalGrade = cg.FinalGrade,
                                           //studentId = scg.StudentId
                                       });
                    int numRows = finalGrades.ToList().Count;

                    // Check if table is empty
                    if (numRows == 0)
                    {
                        return 0;
                    }

                    // Otherwise display all rows
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    foreach (var grade in finalGrades)
                    {
                        // Lookup and show Course information from CourseId
                        Course currentCourse = Utilities.SearchCourseById(null, grade.courseId);
                        Console.WriteLine($"{currentCourse.CourseName} --> {grade.finalGrade}");
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
        ///     Adds the Course specified by the param courseId to the Student_Course table. Courses that a Student is already registered
        ///     for or already completed will not be added.
        /// </summary>
        /// <param name="studentId">The ID of the Student that will register the Course</param>
        /// <param name="courseId">The ID of the Course to register for</param>
        /// <returns>
        ///     Returns 1 if all checks are passed and Course is successfully registered.
        ///     Returns -1 if a duplicate course is found in a Student's registered courses (Student_Course table).
        ///     Returns -2 if the Student has already completed the Course (checked from Student_CourseGrades table).
        ///     Returns -3 if a database operation goes wrong at any point.
        /// </returns>
        public static int RegisterCourse(int studentId, int courseId)
        {
            // Consider improvement by allowing student to take course again if student made below a 'C',
            // would need an UPDATE in CourseGrades
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    // Check if student is already registered for this course
                    string registeredQuery = $"SELECT Id, StudentId, CourseId FROM Student_Course WHERE StudentId = {studentId}";
                    //Student_Course registeredCourse = context.Student_Course.SqlQuery(registeredQuery).SingleOrDefault();
                    List<Student_Course> registeredCourses = context.Student_Course.SqlQuery(registeredQuery).Where(sc => sc.CourseId == courseId).ToList();
                    //if (registeredCourse.CourseId == courseId) { return -1; }
                    //if (registeredCourses == null) { return -1; }
                    if (registeredCourses.Count >= 1) { return -1; }

                    // Check if student has already taken this course
                    //string query = "SELECT CourseGrades.CourseId, CourseGrades.FinalGrade, Student_CourseGrades.StudentId FROM CourseGrades " +
                    //              $"INNER JOIN Student_CourseGrades ON CourseGrades.Id = Student_CourseGrades.CourseGradesId WHERE Student_CourseGrades.StudentId = {this.Id}";
                    var checkCompleted = (from cg in context.CourseGrades
                                          join scg in context.Student_CourseGrades on cg.Id equals scg.CourseGradesId
                                          where scg.StudentId == studentId
                                          select new
                                          {
                                              courseId = cg.CourseId,
                                              //finalGrade = cg.FinalGrade,
                                              //studentId = scg.StudentId
                                          });
                    foreach (var x in checkCompleted)
                    {
                        if(x.courseId == courseId)
                        {
                            return -2;
                        }
                    }

                    // Otherwise, good to register for course by inserting new row
                    context.Student_Course.Add(new Student_Course(Utilities.RegisterCourseIdNumber, studentId, courseId));
                    context.SaveChanges();
                    
                    return 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -3;
                }
            }
        }
        
        /// <summary>
        ///     Removes the Course specified by the courseId param from the Student_Course table.
        /// </summary>
        /// <param name="studentId">ID of the Student that will be deregistering a Course</param>
        /// <param name="courseId">ID of the Course the Student will deregister</param>
        /// <returns>
        ///     Returns 1 upon successfully deleting row.
        ///     Returns 0 if the table is empty (Student not registered for any courses).
        ///     Returns -1 if course is not found in the DataSet (Student not registered for specified course).
        ///     Returns -2 if a database operation went wrong.
        /// </returns>
        public static int DeregisterCourse(int studentId, int courseId)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    string query = $"SELECT * FROM Student_Course WHERE StudentID = {studentId}";
                    List<Student_Course> registeredCourses = context.Student_Course.SqlQuery(query).ToList();
                    int numRows = registeredCourses.Count;
                    // nothing to delete if table is empty
                    if (numRows == 0)
                    {
                        return 0;
                    }
                    
                    foreach (Student_Course sc in registeredCourses)
                    {
                        // if the traversing CourseId == CourseId of the course to deregister
                        if (sc.CourseId == courseId)
                        {
                            context.Student_Course.Attach(sc);
                            context.Entry(sc).State = System.Data.Entity.EntityState.Deleted;
                            context.SaveChanges();
                            return 1;
                        }
                    }
                    // if loop finishes, course not found in registered courses and no delete done
                    return -1;
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    return -2;
                }
            }
        }

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
                            Course course = SearchCourseById(null, courseId);
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
        ///     Searches through the table of a certain user type specified by the userType parameter and finds a user with a
        ///     matching ID as the id parameter.
        /// </summary>
        /// <param name="id">ID of the User to search for</param>
        /// <param name="userType">Specifies a certain user type to know which table to search in and return</param>
        /// <returns>Returns a User that can be casted to a more specific User type, otherwise returns null if not found in table</returns>
        public static User SearchUserById(int id, int userType)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    if (userType == 1)
                    {
                        return context.Administrators.Find(id);
                    }
                    else if (userType == 2)
                    {
                        return context.Instructors.Find(id);
                    }
                    else if (userType == 3)
                    {
                        return context.Students.Find(id);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }

        /// <summary>
        ///     Searches through the Course table to find a Course with an ID matching the param id. If parameter 'query' is null,
        ///     searches through the entire Course table. Otherwise, uses the query to subset the Course table to search through.
        /// </summary>
        /// <param name="query">An optional param that if specified, this will be the query to use for the SqlDataAdapter</param>
        /// <param name="id">ID of the course to search for in the Course table</param>
        /// <returns>Returns a Course object with matching fields from the Course table, otherwise return null if not found</returns>
        public static Course SearchCourseById(string query, int id)
        {
            if (query == null) { query = "SELECT * FROM Course"; }
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    // TODO - search custom query
                    return context.Courses.Find(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
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
        public static int ViewRegisteredStudents(int studentId)
        {
            // SELECT only the students registered for the specified course
            string query = $"SELECT * FROM Student_Course WHERE CourseID = {studentId}";

            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    List<Student> registeredStudents = context.Students.SqlQuery(query).ToList();
                    int numRows = registeredStudents.Count;
                    // check if empty
                    if (numRows == 0)
                    {
                        return 0;
                    }

                    Console.WriteLine("-----------------------------------------------------------------------------");
                    foreach (Student s in registeredStudents)
                    {
                        Console.WriteLine(s);
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
        ///     Accesses the database to check inputted credentials with the ones stored in the database. Which table is
        ///     accessed is based on the userType parameter. User name matching is NOT case-sensitive, but password matching
        ///     IS case-sensitive, as per standard.
        /// </summary>
        /// <param name="context">DB Context from Login method, same context from Main</param>
        /// <param name="userName">User name to find a match for in DB</param>
        /// <param name="password">Password to find a match for in DB</param>
        /// <param name="userType">Int to specify which DB table to search through</param>
        /// <returns>Returns Id of the user if match is found, otherwise returns -1 if no match</returns>
        static int CheckCredentials(IzendaCMSContext context, string userName, string password, int userType)
        {
            List<Administrator> admins;
            List<Instructor> instructors;
            List<Student> students;

            // figure out which database table to look through and check match in username and password
            if (userType == 1) // Admin
            {
                admins = context.Administrators.ToList();

                foreach (Administrator a in admins)
                {
                    if (userName.Equals(a.UserName, StringComparison.OrdinalIgnoreCase) &&
                    password.Equals(a.Password))
                    {
                        return a.Id;
                    }
                }
            }
            else if (userType == 2) // Instructor
            {
                instructors = context.Instructors.ToList();

                foreach (Instructor i in instructors)
                {
                    if (userName.Equals(i.UserName, StringComparison.OrdinalIgnoreCase) &&
                    password.Equals(i.Password))
                    {
                        return i.Id;
                    }
                }
            }
            else if (userType == 3) // Student
            {
                students = context.Students.ToList();

                foreach (Student s in students)
                {
                    if (userName.Equals(s.UserName, StringComparison.OrdinalIgnoreCase) &&
                    password.Equals(s.Password))
                    {
                        return s.Id;
                    }
                }
            }
            else
            {
                return -1;
            }

            return -1;
        }

        /// <summary>
        ///     Interacts with user for a number of options. First it will prompt user to choose what type of user he/she
        ///     would like to login as. Then it'll proceed to ask user for a username and password, which will then jump to
        ///     the CheckCredentials method to check in with the data for a match in the corresponding table (Administrator,
        ///     Instructor, or Student tables, based on previous option).
        /// </summary>
        /// <param name="context">DB Context from Main</param>
        /// <returns>
        ///     Returns -1 upon user entering the quit (4) option.
        ///     Otherwise, return the option that was selected in the beginning that specified user type.
        /// </returns>
        public static int Login(IzendaCMSContext context)
        {
            int option, loginId;

            // infinite loop to retry in case of invalid input and handle log in
            while (true)
            {
                Console.WriteLine("What type of user would you like to log in as?");
                Console.WriteLine("[Enter '1' for Administrator, '2' for Instructor, '3' for Student, '4' to quit]:");
                if (Int32.TryParse(Console.ReadLine(), out option))
                {
                    if (option == 4) // quit
                    {
                        return -1;
                    }
                    else if (option <= 0 || option > 4)
                    {
                        Console.WriteLine("Please enter a number in the options above");
                        continue;
                    }
                    // else continue to login code
                }
                else
                {
                    Console.WriteLine("Invalid input, please try again");
                    continue;
                }

                // login
                while (true)
                {
                    Console.Write("User Name: ");
                    string userName = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    loginId = CheckCredentials(context, userName, password, option);
                    if (loginId != -1) // set up current user for navigation if login success
                    {
                        if (option == 1)
                        {
                            CurrentAdmin = (Administrator)SearchUserById(loginId, 1);
                        }
                        else if (option == 2)
                        {
                            CurrentInstructor = (Instructor)SearchUserById(loginId, 2);
                        }
                        else if (option == 3)
                        {
                            CurrentStudent = (Student)SearchUserById(loginId, 3);
                        }
                        return option;
                    }
                    else
                    {
                        Console.Write("Invalid user name or password, would you like to try again? [Y/N]: ");
                        string res = Console.ReadLine();
                        if (res.Equals("Y", StringComparison.OrdinalIgnoreCase)) { continue; }
                        else if (res.Equals("N", StringComparison.OrdinalIgnoreCase)) { System.Environment.Exit(0); }
                        else
                        {
                            Console.Write("Invalid response, returning to login menu...");
                            break;
                        }
                    }
                }
            }
        }
    }
}
