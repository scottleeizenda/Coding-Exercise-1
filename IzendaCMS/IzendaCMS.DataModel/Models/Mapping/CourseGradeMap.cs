using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IzendaCMS.DataModel.Models.Mapping
{
    public class CourseGradeMap : EntityTypeConfiguration<CourseGrade>
    {
        public CourseGradeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.FinalGrade)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("CourseGrades");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CourseId).HasColumnName("CourseId");
            this.Property(t => t.FinalGrade).HasColumnName("FinalGrade");

            // Relationships
            this.HasRequired(t => t.Course)
                .WithMany(t => t.CourseGrades)
                .HasForeignKey(d => d.CourseId);

        }
    }
}
