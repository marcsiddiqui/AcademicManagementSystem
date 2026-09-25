using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.NativeInterop;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly StudentService _studentService;

        const int PageSize = 10;

        public StudentController(
            ApplicationDbContext dbContext,
            IMapper mapper,
            StudentService studentService
            )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _studentService = studentService;
        }

        public async Task<IActionResult> Index(StudentListModel model)
        {
            //var data = _dbContext.Student.FromSqlRaw("SELECT * FROM Student Where Id = 7").ToList();

            //await _dbContext.Database.ExecuteSqlRawAsync("Update student set isactive = 0");

            var pagedData = await _studentService.GetAllStudentsAsync(
                search: model.SearchText,
                status: model.StatusId,
                sortById: model.SortById,
                pageNumber: model.PageNumber,
                pageSize: PageSize);

            if (pagedData != null)
            {
                if (pagedData.Records != null && pagedData.Records.Any())
                {
                    foreach (var student in pagedData.Records)
                    {
                        var studentModel = _mapper.Map<StudentModel>(student);

                        model.Students.Add(studentModel);
                    }
                }

                model.PageSize = PageSize;
                model.TotalPages = pagedData.TotalPages;
                model.TotalRecords = pagedData.TotalRecords;
                model.HasNextPage = (model.PageNumber) < model.TotalPages;
                model.HasPreviousPage = model.PageNumber > 1;
                model.ShowingFrom = ((model.PageNumber - 1) * model.PageSize) + 1;
                model.ShowingTo = model.PageNumber == model.TotalPages ? model.TotalRecords : model.PageNumber * model.PageSize;
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
            bool isValid = Regex.IsMatch(model.StudentFullName , @"^[\p{L}]+(?: [\p{L}]+)*$");
            if (!isValid)
                ModelState.AddModelError(nameof(model.StudentFullName), "Invalid Full Name!");

            bool existingStudent = await _dbContext.Student.AnyAsync(d => d.FullName == model.StudentFullName);
            if (existingStudent)
                ModelState.AddModelError(nameof(model.StudentFullName), "Student already exists!");

            if (ModelState.IsValid)
            {
                var student = _mapper.Map<Student>(model);

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

            var model = _mapper.Map<StudentModel>(student);

            PrepareAvailableGenders(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(StudentModel model)
        {
            bool isValid = Regex.IsMatch(model.StudentFullName, @"^[\p{L}]+(?: [\p{L}]+)*$");
            if (!isValid)
                ModelState.AddModelError(nameof(model.StudentFullName), "Invalid Full Name!");

            bool existingStudent = await _dbContext.Student.AnyAsync(d => d.FullName == model.StudentFullName && d.Id != model.Id);
            if (existingStudent)
                ModelState.AddModelError(nameof(model.StudentFullName), "Student already exists!");

            var student = await _dbContext.Student.FindAsync(model.Id);
            if (student == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                _mapper.Map(model, student);

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

            var existingEnrollments = _dbContext.Enrollment.Any(x => x.StudentId == student.Id);
            if (existingEnrollments)
                return RedirectToAction("Index");

            var model = _mapper.Map<StudentModel>(student);

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
