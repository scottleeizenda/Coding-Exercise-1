using IzendaCMS.DataModel;
using IzendaCMS.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IzendaCMS.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            /*using (var ctx = new IzendaCMSContext())
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

                string assignedCoursesQuery = "SELECT Course.Id, Course.StartDate, Course.EndDate, Course.CreditHours, Course.CourseName, Course.CourseDescription FROM Course " +
                                 $"INNER JOIN Instructor_Course ON Course.Id = Instructor_Course.CourseId WHERE Instructor_Course.InstructorId = 102";

                List<Course> courses = ctx.Courses.SqlQuery(assignedCoursesQuery).ToList();
                Course co = courses.Find(c => c.Id == 10001302);
                if (co == null) { Console.WriteLine("NULLLL"); }

                //ctx.Database.Log = Console.WriteLine;
                Student s = ctx.Students.Find(5002);
                ctx.Entry(s).Collection(reg => reg.Student_Course).Load();
                ctx.Entry(s).Collection(grades => grades.Student_CourseGrades).Load();
                Console.WriteLine(Utilities.CalculateGPA(s.Id));
                //Console.WriteLine($"Courses: {s.Student_Course.Count}, Grades: {s.Student_CourseGrades.Count}");
                foreach (Student_CourseGrades scg in s.Student_CourseGrades)
                {
                    CourseGrade cg = ctx.CourseGrades.Find(scg.CourseGradesId);
                    Course c = Utilities.SearchCourseById(null, cg.CourseId);
                    Console.WriteLine($"{c.CourseName} --> {cg.FinalGrade}");
                }
                
                Course course = ctx.Courses.Find(10001302);
                foreach (CourseGrade cg in course.CourseGrades)
                {
                    Console.WriteLine(cg.Id);
                }
            }*/

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
