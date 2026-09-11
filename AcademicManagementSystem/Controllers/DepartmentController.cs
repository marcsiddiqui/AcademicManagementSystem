using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;

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

        public async Task<IActionResult> Create()
        {
            var model = new DepartmentModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentModel model)
        {
            bool isValid = Regex.IsMatch(model.Name, @"^[\p{L}]+(?: [\p{L}]+)*$");
            if (!isValid)
                ModelState.AddModelError(nameof(model.Name), "Invalid Name!");

            bool existingDepartment = await _dbContext.Departments.AnyAsync(d => d.Name == model.Name);
            if (existingDepartment)
                ModelState.AddModelError(nameof(model.Name), "Department already exists!");

            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    Name = model.Name,
                    IsActive = model.IsActive,
                };

                _dbContext.Departments.Add(department);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var department = await _dbContext.Departments.FindAsync(id);
            if (department == null)
                return RedirectToAction("Index");

            var model = new DepartmentModel
            {
                Id = department.Id,
                Name = department.Name,
                IsActive = department.IsActive
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(DepartmentModel model)
        {
            bool isValid = Regex.IsMatch(model.Name, @"^[\p{L}]+(?: [\p{L}]+)*$");
            if (!isValid)
                ModelState.AddModelError(nameof(model.Name), "Invalid Name!");

            bool existingDepartment = await _dbContext.Departments.AnyAsync(d => d.Name == model.Name && d.Id != model.Id);
            if (existingDepartment)
                ModelState.AddModelError(nameof(model.Name), "Department already exists!");

            var department = await _dbContext.Departments.FindAsync(model.Id);
            if (department == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                department.Name = model.Name;
                department.IsActive = model.IsActive;

                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var department = await _dbContext.Departments.FindAsync(id);
            if (department == null)
                return RedirectToAction("Index");

            var model = new DepartmentModel
            {
                Id = department.Id,
                Name = department.Name,
                IsActive = department.IsActive
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DepartmentModel model)
        {
            var department = await _dbContext.Departments.FindAsync(model.Id);
            if (department == null)
                return RedirectToAction("Index");

            _dbContext.Departments.Remove(department);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
