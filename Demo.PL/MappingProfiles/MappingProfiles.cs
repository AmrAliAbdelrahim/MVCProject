using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Demo.PL.Helpers.MappingProfiles
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<EmployeeViewModel, Employee>().ReverseMap();
            CreateMap<DepartmentViewModel, Department>().ReverseMap();
            CreateMap<UserViwModel, ApplicationUser>().ReverseMap();
            CreateMap<IdentityRole,RoleViewModel>()
                .ForMember(d=>d.RoleName,O=>O.MapFrom(s=>s.Name)).ReverseMap();
        }
    }
}
