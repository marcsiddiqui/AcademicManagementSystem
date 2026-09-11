using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcademicManagementSystem.Models
{
    public class CourseModel
    {
        public CourseModel()
        {
            AvailableDepartments = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [DisplayName("Course Name")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Course Name is required!")]
        public string Name { get; set; }

        [DisplayName("Fees")]
        [Required(ErrorMessage = "Fees is required!")]
        public decimal Fee { get; set; }

        public int DepartmentId { get; set; }

        public List<SelectListItem> AvailableDepartments { get; set; }
    }

    public class CourseListModel
    {
        public CourseListModel()
        {
            Courses = new List<CourseModel>();
        }

        public List<CourseModel> Courses { get; set; }
    }
}
