using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace AcademicManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DepartmentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = new DepartmentListModel();

            var departments = _dbContext.Departments.ToList();

            if (departments != null && departments.Any())
            {
                foreach (var department in departments)
                {
                    var departmentModel = new DepartmentModel
                    {
                        Id = department.Id,
                        Name = department.Name,
                        IsActive = department.IsActive
                    };

                    model.Departments.Add(departmentModel);
                }
            }


            return View(model);
        }

        public IActionResult Create()
        {
            var model = new DepartmentModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(DepartmentModel model)
        {
            return View(model);
        }
    }
}
