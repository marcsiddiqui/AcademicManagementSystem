namespace AcademicManagementSystem.DatabaseConfiguration
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOnUtc { get; set; }
        public Role Role { get; set; } = null!;
        public string? ImagePath { get; set; }
    }
}
