using AcademicManagementSystem.DatabaseConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Services
{
    public class RoleService
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<Role>> GetAllRolesAsync(string? search = null, int status = 0, int sortById = 0, int pageNumber = 1, int pageSize = 10)
        {
            var query = _dbContext.Role.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search));

            if (status == 1)
                query = query.Where(x => x.IsActive);
            else if (status == 2)
                query = query.Where(x => !x.IsActive);

            query = sortById switch
            {
                1 => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Id)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var roles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<Role>
            {
                Records = roles,
                TotalRecords = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
