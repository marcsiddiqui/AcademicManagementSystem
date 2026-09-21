using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Models
{
    public class EnrollmentModel
    {
        public EnrollmentModel()
        {
            AvailableStudents = new List<SelectListItem>();
            AvailableCourses = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Student is required!")]
        public int StudentId { get; set; }
        
        [DisplayName("Student Name")]
        public string? StudentName { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Course is required!")]
        public int CourseId { get; set; }
        
        [DisplayName("Course Name")]
        public string? CourseName { get; set; }

        [Required(ErrorMessage = "Enrollment date is required!")]
        [DisplayName("Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [NotMapped]
        public List<SelectListItem> AvailableStudents { get; set; }

        [NotMapped]
        public List<SelectListItem> AvailableCourses { get; set; }

        public decimal Fee { get; set; }
    }

    public class EnrollmentListModel
    {
        public EnrollmentListModel()
        {
            Enrollments = new List<EnrollmentModel>();
        }

        public List<EnrollmentModel> Enrollments { get; set; }
    }
}
