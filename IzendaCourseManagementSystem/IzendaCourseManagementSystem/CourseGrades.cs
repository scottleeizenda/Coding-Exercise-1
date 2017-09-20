namespace IzendaCourseManagementSystem
{
    public class CourseGrades
    {
        // Declarations & Getters/Setters
        public int Id { get; set; }
        public int CourseId { get; set; }
        public char FinalGrade { get; set; }

        public CourseGrades(int id, int courseId, char finalGrade)
        {
            Id = id;
            CourseId = courseId;
            FinalGrade = finalGrade;
        }

        public override string ToString()
        {
            return $"Course ID: {CourseId} - Final Grade: {FinalGrade}\n";
        }
    }
}
