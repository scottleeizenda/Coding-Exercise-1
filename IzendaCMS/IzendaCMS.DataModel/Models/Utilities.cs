using System;
using System.Collections.Generic;
using System.Linq;

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
            if (courseFields.Length != 6) { return false; }
            
            int id, hours;
            DateTime startDate, endDate;

            // return false upon an invalid input field
            if (!Int32.TryParse(courseFields[0], out id)) { return false; }
            if (!DateTime.TryParse(courseFields[1], out startDate)) { return false; }
            if (!DateTime.TryParse(courseFields[2], out endDate)) { return false; }
            if (!Int32.TryParse(courseFields[3], out hours)) { return false; }
            // also make sure "CourseName" and "CourseDescription" fields aren't too big for DB
            if (courseFields[4].Length > 50) { return false; }
            if (courseFields[5].Length > 100) { return false; }

            // if all fields good, proceed to INSERT INTO Course table
            Course course = new Course(id, startDate, endDate, hours, courseFields[4], courseFields[5]);
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
            if (courseFields.Length != 6) { return false; }
            
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
            // also check if new string length will fit into database, if applicable
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
        ///     Assigns an Instructor to a Course by adding an entry to the Instructor_Course associative table.
        ///     The table has a pairing of an Instructor's ID and Course's ID.
        /// </summary>
        /// <param name="selectedCourse">Course that an Instructor will be assigned to teach</param>
        /// <param name="selectedInstructor">Instructor that will be assign to teach a Course</param>
        /// <returns>Returns true upon a successful INSERT, otherwise returns false from database operation gone wrong</returns>
        public static bool AssignInstructor(Course selectedCourse, Instructor selectedInstructor)
        {
            // construct new Instructor_Course object for new row into respective table
            Instructor_Course assignedCourse = new Instructor_Course(AssignInstructorIdNumber, selectedInstructor.Id, selectedCourse.Id);

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
        ///     Unassigns an Instructor by removing the corresponding row from the Instructor_Course table specified
        ///     through the given parameters. If more than one instance of the same Course was assigned to the specified
        ///     Instructor, all instances of the Course will be removed from the database.
        /// </summary>
        /// <param name="courseId">ID of the course to unassign from instructor</param>
        /// <param name="instructorId">ID of the instructor to unassign course from</param>
        /// <returns>Returns true upon successfully unassigning a course, returns false otherwise</returns>
        public static bool UnassignInstructor(int courseId, int instructorId)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    // assignedCourses should be narrowed down to only 1 entry if only one instance of course was assigned to instructor
                    List<Instructor_Course> assignedCourses = context.Instructor_Course.Where(ic => ic.CourseId == courseId && ic.InstructorId == instructorId).ToList();
                    if (assignedCourses.Count == 0) { return false; }
                    foreach (Instructor_Course ic in assignedCourses)
                    {
                        context.Entry(ic).State = System.Data.Entity.EntityState.Deleted;
                    }
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
        ///     finally the course is deregistered for the Student by calling the Student's deregister method. If one of the tasks were
        ///     to go wrong, EF should rollback the transaction.
        ///     TODO - consider changing return type to int to create more meaningful user response
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
                        
                        // UPDATE Student's CreditHours and Level if final grade is at least a 'C'
                        if (letterGrade.ToUpper() == "A" || letterGrade.ToUpper() == "B" || letterGrade.ToUpper() == "C")
                        {
                            selectedStudent.CreditHours += creditHours;
                            selectedStudent.Level = CalculateLevel(selectedStudent.CreditHours);
                            context.Students.Attach(selectedStudent);
                            context.Entry(selectedStudent).State = System.Data.Entity.EntityState.Modified;
                        }

                        // deregister student regardless of what grade received
                        Console.WriteLine("Student info updated, now attempting to deregister student from this course...");
                        int status = Utilities.DeregisterCourse(selectedStudent.Id, courseId);
                        if (status == 1)
                        {
                            context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return false;
                    }
                }

                // separate context to use the saved changes from previous context, uses newly updated tables
                // to calculate GPA (i.e includes the newly added row in Student_CourseGrades)
                using (IzendaCMSContext context = new IzendaCMSContext())
                {
                    try
                    {
                        // Update GPA
                        selectedStudent.GPA = CalculateGPA(selectedStudent.Id);
                        context.Students.Attach(selectedStudent);
                        context.Entry(selectedStudent).State = System.Data.Entity.EntityState.Modified;
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
            return false; // invalid letterGrade
        }

        /// <summary>
        ///     Takes in the number of CreditHours for a Student and returns a string representing the Student's grade Level.
        /// </summary>
        /// <param name="hours">The total credit hours to convert to a certain grade level</param>
        /// <returns>A string representing the Student's grade level</returns>
        public static string CalculateLevel(int hours)
        {
            if (hours >= 0 && hours < 30) { return "Freshman"; }
            else if (hours >= 30 && hours < 60) { return "Sophomore"; }
            else if (hours >= 60 && hours < 90) { return "Junior"; }
            else if (hours >= 90) { return "Senior"; }
            else { return "Undefined"; }
        }

        /// <summary>
        ///     Calculates a student's GPA by retrieving the necessary data from a LINQ query and traversing through
        ///     the acquired list to calculate an average.
        /// </summary>
        /// <param name="studentId">ID of the student to calculate GPA for</param>
        /// <returns>The specified student's calculated GPA</returns>
        public static double CalculateGPA(int studentId)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                var finalGrades = (from cg in context.CourseGrades
                                   join scg in context.Student_CourseGrades on cg.Id equals scg.CourseGradesId
                                   where scg.StudentId == studentId
                                   select new
                                   {
                                       finalGrade = cg.FinalGrade,
                                   });
                double sum = 0;
                int count = 0;
                foreach (var grade in finalGrades)
                {
                    if (grade.finalGrade == "A") { sum += 4; }
                    else if (grade.finalGrade == "B") { sum += 3; }
                    else if (grade.finalGrade == "C") { sum += 2; }
                    else if (grade.finalGrade == "D") { sum += 1; }
                    count++;
                }
                //Console.WriteLine($"{sum}, {count}, {sum / count}");
                return sum / count;
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
                    Student selectedStudent = context.Students.Find(studentId);
                    context.Entry(selectedStudent).Collection(s => s.Student_Course).Load();
                    foreach (Student_Course sc in selectedStudent.Student_Course)
                    {
                        if (sc.CourseId == courseId) { return -1; }
                    }

                    // Check if student has already taken this course
                    /*var checkCompleted = (from cg in context.CourseGrades
                                          join scg in context.Student_CourseGrades on cg.Id equals scg.CourseGradesId
                                          where scg.StudentId == studentId
                                          select new
                                          {
                                              courseId = cg.CourseId
                                          });
                    foreach (var x in checkCompleted)
                    {
                        if(x.courseId == courseId)
                        {
                            return -2;
                        }
                    }*/
                    context.Entry(selectedStudent).Collection(s => s.Student_CourseGrades).Load();
                    foreach (Student_CourseGrades scg in selectedStudent.Student_CourseGrades)
                    {
                        // course ID not in Student_CourseGrades table, so look it up through CourseGrade
                        CourseGrade cg = context.CourseGrades.Find(scg.CourseGradesId);
                        if (cg.CourseId == courseId) { return -2; }
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
        ///     Searches through the table of a certain user type specified by the userType parameter and finds a user with a
        ///     matching ID as the id parameter. If a query is defined, searches for user in the newly specified table.
        /// </summary>
        /// <param name="query">Query to specify a more refined table, or can be null to search entire table by default</param>
        /// <param name="id">ID of the User to search for</param>
        /// <param name="userType">Specifies a certain user type to know which table to search in and return</param>
        /// <returns>Returns a User that can be casted to a more specific User type, otherwise returns null if not found in table</returns>
        public static User SearchUserById(string query, int id, int userType)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    if (userType == 1)
                    {
                        if (query == null) { return context.Administrators.Find(id); }
                        else
                        {
                            List<Administrator> newAdminList = context.Administrators.SqlQuery(query).ToList();
                            return newAdminList.Find(a => a.Id == id);
                        }
                    }
                    else if (userType == 2)
                    {
                        if (query == null) { return context.Instructors.Find(id); }
                        else
                        {
                            List<Instructor> newInstructorList = context.Instructors.SqlQuery(query).ToList();
                            return newInstructorList.Find(i => i.Id == id);
                        }
                    }
                    else if (userType == 3)
                    {
                        if (query == null) { return context.Students.Find(id); }
                        else
                        {
                            List<Student> newStudentList = context.Students.SqlQuery(query).ToList();
                            return newStudentList.Find(s => s.Id == id);
                        }
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
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                try
                {
                    if (query == null)
                    {
                        return context.Courses.Find(id);
                    }
                    else
                    {
                        List<Course> newCourseList = context.Courses.SqlQuery(query).ToList();
                        return newCourseList.Find(c => c.Id == id);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }

        /// <summary>
        ///     Searches through the table of a certain user type specified by the userType parameter and finds a user with a
        ///     matching ID as the id parameter. If a query is defined, searches for user in the newly specified table.
        /// </summary>
        /// <param name="query">Query to specify a more refined table, or can be null to search entire table by default</param>
        /// <param name="id">ID of the User to search for</param>
        /// <param name="userType">Specifies a certain user type to know which table to search in and return</param>
        /// <returns>Returns a User that can be casted to a more specific User type, otherwise returns null if not found in table</returns>
        public static Student SearchRegisteredStudentById(int courseId, int studentId)
        {
            using (IzendaCMSContext context = new IzendaCMSContext())
            {
                string query = $"SELECT * FROM Student_Course WHERE CourseID = {courseId}";

                try
                {
                    List<Student_Course> registeredStudents = context.Student_Course.SqlQuery(query).ToList();
                    Student_Course sc = registeredStudents.Find(s => s.StudentId == studentId);
                    if (sc != null) { return (Student)SearchUserById(sc.StudentId, 3); }

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
