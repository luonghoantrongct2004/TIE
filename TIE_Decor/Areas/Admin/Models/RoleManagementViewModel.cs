using Microsoft.AspNetCore.Identity;
using TIE_Decor.Models;

namespace TIE_Decor.Areas.Admin.Models
{
	public class RoleManagementViewModel
	{
		public List<IdentityRole> Roles { get; set; }
		public List<UserRolesViewModel> UserList { get; set; }
	}

}
