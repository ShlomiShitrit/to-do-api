using AutoMapper;
using Backend.Dto;
using Backend.Models;

namespace Backend.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore());
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Tasks, opt => opt.Ignore());
            CreateMap<SubCategory, SubCategoryDto>();
            CreateMap<SubCategoryDto, SubCategory>()
                .ForMember(dest => dest.Tasks, opt => opt.Ignore());
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
            CreateMap<User, UserDto>();
            CreateMap<TaskModel, TaskDto>();
            CreateMap<TaskDto, TaskModel>();
        }

    }
}