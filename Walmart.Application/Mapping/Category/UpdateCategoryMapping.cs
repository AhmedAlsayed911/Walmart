using AutoMapper;
using Walmart.Application.Features.Category.Commands.Models;

namespace Walmart.Application.Mapping.Category
{
    public partial class CategoryProfile
    {
        public void UpdateCategoryMapping()
        {
            CreateMap<UpdateCategoryCommand, Walmart.Domain.Entities.Category>();
        }
    }
}
