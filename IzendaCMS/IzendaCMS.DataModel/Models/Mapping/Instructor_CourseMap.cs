using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IzendaCMS.DataModel.Models.Mapping
{
    public class Instructor_CourseMap : EntityTypeConfiguration<Instructor_Course>
    {
        public Instructor_CourseMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("Instructor_Course");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InstructorId).HasColumnName("InstructorId");
            this.Property(t => t.CourseId).HasColumnName("CourseId");

            // Relationships
            this.HasRequired(t => t.Course)
                .WithMany(t => t.Instructor_Course)
                .HasForeignKey(d => d.CourseId);
            this.HasRequired(t => t.Instructor)
                .WithMany(t => t.Instructor_Course)
                .HasForeignKey(d => d.InstructorId);

        }
    }
}
