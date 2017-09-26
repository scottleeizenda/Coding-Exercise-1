using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace IzendaCMS.DataModel.Models.Mapping
{
    public class CourseMap : EntityTypeConfiguration<Course>
    {
        public CourseMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CourseName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CourseDescription)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Course");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.CreditHours).HasColumnName("CreditHours");
            this.Property(t => t.CourseName).HasColumnName("CourseName");
            this.Property(t => t.CourseDescription).HasColumnName("CourseDescription");
        }
    }
}
