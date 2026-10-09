using AcademicManagementSystem.DatabaseConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Services
{
    public class StudentService : IStudentService
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

        public Task<Student?> GetStudentByIdAsync(int id)
        {
            return _dbContext.Student.FirstOrDefaultAsync(student => student.Id == id);
        }

        public Task<List<Student>> GetActiveStudentsAsync()
        {
            return _dbContext.Student
                .Where(student => student.IsActive)
                .OrderBy(student => student.FullName)
                .ToListAsync();
        }

        public Task<bool> StudentExistsAsync(string fullName, int? excludedId = null)
        {
            return _dbContext.Student.AnyAsync(student =>
                student.FullName == fullName && (!excludedId.HasValue || student.Id != excludedId.Value));
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            _dbContext.Student.Add(student);
            await _dbContext.SaveChangesAsync();
            return student;
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _dbContext.Student.Update(student);
            await _dbContext.SaveChangesAsync();
        }

        public Task<bool> HasEnrollmentsAsync(int studentId)
        {
            return _dbContext.Enrollment.AnyAsync(enrollment => enrollment.StudentId == studentId);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await GetStudentByIdAsync(id);
            if (student == null || await HasEnrollmentsAsync(id))
                return false;

            _dbContext.Student.Remove(student);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
