using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public EnrollmentController(
            ApplicationDbContext dbContext,
            IMapper mapper
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var model = new EnrollmentListModel();

            #region Without Join

#if false
            var courses = _dbContext.Enrollment.ToList();
            var departments = _dbContext.Departments.ToList();

            if (courses != null && courses.Any())
            {
                foreach (var course in courses)
                {
                    var department = departments != null && departments.Any() ? departments.FirstOrDefault(x => x.Id == course.DepartmentId) : null;

                    var courseModel = new EnrollmentModel
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Fee = course.Fee,
                        DepartmentId = course.DepartmentId,
                        DepartmentName = department != null ? department.Name : string.Empty
                    };

                    model.Enrollments.Add(courseModel);
                }
            }
#endif

            #endregion

            #region With Join

#if false

            var data = from c in _dbContext.Enrollment
                       join d in _dbContext.Departments on c.DepartmentId equals d.Id
                       select new { c.Id, c.Name, c.Fee, c.DepartmentId, DepartmentName = d.Name };

            if (data != null && data.Any())
            {
                foreach (var course in data)
                {
                    var courseModel = new EnrollmentModel
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Fee = course.Fee,
                        DepartmentId = course.DepartmentId,
                        DepartmentName = course.DepartmentName
                    };

                    model.Enrollments.Add(courseModel);
                }
            }

#endif


            #endregion


            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new EnrollmentModel();

            PrepareAvailableStudents(model);
            PrepareAvailableCourses(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EnrollmentModel model)
        {
            if (ModelState.IsValid)
            {
                var enrollment = _mapper.Map<Enrollment>(model);

                _dbContext.Enrollment.Add(enrollment);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var course = await _dbContext.Enrollment.FindAsync(id);
            if (course == null)
                return RedirectToAction("Index");

            var model = _mapper.Map<EnrollmentModel>(course);

            PrepareAvailableStudents(model);
            PrepareAvailableCourses(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(EnrollmentModel model)
        {
            var course = await _dbContext.Enrollment.FindAsync(model.Id);
            if (course == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                _mapper.Map(model, course);

                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var course = await _dbContext.Enrollment.FindAsync(id);
            if (course == null)
                return RedirectToAction("Index");

            var model = _mapper.Map<EnrollmentModel>(course);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EnrollmentModel model)
        {
            var course = await _dbContext.Enrollment.FindAsync(model.Id);
            if (course == null)
                return RedirectToAction("Index");

            _dbContext.Enrollment.Remove(course);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public void PrepareAvailableStudents(EnrollmentModel model)
        {
            var students = _dbContext.Student.Where(x => x.IsActive).ToList();

            model.AvailableStudents.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select Student"
            });

            if (students != null && students.Any())
            {
                model.AvailableStudents.AddRange(students.Select(s =>
                new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.FullName,
                    Selected = s.Id == model.StudentId
                }).ToList());
            }
        }

        public void PrepareAvailableCourses(EnrollmentModel model)
        {
            var courses = _dbContext.Courses.ToList();

            model.AvailableCourses.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select Course"
            });

            if (courses != null && courses.Any())
            {
                model.AvailableCourses.AddRange(courses.Select(c =>
                new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == model.CourseId
                }).ToList());
            }
        }
    }
}
