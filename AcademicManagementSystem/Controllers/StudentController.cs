using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AcademicManagementSystem.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        const int PageSize = 10;

        public StudentController(
            IMapper mapper,
            IStudentService studentService,
            ILogger<StudentController> logger
            )
        {
            _mapper = mapper;
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(StudentListModel model)
        {
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

            bool existingStudent = await _studentService.StudentExistsAsync(model.StudentFullName);
            if (existingStudent)
                ModelState.AddModelError(nameof(model.StudentFullName), "Student already exists!");

            if (ModelState.IsValid)
            {
                try
                {
                    var student = _mapper.Map<Student>(model);

                    var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int parsedUserId) ? parsedUserId : 0;

                    student.CreatedBy = userId;
                    student.CreatedOnUtc = DateTime.UtcNow;

                    await _studentService.CreateStudentAsync(student);

                    _logger.LogInformation("Student created successfully. Student ID: {StudentId}, Created By User ID: {UserId}", student.Id, userId);

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    _logger.LogError("An error occurred while creating the student. Student Full Name: {StudentFullName}", model.StudentFullName);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var student = await _studentService.GetStudentByIdAsync(id);
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

            bool existingStudent = await _studentService.StudentExistsAsync(model.StudentFullName, model.Id);
            if (existingStudent)
                ModelState.AddModelError(nameof(model.StudentFullName), "Student already exists!");

            var student = await _studentService.GetStudentByIdAsync(model.Id);
            if (student == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                model.CreatedBy = student.CreatedBy;
                model.CreatedOnUtc = student.CreatedOnUtc;

                _mapper.Map(model, student);

                var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int parsedUserId) ? parsedUserId : 0;

                student.UpdatedBy = userId;
                student.UpdatedOnUtc = DateTime.UtcNow;

                await _studentService.UpdateStudentAsync(student);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null)
                return RedirectToAction("Index");

            var existingEnrollments = await _studentService.HasEnrollmentsAsync(student.Id);
            if (existingEnrollments)
                return RedirectToAction("Index");

            var model = _mapper.Map<StudentModel>(student);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(StudentModel model)
        {
            await _studentService.DeleteStudentAsync(model.Id);

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
