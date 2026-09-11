using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CourseController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = new CourseListModel();

            var courses = _dbContext.Courses.ToList();

            if (courses != null && courses.Any())
            {
                foreach (var course in courses)
                {
                    var courseModel = new CourseModel
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Fee = course.Fee
                    };

                    model.Courses.Add(courseModel);
                }
            }


            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _dbContext.Departments.ToListAsync();

            var model = new CourseModel();

            model.AvailableDepartments = departments.Select(d => 
            new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseModel model)
        {
            bool existingCourse = await _dbContext.Courses.AnyAsync(d => d.Name == model.Name);
            if (existingCourse)
                ModelState.AddModelError(nameof(model.Name), "Course already exists!");

            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    Name = model.Name,
                    Fee = model.Fee,
                    DepartmentId = model.DepartmentId
                };

                _dbContext.Courses.Add(course);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var course = await _dbContext.Courses.FindAsync(id);
            if (course == null)
                return RedirectToAction("Index");

            var model = new CourseModel
            {
                Id = course.Id,
                Name = course.Name,
                Fee = course.Fee
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(CourseModel model)
        {
            bool existingCourse = await _dbContext.Courses.AnyAsync(d => d.Name == model.Name && d.Id != model.Id);
            if (existingCourse)
                ModelState.AddModelError(nameof(model.Name), "Course already exists!");

            var course = await _dbContext.Courses.FindAsync(model.Id);
            if (course == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                course.Name = model.Name;
                course.Fee = model.Fee;

                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var course = await _dbContext.Courses.FindAsync(id);
            if (course == null)
                return RedirectToAction("Index");

            var model = new CourseModel
            {
                Id = course.Id,
                Name = course.Name,
                Fee = course.Fee
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CourseModel model)
        {
            var course = await _dbContext.Courses.FindAsync(model.Id);
            if (course == null)
                return RedirectToAction("Index");

            _dbContext.Courses.Remove(course);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
