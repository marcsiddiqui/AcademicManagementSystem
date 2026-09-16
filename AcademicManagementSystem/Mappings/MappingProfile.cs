using AcademicManagementSystem.DatabaseConfiguration;
using AcademicManagementSystem.Models;
using AutoMapper;

namespace AcademicManagementSystem.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentModel>()
                .ForMember(dest => dest.StudentFullName, opt => opt.MapFrom(src => src.FullName))
                .ReverseMap();

            CreateMap<Course, CourseModel>().ReverseMap();

            CreateMap<Enrollment, EnrollmentModel>().ReverseMap();

            CreateMap<Department, DepartmentModel>().ReverseMap();
        }
    }
}
