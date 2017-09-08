using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCourseManagementSystem
{
    public class Student : User
    {
        // Extra declarations from a User
        public float GPA { get; set; }
        public int CreditHours { get; set; }
        public string Level { get; set; }
        public List<Course> RegisteredCourses { get; set; }
        public List<CourseGrades> FinalGrades { get; set; }

        public Student(int id, string firstName, string lastName, string userName, string password, float gpa, int creditHours)
            : base(id, firstName, lastName, userName, password)
        {
            GPA = gpa;
            CreditHours = creditHours;
            Level = CalculateLevel(creditHours);
            RegisteredCourses = new List<Course>();
            FinalGrades = new List<CourseGrades>();
        }
        
        /// <summary>
        ///     Takes in the number of CreditHours for a Student and returns a string representing
        ///     the Student's grade Level. Returns null if hours is negative.
        /// </summary>
        public string CalculateLevel(int hours)
        {
            if (hours >= 0 && hours < 30)
            {
                return "Freshman";
            }
            else if (hours >= 30 && hours < 60)
            {
                return "Sophomore";
            }
            else if (hours >= 60 && hours < 90)
            {
                return "Junior";
            }
            else if (hours >= 90)
            {
                return "Senior";
            }
            else
            {
                return null;
            }
        }

        /**
         * Displays all CourseGrades the student has earned.
         * If the list is empty, returns false. Otherwise, successfully displays and returns true.
         * BEWARE, NO PARAMS PASSED IN. Maybe think about restructuring later.
         */
        public bool ViewFinalGrades()
        {
            if (!FinalGrades.Any())
            {
                return false;
            }

            Console.WriteLine("-----------------------------------------------------------------------------");
            foreach (CourseGrades cg in FinalGrades)
            {
                Console.WriteLine(cg);
            }
            Console.WriteLine("-----------------------------------------------------------------------------");
            return true;
        }
        
        /// <summary>
        ///     Adds Course courseToRegister param to the Student's RegisteredCourses List. Courses that a Student
        ///     is already registered for or already completed will not be added.
        ///     Returns 1 upon success, -1 if duplicate course in RegisteredCourses, -2 if course already completed.
        /// </summary>
        public int RegisterCourse(Course courseToRegister)
        {
            // Check if already registered for param course
            for (int i = 0; i < RegisteredCourses.Count; i++)
            {
                if (courseToRegister.Id == RegisteredCourses[i].Id)
                {
                    return -1;
                }
            }

            // Check if already taken the param course
            // TODO - improve by allowing student to take course again if student made below a 'C'
            for (int i = 0; i < FinalGrades.Count; i++)
            {
                if (courseToRegister.Id == FinalGrades[i].CourseId)
                {
                    return -2;
                }
            }

            // Otherwise, good to register for course
            RegisteredCourses.Add(courseToRegister);
            return 1;
        }
        
        /// <summary>
        ///     Removes the Course specified in the params to the RegisteredCourses list. Also updates RegisteredStudents list
        ///     in Course. Check for valid course and index done before this method call.
        /// </summary>
        public bool DeregisterCourse(int index)
        {
            // first deregister student from RegisteredStudents list in Course
            Course courseToDeregister = this.RegisteredCourses[index];
            int studentIndex = courseToDeregister.SearchStudentById(this.Id);
            courseToDeregister.RegisteredStudents.RemoveAt(studentIndex);

            // now remove course from RegisteredCourses list in this Student
            RegisteredCourses.RemoveAt(index);
            return true;
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
        /// <param name="courses">List of all existing courses</param>
        /// <param name="action">Number to represent what course of action for a Student to take</param>
        /// <returns>True/False of whether the action has been completed or not</returns>
        public bool StudentActionHandler(List<Course> courses, int action)
        {
            if (action == 1) // view courses
            {
                bool status = this.ViewCourses(courses);
                if (!status)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("There are currently no existing courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }
                return status;
            }
            else if (action == 2) // view registered
            {
                bool status = this.ViewCourses(this.RegisteredCourses);
                if (!status)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("You are not currently registered for any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }
                return status;
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
                // TODO - show grades in more detail, currently only shows course ID with final grade.
                //        Preferrably have it show course name and ID.

                bool status = this.ViewFinalGrades();
                if (!status)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("You have not completed any courses.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }
                return status;
            }
            else if (action == 5) // register course
            {
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to register for: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    int index = Course.SearchCourseById(courses, id);
                    if (index >= 0)
                    {
                        Console.WriteLine($"Course Successfully Found, adding this course to your registered courses:\n{courses[index]}");
                        int status = this.RegisterCourse(courses[index]);
                        if (status == 1)
                        {
                            Console.WriteLine("Course successfully registered.");
                            courses[index].RegisteredStudents.Add(this); // also add student to RegisteredStudents list in Course
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
                if (this.RegisteredCourses.Count == 0)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("You are not currently registered for any courses");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    return false;
                }
                Console.WriteLine("-----------------------------------------------------------------------------");
                Console.Write("Enter the ID of the course you would like to deregister: ");
                int id;
                // make sure the inputted id is an int
                if (Int32.TryParse(Console.ReadLine(), out id))
                {
                    int index = Course.SearchCourseById(this.RegisteredCourses, id); // index for RegisteredCourses list in Student
                    if (index >= 0)
                    {
                        Console.WriteLine($"Course Successfully Found, removing this course from your registered courses:\n{courses[index]}");
                        bool status = this.DeregisterCourse(index);
                        if (status)
                        {
                            Console.WriteLine("Course successfully deregistered.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to deregister course.");
                        }
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        return status;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to find course of ID {id} in your registered courses");
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
