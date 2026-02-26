using AutoMapper;
using LifeSync.Api.Data.Entities;

namespace LifeSync.Api.Features.Expenses;

public class ExpenseMappingProfile : Profile
{
    public ExpenseMappingProfile()
    {
        CreateMap<Expense, ExpenseDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.CategoryColor, opt => opt.MapFrom(s => s.Category.Color));
    }
}
