using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IStudentService _studentService;

        public EnrollmentController(
            ApplicationDbContext dbContext,
            IMapper mapper,
            IStudentService studentService
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            var model = new EnrollmentListModel();

            #region With Join

            #endregion

            #region With StoredProcedure

            var data2 = _dbContext.Database.SqlQueryRaw<EnrollmentModel>("exec EnrollmentInfo;").ToList();
            if (data2 != null && data2.Any())
                model.Enrollments.AddRange(data2);

            #endregion


            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new EnrollmentModel();

            model.EnrollmentDate = DateTime.Now;

            await PrepareAvailableStudentsAsync(model);
            PrepareAvailableCourses(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EnrollmentModel model)
        {
            var hasExistingEnrollments = _dbContext.Enrollment.Any(x => x.StudentId == model.StudentId && x.CourseId == model.CourseId);
            if (hasExistingEnrollments)
                ModelState.AddModelError(nameof(model.StudentId), "Enrollment against this Student and Course Already Exists!");

            if (ModelState.IsValid)
            {
                var enrollment = _mapper.Map<Enrollment>(model);
                enrollment.Course = null;

                _dbContext.Enrollment.Add(enrollment);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            await PrepareAvailableStudentsAsync(model);
            PrepareAvailableCourses(model);

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

            await PrepareAvailableStudentsAsync(model);
            PrepareAvailableCourses(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(EnrollmentModel model)
        {
            var enrollment = await _dbContext.Enrollment.FindAsync(model.Id);
            if (enrollment == null)
                return RedirectToAction("Index");

            var hasExistingEnrollments = _dbContext.Enrollment.Any(x => x.Id != enrollment.Id && x.StudentId == model.StudentId && x.CourseId == model.CourseId);
            if (hasExistingEnrollments)
                ModelState.AddModelError(nameof(model.StudentId), "Enrollment against this Student and Course Already Exists!");

            if (ModelState.IsValid)
            {
                _mapper.Map(model, enrollment);
                enrollment.Course = null;

                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            await PrepareAvailableStudentsAsync(model);
            PrepareAvailableCourses(model);

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

        public async Task PrepareAvailableStudentsAsync(EnrollmentModel model)
        {
            var students = await _studentService.GetActiveStudentsAsync();

            model.AvailableStudents.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select Student"
            });

            if (students.Any())
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
