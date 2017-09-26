using IzendaCMS.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace IzendaCMS.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Course course = new Course
            {
                Id = 1000,
                StartDate = DateTime.Parse("01/01/2000"),
                EndDate = DateTime.Parse("01/01/2001"),
                CreditHours = 6,
                CourseName = "Test",
                CourseDescription = "TestDesc"
            };

            using (var ctx = new IzendaCMSContext())
            {
                List<Course> courses = ctx.Courses.ToList();
                List<Course> courses2 = ctx.Courses.Where(c => c.CreditHours == 3).ToList();
                
                courses = ctx.Courses.ToList();
                //var query = context.Course;
                foreach (Course c in courses)
                {
                    Console.WriteLine($"{c.Id}, {c.CourseName}, {c.CourseDescription}");
                }
                foreach (Course c in courses2)
                {
                    Console.WriteLine($"{c.CreditHours}, {c.CourseName}, {c.CourseDescription}");
                }
            }

            using (var ctx = new IzendaCMSContext())
            {
                var checkCompletionQuery = (from cg in ctx.CourseGrades
                                            join scg in ctx.Student_CourseGrades on cg.Id equals scg.CourseGradesId
                                            where scg.StudentId == 5001
                                            select new
                                            {
                                                courseId = cg.CourseId,
                                                finalGrade = cg.FinalGrade,
                                                studentId = scg.StudentId
                                            });

                foreach (var x in checkCompletionQuery)
                {
                    Console.WriteLine($"{x.courseId} | {x.finalGrade} | {x.studentId}");
                }
            }

            // Set up ID number generation
            using (IzendaCMSContext ctx = new IzendaCMSContext())
            {
                string courseGradesQuery = "SELECT MAX(Id) AS MostRecentId FROM CourseGrades";
                string instructorCourseQuery = "SELECT MAX(Id) AS MostRecentId FROM Instructor_Course";
                string studentCourseQuery = "SELECT MAX(Id) AS MostRecentId FROM Student_Course";

                try
                {
                    Utilities.CourseGradeIdNumber = ctx.Database.SqlQuery<int>(courseGradesQuery).FirstOrDefault() + 1;
                    Utilities.AssignInstructorIdNumber = ctx.Database.SqlQuery<int>(instructorCourseQuery).FirstOrDefault() + 1;
                    Utilities.RegisterCourseIdNumber = ctx.Database.SqlQuery<int>(studentCourseQuery).FirstOrDefault() + 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                    System.Environment.Exit(1);
                }
            }

            /****** Start of the text-based user interactions ******/
            Console.WriteLine("=============================================================");
            Console.WriteLine("||     Welcome to the Izenda Course Management System!     ||");
            Console.WriteLine("=============================================================");

            IzendaCMSContext context = new IzendaCMSContext();
            int loginStatus;
            // loop to allow log in as a different user upon logging out
            while (true)
            {
                loginStatus = Utilities.Login(context);
                if (loginStatus == -1)
                {
                    Console.WriteLine("Login cancelled, exiting system...");
                    break;
                }
                else
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    // loop to process a user's selected actions
                    while (true)
                    {
                        if (ActionHandler.UserAction(loginStatus))
                        {
                            // Quit action selected, go back to login prompt
                            break;
                        }
                    }
                }
            }
            context.Dispose();
        }
    }
}
