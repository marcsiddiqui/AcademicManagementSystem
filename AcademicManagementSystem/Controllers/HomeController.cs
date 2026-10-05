using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AcademicManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly StudentService _studentService;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(StudentService studentService, ApplicationDbContext dbContext)
        {
            _studentService = studentService;
            _dbContext = dbContext;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public virtual async Task<JsonResult> GetDashboardData(string Username, string Email)
        {
            var studentsCount = _studentService.GetAllStudentsAsync(pageNumber: 1, pageSize: int.MaxValue).Result.TotalRecords;
            var coursesCount = _dbContext.Courses.CountAsync().Result;
            var departmentsCount = _dbContext.Departments.CountAsync().Result;
            var enrollmentCount = _dbContext.Enrollment.CountAsync().Result;

            var model = new DashboardModel
            {
                StudentsCount = studentsCount,
                CoursesCount = coursesCount,
                DepartmentsCount = departmentsCount,
                EnrollmentsCount = enrollmentCount
            };

            return Json(new { Success = true, Data = model });
        }
    }
}
