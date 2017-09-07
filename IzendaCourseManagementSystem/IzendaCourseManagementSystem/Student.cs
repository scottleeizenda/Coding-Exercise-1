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

        /**
         * Returns a grade level based on amount of credit hours,
         * and if hours is negative, null is returned.
         */
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

        /**
         * Adds the Course specified in the params to the RegisteredCourses list.
         * Returns 1 upon success, -1 if duplicate course in RegisteredCourses, -2 if course already completed.
         */
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

        /**
         * Removes the Course specified in the params to the RegisteredCourses list. Also updates RegisteredStudents list in Course.
         * Check for valid course and index done before this method call.
         */
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

        public override string ToString()
        {
            return $"Student Id: {Id}\nName: {LastName}, {FirstName}\nCredit Hours Earned: {CreditHours}\nGPA: {GPA}\nGrade Level: {Level}\n";
        }
    }
}
