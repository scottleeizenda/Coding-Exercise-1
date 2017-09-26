using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using IzendaCMS.DataModel.Models.Mapping;

namespace IzendaCMS.DataModel.Models
{
    public partial class IzendaCMSContext : DbContext
    {
        static IzendaCMSContext()
        {
            Database.SetInitializer<IzendaCMSContext>(null);
        }

        public IzendaCMSContext()
            : base("Name=IzendaCourseManagementSystemContext")
        {
        }

        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseGrade> CourseGrades { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Instructor_Course> Instructor_Course { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Student_Course> Student_Course { get; set; }
        public DbSet<Student_CourseGrades> Student_CourseGrades { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AdministratorMap());
            modelBuilder.Configurations.Add(new CourseMap());
            modelBuilder.Configurations.Add(new CourseGradeMap());
            modelBuilder.Configurations.Add(new InstructorMap());
            modelBuilder.Configurations.Add(new Instructor_CourseMap());
            modelBuilder.Configurations.Add(new StudentMap());
            modelBuilder.Configurations.Add(new Student_CourseMap());
            modelBuilder.Configurations.Add(new Student_CourseGradesMap());
        }
    }
}
