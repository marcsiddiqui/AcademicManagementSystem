using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public StudentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = new StudentListModel();

            var students = _dbContext.Student.ToList();

            if (students != null && students.Any())
            {
                foreach (var student in students)
                {
                    var studentModel = new StudentModel
                    {
                        Id = student.Id,
                        FullName = student.FullName,
                        Email = student.Email,
                        Phone = student.Phone,
                        Gender = student.Gender,
                        DateOfBirth = student.DateOfBirth,
                        Address = student.Address,
                        AdmissionDate = student.AdmissionDate
                    };

                    model.Students.Add(studentModel);
                }
            }


            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new StudentModel();

            model.AdmissionDate = DateTime.Now;
            model.DateOfBirth = DateTime.Now.AddYears(-18);

            PrepareAvailableGenders(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentModel model)
        {
            bool isValid = Regex.IsMatch(model.FullName , @"^[\p{L}]+(?: [\p{L}]+)*$");
            if (!isValid)
                ModelState.AddModelError(nameof(model.FullName), "Invalid Full Name!");

            bool existingStudent = await _dbContext.Student.AnyAsync(d => d.FullName == model.FullName);
            if (existingStudent)
                ModelState.AddModelError(nameof(model.FullName), "Student already exists!");

            if (ModelState.IsValid)
            {
                var student = new Student
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    AdmissionDate = model.AdmissionDate,
                    IsActive = model.IsActive
                };

                _dbContext.Student.Add(student);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var student = await _dbContext.Student.FindAsync(id);
            if (student == null)
                return RedirectToAction("Index");

            var model = new StudentModel
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                Address = student.Address,
                AdmissionDate = student.AdmissionDate,
                IsActive = student.IsActive
            };

            PrepareAvailableGenders(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(StudentModel model)
        {
            bool isValid = Regex.IsMatch(model.FullName, @"^[\p{L}]+(?: [\p{L}]+)*$");
            if (!isValid)
                ModelState.AddModelError(nameof(model.FullName), "Invalid Full Name!");

            bool existingStudent = await _dbContext.Student.AnyAsync(d => d.FullName == model.FullName && d.Id != model.Id);
            if (existingStudent)
                ModelState.AddModelError(nameof(model.FullName), "Student already exists!");

            var student = await _dbContext.Student.FindAsync(model.Id);
            if (student == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                student.FullName = model.FullName;
                student.Email = model.Email;
                student.Phone = model.Phone;
                student.Gender = model.Gender;
                student.DateOfBirth = model.DateOfBirth;
                student.Address = model.Address;
                student.AdmissionDate = model.AdmissionDate;
                student.IsActive = model.IsActive;

                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var student = await _dbContext.Student.FindAsync(id);
            if (student == null)
                return RedirectToAction("Index");

            var model = new StudentModel
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                Address = student.Address,
                AdmissionDate = student.AdmissionDate,
                IsActive = student.IsActive
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(StudentModel model)
        {
            var student = await _dbContext.Student.FindAsync(model.Id);
            if (student == null)
                return RedirectToAction("Index");

            _dbContext.Student.Remove(student);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public void PrepareAvailableGenders(StudentModel model)
        {
            model.AvailableGenders.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select Gender",
                Selected = model.Gender == 0
            });

            model.AvailableGenders.Add(new SelectListItem
            {
                Value = "1",
                Text = "Female",
                Selected = model.Gender == 1
            });

            model.AvailableGenders.Add(new SelectListItem
            {
                Value = "2",
                Text = "Male",
                Selected = model.Gender == 2
            });

            model.AvailableGenders.Add(new SelectListItem
            {
                Value = "3",
                Text = "Prefer Not to Say",
                Selected = model.Gender == 3
            });
        }
    }
}
