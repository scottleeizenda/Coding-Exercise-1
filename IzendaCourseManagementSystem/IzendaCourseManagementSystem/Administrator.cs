using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace IzendaCourseManagementSystem
{
    public class Administrator : User
    {
        // Extra declarations from a User
        public DateTime HireDate { get; set; }

        public Administrator(int id, string firstName, string lastName, DateTime hireDate, string userName, string password)
            : base(id, firstName, lastName, userName, password)
        {
            HireDate = hireDate;
            UserType = "Administrator";
        }
        
        /// <summary>
        ///    Checks if all fields in courseFields are valid, then proceeds to create and add a
        ///    new Course in the Course table of the database (INSERT INTO).
        ///    Returns true upon successful INSERT INTO database, otherwise returns false upon any failure.
        ///    Failure may be a bad courseField input or failure to insert into DB.
        /// </summary>
        public bool CreateCourse(SqlConnection connection, string[] courseFields)
        {
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

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Course", connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            DataSet set = new DataSet();
            adapter.Fill(set, "Course");
            DataRow row = set.Tables[0].NewRow();
            row["Id"] = id;
            row["StartDate"] = startDate;
            row["EndDate"] = endDate;
            row["CreditHours"] = hours;
            row["CourseName"] = courseFields[4];
            row["CourseDescription"] = courseFields[5];
            try
            {
                set.Tables[0].Rows.Add(row);
                adapter.Update(set.Tables[0]);
            }
            catch
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        ///     Checks if all fields in courseFields are valid, where a blank ("") element means to keep that field the same as before.
        ///     If no problems with the fields, proceeds to do an UPDATE to the Course table in DB with the new specified fields.
        ///     Returns true if UPDATE is successful, otherwise returns false.
        /// </summary>
        public bool UpdateCourse(SqlConnection connection, int currentId, string[] courseFields)
        {
            // courseFields should be exactly 6 elements, corresponds exactly with Course columns in DB
            if (courseFields.Length != 6)
            {
                return false;
            }

            int id, hours;
            DateTime startDate, endDate;
            Course currentCourse = Course.SearchCourseById(connection, currentId);

            // check all fields' validity first before attempting to update database, return false upon an invalid input field

            // if blank courseField detected, keep field same as the one before. Otherwise, check for validity of non-string fields
            if (courseFields[0] == "")
            {
                id = currentId;
            }
            else if (!Int32.TryParse(courseFields[0], out id))
            {
                return false;
            }
            if (courseFields[1] == "")
            {
                startDate = currentCourse.StartDate;
            }
            else if (!DateTime.TryParse(courseFields[1], out startDate))
            {
                return false;
            }
            if (courseFields[2] == "")
            {
                endDate = currentCourse.EndDate;
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
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Course", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Course");
                set.Tables["Course"].Constraints.Add("Id_PK", set.Tables["Course"].Columns["Id"], true);
                DataRow row = set.Tables["Course"].Rows.Find(currentId);
                row.BeginEdit();
                row["Id"] = id;
                row["StartDate"] = startDate;
                row["EndDate"] = endDate;
                row["CreditHours"] = hours;
                row["CourseName"] = courseFields[4];
                row["CourseDescription"] = courseFields[5];
                row.EndEdit();
                adapter.Update(set.Tables["Course"]);

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        ///     Deletes a Course from the Course table in the database. Returns true upon success, false if anything
        ///     goes wrong and delete fails.
        /// </summary>
        public bool DeleteCourse(SqlConnection connection, int courseId)
        {
            try
            {
                // TODO - decide how to handle deletion of Course with foreign key references to it

                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Course", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Course");
                set.Tables["Course"].Constraints.Add("Id_PK", set.Tables["Course"].Columns["Id"], true);
                set.Tables["Course"].Rows.Find(courseId).Delete();
                adapter.Update(set.Tables["Course"]);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        ///     Displays all available Instructors that an Administrator can assign a Course to, pulled from the Instructor table.
        ///     Returns false if somehow something goes wrong. Otherwise, successfully displays and returns true.
        /// </summary>
        public bool ViewInstructors(SqlConnection connection)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Instructor", connection);
                DataSet set = new DataSet();
                adapter.Fill(set, "Instructor");
                DataTable table = set.Tables[0];

                Console.WriteLine("-----------------------------------------------------------------------------");
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine($"Instructor ID: {row["Id"]}");
                    Console.WriteLine($"Name: {row["LastName"]}, {row["FirstName"]}");
                    Console.WriteLine($"Hire Date: {row["HireDate"]}");
                    Console.WriteLine($"User Name: {row["UserName"]}");
                    Console.WriteLine($"Password: {row["Password"]}\n");
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
        ///     Searches through the Instructor table in the database by ID and returns a new Instructor object
        ///     with matching attributes from the table columns. Returns null if Instructor not found.
        /// </summary>
        public static Administrator SearchAdministratorById(SqlConnection connection, int id)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Administrator", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Administrator");
                set.Tables["Administrator"].Constraints.Add("Id_PK", set.Tables["Administrator"].Columns["Id"], true);

                DataRow row = set.Tables["Administrator"].Rows.Find(id);
                DateTime hireDate = DateTime.Parse(row["HireDate"].ToString());

                return new Administrator(id, row["FirstName"].ToString(), row["LastName"].ToString(), hireDate, row["UserName"].ToString(), row["Password"].ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Searches through the Instructor table in the database by ID and returns a new Instructor object
        ///     with matching attributes from the table columns. Returns null if Instructor not found.
        /// </summary>
        public static Instructor SearchInstructorById(SqlConnection connection, int id)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Instructor", connection);
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                DataSet set = new DataSet();
                adapter.Fill(set, "Instructor");
                set.Tables["Instructor"].Constraints.Add("Id_PK", set.Tables["Instructor"].Columns["Id"], true);

                DataRow row = set.Tables["Instructor"].Rows.Find(id);
                DateTime hireDate = DateTime.Parse(row["HireDate"].ToString());

                return new Instructor(id, row["FirstName"].ToString(), row["LastName"].ToString(), hireDate, row["UserName"].ToString(), row["Password"].ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Assigns an Instructor to a Course by adding an entry to the Instructor_Course associative table.
        ///     The table has a pairing of an Instructor's ID and Course's ID.
        /// </summary>
        public bool AssignInstructor(SqlConnection connection, Course selectedCourse, Instructor selectedInstructor)
        {
            // TODO - Prevent any duplicate entries? May not be necessary if Instructor is allowed to teach multiple
            //        classes of the same Course.

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Instructor_Course", connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            DataSet set = new DataSet();
            adapter.Fill(set, "Instructor_Course");
            DataRow row = set.Tables["Instructor_Course"].NewRow();
            row["Id"] = Program.assignInstructorIdNumber;
            Program.assignInstructorIdNumber++;
            row["InstructorId"] = selectedInstructor.Id;
            row["CourseId"] = selectedCourse.Id;
            try
            {
                set.Tables["Instructor_Course"].Rows.Add(row);
                adapter.Update(set.Tables["Instructor_Course"]);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Handles an Administrator action specified by user input from Main. Database not yet implemented,
        ///     so data comes from Lists created in Main. The check for a valid action number also done in Main.
        ///     Action #1 = Creating a course (Database INSERT)
        ///     Action #2 = View all courses (Database SELECT, may expand later to view specific courses)
        ///     Action #3 = Edit a course (Database UPDATE)
        ///     Action #4 = Delete a course (Database DELETE)
        ///     Action #5 = Assign an Instructor to teach a Course (will also later deal with a database)
        /// </summary>
        /// <param name="courses">List of all existing courses</param>
        /// <param name="instructors">List of all available instructors</param>
        /// <param name="action">Number to represent what course of action for an Administrator to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public bool AdminActionHandler(SqlConnection connection, int action)
        {
            if (action == 1) // create course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                string[] courseFields = new string[6];
                Console.Write("Enter the course ID: ");
                courseFields[0] = Console.ReadLine();
                Console.Write("Enter the course start date: ");
                courseFields[1] = Console.ReadLine();
                Console.Write("Enter the course end date: ");
                courseFields[2] = Console.ReadLine();
                Console.Write("Enter the course credit hours: ");
                courseFields[3] = Console.ReadLine();
                Console.Write("Enter the course name: ");
                courseFields[4] = Console.ReadLine();
                Console.Write("Enter the course description: ");
                courseFields[5] = Console.ReadLine();

                bool status = this.CreateCourse(connection, courseFields);
                if (status)
                {
                    Console.WriteLine("Course successfully created.");
                }
                else
                {
                    Console.WriteLine("Failed to create course.");
                }
                Console.WriteLine("-----------------------------------------------------------------------------");
                return status;
            }
            else if (action == 2) // view courses
            {
                bool status = this.ViewCourses(connection);
                if (!status)
                {
                    Console.WriteLine("There are currently no existing courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }
                return status;
            }
            else if (action == 3) // edit course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to edit: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    Course currentCourse = Course.SearchCourseById(connection, id);
                    if (currentCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found:\n{currentCourse}");
                        Console.WriteLine("[Leave a field blank if you wish to keep it the same as before]");
                        string[] courseFields = new string[6];
                        Console.Write("Enter the new course ID: ");
                        courseFields[0] = Console.ReadLine();
                        Console.Write("Enter the new course start date: ");
                        courseFields[1] = Console.ReadLine();
                        Console.Write("Enter the new course end date: ");
                        courseFields[2] = Console.ReadLine();
                        Console.Write("Enter the new course credit hours: ");
                        courseFields[3] = Console.ReadLine();
                        Console.Write("Enter the new course name: ");
                        courseFields[4] = Console.ReadLine();
                        Console.Write("Enter the new course description: ");
                        courseFields[5] = Console.ReadLine();
                        
                        bool status = this.UpdateCourse(connection, id, courseFields);
                        if (status)
                        {
                            Console.WriteLine("Course successfully updated.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to update course.");
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return status;
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
            else if (action == 4) // delete course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to delete: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    Course currentCourse = Course.SearchCourseById(connection, id);
                    if (currentCourse != null)
                    {
                        Console.WriteLine($"Course Successfully Found:\n{currentCourse}");
                        Console.WriteLine("Are you sure you would like to delete this course?");
                        Console.Write("[Enter Y/N]: ");
                        string choice = Console.ReadLine();
                        if (choice.Equals("Y", StringComparison.OrdinalIgnoreCase) || choice.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                        {
                            bool status = this.DeleteCourse(connection, id);
                            if (status)
                            {
                                Console.WriteLine($"Successfully deleted course with ID {id}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to delete course with ID {id}.");
                                Console.WriteLine("Some reasons may be:");
                                Console.WriteLine("1) An Instructor is still assigned to teach this course.");
                                Console.WriteLine("2) Student(s) have a CourseGrade for this course.");
                                Console.WriteLine("3) Student(s) are currently registered for this course.");
                            }
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            return status;
                        }
                        else if (choice.Equals("N", StringComparison.OrdinalIgnoreCase) || choice.Equals("No", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Cancelling deletion of course with ID {id}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice, returning to menu...");
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false; // return false upon answering "No" or invalid input
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
            else // assign instructor
            {
                // TODO - consider adding check for empty tables to end this action earlier

                Instructor selectedInstructor;
                Course selectedCourse;
                // loop for user input on selecting an instructor
                while (true)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Type the ID of the instructor you would like to assign a course to");
                    Console.WriteLine("[Enter 'list' to see all the available instructors]");
                    Console.Write("[Enter 'quit' to cancel]: ");
                    string input = Console.ReadLine();
                    int id;
                    if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ViewInstructors(connection);
                    }
                    else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Cancelled, returning to menu...");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return false;
                    }
                    else if (Int32.TryParse(input, out id))
                    {
                        selectedInstructor = SearchInstructorById(connection, id);
                        if (selectedInstructor != null)
                        {
                            Console.WriteLine($"Instructor {selectedInstructor.FirstName} {selectedInstructor.LastName} successfully found.");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            // loop for user input on selecting a course to assign
                            while (true)
                            {
                                Console.WriteLine("Type the ID of the course you would like to assign to this instructor");
                                Console.WriteLine("[Enter 'list' to see all the available courses]");
                                Console.Write("[Enter 'quit' to cancel]: ");
                                input = Console.ReadLine();
                                if (input.Equals("list", StringComparison.OrdinalIgnoreCase))
                                {
                                    this.ViewCourses(connection);
                                }
                                else if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Cancelled, returning to instructor select...");
                                    break;
                                }
                                else if (Int32.TryParse(input, out id))
                                {
                                    selectedCourse = Course.SearchCourseById(connection, id);
                                    if (selectedCourse != null)
                                    {
                                        Console.WriteLine($"Course {selectedCourse.CourseName} successfully found.");
                                        bool status = this.AssignInstructor(connection, selectedCourse, selectedInstructor);
                                        if (status)
                                        {
                                            Console.WriteLine($"Instructor {selectedInstructor.FirstName} {selectedInstructor.LastName} has been assigned to teach course {selectedCourse.CourseName}");
                                            Program.assignInstructorIdNumber++;
                                            Console.WriteLine("-----------------------------------------------------------------------------");
                                            return true;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Failed to assign {selectedInstructor.FirstName} {selectedInstructor.LastName} to teach the course {selectedCourse.CourseName}");
                                            Console.WriteLine("-----------------------------------------------------------------------------");
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to find course with ID {id}");
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, please try again.");
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to find instructor with ID {id}");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }
        }
    }
}
