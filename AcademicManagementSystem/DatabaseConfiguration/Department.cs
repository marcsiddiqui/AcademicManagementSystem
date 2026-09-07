namespace AcademicManagementSystem.DatabaseConfiguration
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
