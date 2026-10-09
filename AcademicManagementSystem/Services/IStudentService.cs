using AcademicManagementSystem.DatabaseConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Services
{
    public interface IStudentService
    {
        Task<PagedList<Student>> GetAllStudentsAsync(string search = null, int status = 0, int sortById = 0, int pageNumber = 0, int pageSize = 10);
        Task<Student?> GetStudentByIdAsync(int id);
        Task<List<Student>> GetActiveStudentsAsync();
        Task<bool> StudentExistsAsync(string fullName, int? excludedId = null);
        Task<Student> CreateStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task<bool> HasEnrollmentsAsync(int studentId);
        Task<bool> DeleteStudentAsync(int id);
    }
}
