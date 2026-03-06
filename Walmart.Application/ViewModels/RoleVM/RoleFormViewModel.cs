using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.ViewModels.RoleVM
{
    public class RoleFormViewModel
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}
