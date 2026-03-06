using AutoMapper;
using Walmart.Application.Features.Category.Commands.Models;

namespace Walmart.Application.Mapping.Category
{
    public partial class CategoryProfile
    {
        public void AddCategoryMapping()
        {
            CreateMap<AddCategoryCommand, Walmart.Domain.Entities.Category>();
        }
    }
}
