using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AcademicManagementSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademicManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/students")]
    public class StudentApiController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentApiController(
            IStudentService studentService,
            IMapper mapper
            )
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<StudentListModel>> GetAllStudents()
        {
            var pagedData = await _studentService.GetAllStudentsAsync(
                pageNumber: 1,
                pageSize: int.MaxValue);

            var model = new StudentListModel();

            if (pagedData != null)
                if (pagedData.Records != null && pagedData.Records.Any())
                    foreach (var student in pagedData.Records)
                        model.Students.Add(_mapper.Map<StudentModel>(student));

            return Ok(model);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentModel>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();

            return Ok(_mapper.Map<StudentModel>(student));
        }

        [HttpPost]
        public async Task<ActionResult<StudentModel>> CreateStudent(StudentModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = _mapper.Map<Student>(model);

#warning In a real application, you would get the user ID from the authenticated user context, not hardcode it. Fix it later
            student.CreatedBy = 1;
            student.CreatedOnUtc = DateTime.UtcNow;

            var createdStudent = await _studentService.CreateStudentAsync(student);

            return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.Id }, _mapper.Map<StudentModel>(createdStudent));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStudent(int id, StudentModel model)
        {
            if (id != model.Id)
                return BadRequest("Student ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
                return NotFound();

            model.CreatedBy = existingStudent.CreatedBy;
            model.CreatedOnUtc = existingStudent.CreatedOnUtc;

            _mapper.Map(model, existingStudent);

#warning In a real application, you would get the user ID from the authenticated user context, not hardcode it. Fix it later
            existingStudent.UpdatedBy = 1;
            existingStudent.UpdatedOnUtc = DateTime.UtcNow;

            await _studentService.UpdateStudentAsync(existingStudent);

            return NoContent();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
         
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
