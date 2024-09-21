using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIE_Decor.Areas.Admin.Models;
using TIE_Decor.Entities;
using TIE_Decor.Models;

[Area("Admin")]
public class RoleController : Controller
{

	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly UserManager<User> _userManager;

	public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
	{
		_roleManager = roleManager;
		_userManager = userManager;
	}

	// GET: /admin/role/index
	public async Task<IActionResult> Index()
	{
		var roles = await _roleManager.Roles.ToListAsync();
		var users = await _userManager.Users.ToListAsync();
		var userRolesViewModel = new List<UserRolesViewModel>();

		foreach (var user in users)
		{
			var rolesForUser = await _userManager.GetRolesAsync(user);
			userRolesViewModel.Add(new UserRolesViewModel
			{
				UserId = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				CurrentRole = rolesForUser.FirstOrDefault() ?? "No role"
			});
		}

		var viewModel = new RoleManagementViewModel
		{
			Roles = roles,
			UserList = userRolesViewModel
		};

		return View(viewModel);
	}

	// POST: /admin/role/assignRole
	[HttpPost]
	public async Task<IActionResult> AssignRole(Dictionary<string, string> roles)
	{
		foreach (var roleAssignment in roles)
		{
			var userId = roleAssignment.Key;
			var newRole = roleAssignment.Value;

			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				var currentRoles = await _userManager.GetRolesAsync(user);
				if (!string.IsNullOrEmpty(newRole) && !currentRoles.Contains(newRole))
				{
					// Loại bỏ tất cả các vai trò hiện tại
					await _userManager.RemoveFromRolesAsync(user, currentRoles);

					// Gán vai trò mới
					await _userManager.AddToRoleAsync(user, newRole);
				}
				else if (string.IsNullOrEmpty(newRole))
				{
					// Loại bỏ tất cả các vai trò nếu không chọn vai trò mới
					await _userManager.RemoveFromRolesAsync(user, currentRoles);
				}
			}
		}

		TempData["SuccessMessage"] = "Roles updated successfully.";
		return Redirect("/admin/role");
	}

	// POST: /admin/role/CreateRole
	[HttpPost]
	public async Task<IActionResult> CreateRole(string roleName)
	{
		if (string.IsNullOrWhiteSpace(roleName))
		{
			TempData["ErrorMessage"] = "Role name cannot be empty.";
			return Redirect("/admin/role");
		}

		var roleExists = await _roleManager.RoleExistsAsync(roleName);
		if (roleExists)
		{
			TempData["ErrorMessage"] = "Role already exists.";
			return Redirect("/admin/role");
		}

		var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
		if (result.Succeeded)
		{
			TempData["SuccessMessage"] = "Role created successfully.";
		}
		else
		{
			TempData["ErrorMessage"] = "Error creating role.";
		}

		return Redirect("/admin/role");
	}

	// POST: /admin/role/UpdateRole
	[HttpPost]
	public async Task<IActionResult> UpdateRole(string RoleId, string RoleName)
	{
		if (string.IsNullOrWhiteSpace(RoleId) || string.IsNullOrWhiteSpace(RoleName))
		{
			TempData["ErrorMessage"] = "Invalid role data.";
			return Redirect("/admin/role");
		}

		var role = await _roleManager.FindByIdAsync(RoleId);
		if (role == null)
		{
			TempData["ErrorMessage"] = "Role not found.";
			return Redirect("/admin/role");
		}

		role.Name = RoleName;
		var result = await _roleManager.UpdateAsync(role);
		if (result.Succeeded)
		{
			TempData["SuccessMessage"] = "Role updated successfully.";
		}
		else
		{
			TempData["ErrorMessage"] = "Error updating role.";
		}

		return Redirect("/admin/role");
	}

	// POST: /admin/role/DeleteRole
	[HttpPost]
	public async Task<IActionResult> Delete(string RoleId)
	{
		if (string.IsNullOrWhiteSpace(RoleId))
		{
			TempData["ErrorMessage"] = "Invalid role id.";
			return Redirect("/admin/role");
		}

		var role = await _roleManager.FindByIdAsync(RoleId);
		if (role == null)
		{
			TempData["ErrorMessage"] = "Role not found.";
			return Redirect("/admin/role");
		}

		var result = await _roleManager.DeleteAsync(role);
		if (result.Succeeded)
		{
			TempData["SuccessMessage"] = "Role deleted successfully.";
		}
		else
		{
			TempData["ErrorMessage"] = "Error deleting role.";
		}

		return Redirect("/admin/role");
	}
}
