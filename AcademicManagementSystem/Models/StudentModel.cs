using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcademicManagementSystem.Models
{
    public class StudentModel
    {
        public StudentModel()
        {
            AvailableGenders = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [DisplayName("Full Name")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Full Name is required!")]
        public string StudentFullName { get; set; }

        [DisplayName("Email")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; }

        [DisplayName("Phone")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Phone is required!")]
        public string Phone { get; set; }

        [DisplayName("Gender")]
        [Required(ErrorMessage = "Gender is required!")]
        public int Gender { get; set; }

        [DisplayName("Date of Birth")]
        [Required(ErrorMessage = "Date of Birth is required!")]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Admission Date")]
        public DateTime AdmissionDate { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> AvailableGenders { get; set; }

    }

    public class StudentListModel
    {
        public StudentListModel()
        {
            Students = new List<StudentModel>();
        }

        public List<StudentModel> Students { get; set; }
    }
}
