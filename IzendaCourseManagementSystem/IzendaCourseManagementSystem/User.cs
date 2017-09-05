using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzendaCourseManagementSystem
{
    public class User
    {
        // Declarations & Getters/Setters
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        
        public User(int id, string firstName, string lastName, string userName, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Password = password;
            UserType = "Undefined";
        }

        /**
         * Displays all available courses in the courses list.
         * If the list is empty, returns false. Otherwise, successfully displays and returns true.
         */
        public bool ViewCourses(List<Course> courses)
        {
            if (!courses.Any())
            {
                return false;
            }
            
            foreach (Course c in courses)
            {
                Console.WriteLine(c);
            }
            return true;
        }
    }
}
