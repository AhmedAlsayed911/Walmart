using MediatR;
using Walmart.Application.ViewModels.Category;

namespace Walmart.Application.Features.Category.Queries.Models
{
    public class GetAllCategoriesForDropdownList : IRequest<List<CategoryListVM>>
    {
    }
}
