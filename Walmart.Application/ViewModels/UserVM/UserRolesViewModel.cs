using System;
using System.Collections.Generic;
using System.Text;
using Walmart.Application.ViewModels.RoleVM;

namespace Walmart.Application.ViewModels.UserVM
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
