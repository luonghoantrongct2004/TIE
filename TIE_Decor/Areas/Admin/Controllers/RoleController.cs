using TIE_Decor.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TIE_Decor.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class RoleController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // List all roles (Read)
    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    // Create a new role
    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            return BadRequest("Role name is required");
        }

        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return BadRequest("Role already exists");
        }

        var role = new IdentityRole(roleName);
        var result = await _roleManager.CreateAsync(role);
        if (result.Succeeded)
        {
            return Ok("Role created successfully");
        }

        return BadRequest("Failed to create role");
    }

    // Update role name
    [HttpPost]
    public async Task<IActionResult> UpdateRole(string roleId, string newRoleName)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return NotFound("Role not found");
        }

        if (string.IsNullOrEmpty(newRoleName))
        {
            return BadRequest("New role name is required");
        }

        role.Name = newRoleName;
        var result = await _roleManager.UpdateAsync(role);
        if (result.Succeeded)
        {
            return Ok("Role updated successfully");
        }

        return BadRequest("Failed to update role");
    }

    // Delete role
    [HttpPost]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return NotFound("Role not found");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return Ok("Role deleted successfully");
        }

        return BadRequest("Failed to delete role");
    }

    // Assign role to user (already implemented)
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return BadRequest("Role does not exist");
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        if (result.Succeeded)
        {
            return Ok("Role assigned successfully");
        }

        return BadRequest("Failed to assign role");
    }
}
