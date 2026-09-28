using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcademicManagementSystem.Models
{
    public class RoleModel
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [StringLength(50, MinimumLength = 2)]
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;
    }

    public class RoleListModel
    {
        public RoleListModel()
        {
            Roles = new List<RoleModel>();
        }

        public List<RoleModel> Roles { get; set; }

        public int StatusId { get; set; }
        public int SortById { get; set; }
        public string? SearchText { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int ShowingFrom { get; set; }
        public int ShowingTo { get; set; }
    }
}
