using MediatR;

namespace Walmart.Application.Features.Category.Commands.Models
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
