using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcademicManagementSystem.Models
{
    public class DashboardModel
    {
        public DashboardModel()
        {
        }

        public int StudentsCount { get; set; }
        public int CoursesCount { get; set; }
        public int EnrollmentsCount { get; set; }
        public int DepartmentsCount { get; set; }
    }
}
