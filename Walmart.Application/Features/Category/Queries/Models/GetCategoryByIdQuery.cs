using MediatR;
using Walmart.Application.ViewModels.Category;

namespace Walmart.Application.Features.Category.Queries.Models
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailsVM>
    {
        public int Id { get; set; }
    }
}
