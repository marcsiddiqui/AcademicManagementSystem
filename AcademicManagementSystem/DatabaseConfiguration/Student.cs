namespace AcademicManagementSystem.DatabaseConfiguration
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public DateTime AdmissionDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
