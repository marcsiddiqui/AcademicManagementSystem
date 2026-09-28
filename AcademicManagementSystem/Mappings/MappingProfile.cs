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

            CreateMap<Role, RoleModel>().ReverseMap();

            CreateMap<User, UserModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty));

            CreateMap<UserModel, User>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());
        }
    }
}
