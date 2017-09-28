using System;
using System.Collections.Generic;

namespace IzendaCMS.DataModel.Models
{
    public partial class Instructor_Course
    {
        public Instructor_Course() { }

        public Instructor_Course(int id, int instructorId, int courseId)
        {
            Id = id;
            InstructorId = instructorId;
            CourseId = courseId;
        }

        public int Id { get; set; }
        public int InstructorId { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}
