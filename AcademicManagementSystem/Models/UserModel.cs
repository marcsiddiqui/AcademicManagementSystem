using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcademicManagementSystem.Models
{
    public class UserModel
    {
        public UserModel()
        {
            AvailableRoles = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [DisplayName("Full Name")]
        [StringLength(50, MinimumLength = 3)]
        [Required(ErrorMessage = "Full Name is required!")]
        public string FullName { get; set; } = string.Empty;

        [DisplayName("Email")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Enter a valid email address!")]
        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; } = string.Empty;

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; } = string.Empty;

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; } = string.Empty;

        [DisplayName("Role")]
        [Range(1, int.MaxValue, ErrorMessage = "Role is required!")]
        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; } = true;

        [DisplayName("Created On")]
        public DateTime CreatedOnUtc { get; set; }

        public List<SelectListItem> AvailableRoles { get; set; }

        public string? ImagePath { get; set; }
    }

    public class UserListModel
    {
        public UserListModel()
        {
            Users = new List<UserModel>();
        }

        public List<UserModel> Users { get; set; }
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
