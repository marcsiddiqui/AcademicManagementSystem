using AcademicManagementSystem.DatabaseConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<User>> GetAllUsersAsync(string? search = null, int status = 0, int sortById = 0, int pageNumber = 1, int pageSize = 10)
        {
            var query = _dbContext.User
                .AsNoTracking()
                .Include(x => x.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.FullName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.Role.Name.Contains(search));
            }

            if (status == 1)
                query = query.Where(x => x.IsActive);
            else if (status == 2)
                query = query.Where(x => !x.IsActive);

            query = sortById switch
            {
                1 => query.OrderByDescending(x => x.FullName),
                2 => query.OrderBy(x => x.CreatedOnUtc),
                3 => query.OrderByDescending(x => x.CreatedOnUtc),
                _ => query.OrderBy(x => x.Id)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<User>
            {
                Records = users,
                TotalRecords = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
