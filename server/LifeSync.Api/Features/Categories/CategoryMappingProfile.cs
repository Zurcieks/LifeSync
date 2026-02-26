using AutoMapper;
using LifeSync.Api.Data.Entities;

namespace LifeSync.Api.Features.Categories;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
