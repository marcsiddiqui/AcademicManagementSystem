using AcademicManagementSystem.DatabaseConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Services
{
    public interface IStudentService
    {
        Task<PagedList<Student>> GetAllStudentsAsync(string search = null, int status = 0, int sortById = 0, int pageNumber = 0, int pageSize = 10);
    }
}
