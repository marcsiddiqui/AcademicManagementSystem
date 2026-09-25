using AcademicManagementSystem.DatabaseConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Services
{
    public class StudentService
    {
        private readonly ApplicationDbContext _dbContext;
        public StudentService(
            ApplicationDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<Student>> GetAllStudentsAsync(string search = null, int status = 0, int sortById = 0, int pageNumber = 0, int pageSize = 10)
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
                    query = query.OrderBy(x => x.Id);
                    break;
            }

            // total count
            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // pagination
            var students = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var pagedList = new PagedList<Student>
            {
                Records = students,
                TotalRecords = totalCount,
                TotalPages = totalPages
            };

            return pagedList;
        }
    }
}
