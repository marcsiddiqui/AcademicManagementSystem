namespace AcademicManagementSystem.DatabaseConfiguration
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

    }
}
