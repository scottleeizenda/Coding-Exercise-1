using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IzendaCMS.DataModel.Models.Mapping
{
    public class Student_CourseGradesMap : EntityTypeConfiguration<Student_CourseGrades>
    {
        public Student_CourseGradesMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("Student_CourseGrades");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.StudentId).HasColumnName("StudentId");
            this.Property(t => t.CourseGradesId).HasColumnName("CourseGradesId");

            // Relationships
            this.HasRequired(t => t.CourseGrade)
                .WithMany(t => t.Student_CourseGrades)
                .HasForeignKey(d => d.CourseGradesId);
            this.HasRequired(t => t.Student)
                .WithMany(t => t.Student_CourseGrades)
                .HasForeignKey(d => d.StudentId);

        }
    }
}
