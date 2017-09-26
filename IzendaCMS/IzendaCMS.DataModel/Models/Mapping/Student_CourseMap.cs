using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IzendaCMS.DataModel.Models.Mapping
{
    public class Student_CourseMap : EntityTypeConfiguration<Student_Course>
    {
        public Student_CourseMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("Student_Course");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.StudentId).HasColumnName("StudentId");
            this.Property(t => t.CourseId).HasColumnName("CourseId");

            // Relationships
            this.HasRequired(t => t.Course)
                .WithMany(t => t.Student_Course)
                .HasForeignKey(d => d.CourseId);
            this.HasRequired(t => t.Student)
                .WithMany(t => t.Student_Course)
                .HasForeignKey(d => d.StudentId);

        }
    }
}
