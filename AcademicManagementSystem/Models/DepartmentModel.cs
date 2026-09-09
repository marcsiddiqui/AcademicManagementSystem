using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcademicManagementSystem.Models
{
    public class DepartmentModel
    {
        public int Id { get; set; }

        [DisplayName("Department Name")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Department Name is required!")]
        public string Name { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;
    }

    public class DepartmentListModel
    {
        public DepartmentListModel()
        {
            Departments = new List<DepartmentModel>();
        }

        public List<DepartmentModel> Departments { get; set; }
    }
}
