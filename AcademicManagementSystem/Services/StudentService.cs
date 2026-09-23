using AcademicManagementSystem.DatabaseConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Services
{
    public class StudentService
    {
        const int PageSize = 10;

        private readonly ApplicationDbContext _dbContext;
        public StudentService(
            ApplicationDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<List<Student>> GetAllStudentsAsync(string search = null, int status = 0, int sortById = 0, int pageNumber = 0)
        {
            // iqueryable
            var query = _dbContext.Student.AsQueryable();

            // filters
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.FullName.Contains(search) || x.Email.Contains(search) || x.Phone.Contains(search));

            if (status > 0)
            {
                if (status == 1)
                    query = query.Where(x => x.IsActive);
                else if (status == 2)
                    query = query.Where(x => !x.IsActive);
            }

            // sorting
            switch (sortById)
            {
                case 1:
                    query = query.OrderByDescending(x => x.FullName);
                    break;
                case 2:
                    query = query.OrderBy(x => x.AdmissionDate);
                    break;
                case 3:
                    query = query.OrderByDescending(x => x.AdmissionDate);
                    break;
                default:
                    query = query.OrderBy(x => x.FullName);
                    break;
            }

            // pagination
            var students = await query.Skip(pageNumber - 1 * PageSize).Take(PageSize).ToListAsync();

            return students;
        }
    }
}
