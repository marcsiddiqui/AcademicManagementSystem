using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Cryptography;
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

            #region With Join

#if false

            var data_list = from e in _dbContext.Enrollment
                       join s in _dbContext.Student on e.StudentId equals s.Id
                       join c in _dbContext.Courses on e.CourseId equals c.Id
                       select new { e.Id, e.EnrollmentDate, e.Status, e.IsActive, StudentName = s.FullName, CourseName = c.Name, c.Fee };

            if (data_list != null && data_list.Any())
            {
                foreach (var data in data_list)
                {
                    var enrollmentModel = new EnrollmentModel
                    {
                        Id = data.Id,
                        StudentName = data.StudentName,
                        CourseName = data.CourseName,
                        EnrollmentDate = data.EnrollmentDate,
                        Status = data.Status,
                        IsActive = data.IsActive,
                        Fee = data.Fee
                    };

                    model.Enrollments.Add(enrollmentModel);
                }
            }

#endif

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

            PrepareAvailableStudents(model);
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

            PrepareAvailableStudents(model);
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

            PrepareAvailableStudents(model);
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

            PrepareAvailableStudents(model);
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
