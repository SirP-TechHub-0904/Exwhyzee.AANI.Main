using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Participant> _signInManager;

        public ManageRolesModel(
            UserManager<Participant> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<Participant> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // Simple role view model for the page
        public class RoleViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public int UserCount { get; set; }
        }

        // Roles shown on the page
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

        [BindProperty]
        [Display(Name = "Role name")]
        public string CreateRoleName { get; set; } = string.Empty;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        // Default roles used for seeding / reset
        private static readonly string[] DefaultRoles = new[]
        {
            "Admin",
            "AANI",
            "Events",
            "Officials",
            "ChapterElection",
            "GeneralElection",
            "Library",
            "Finances",
            "Birthdays",
            "Tools",
            "Slider",
            "Gallery",
            "Users",
            "Content"
        };

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadRolesAsync();
            return Page();
        }

        private async Task LoadRolesAsync()
        {
            Roles.Clear();
            var allRoles = _roleManager.Roles.ToList();
            foreach (var r in allRoles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(r.Name);
                Roles.Add(new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name ?? string.Empty,
                    UserCount = usersInRole?.Count ?? 0
                });
            }
            Roles = Roles.OrderBy(r => r.Name).ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (string.IsNullOrWhiteSpace(CreateRoleName))
            {
                ErrorMessage = "Role name cannot be empty.";
                await LoadRolesAsync();
                return Page();
            }

            var exists = await _roleManager.RoleExistsAsync(CreateRoleName.Trim());
            if (exists)
            {
                ErrorMessage = $"Role '{CreateRoleName}' already exists.";
                await LoadRolesAsync();
                return Page();
            }

            var role = new IdentityRole(CreateRoleName.Trim());
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Role '{CreateRoleName}' created.";
                return RedirectToPage();
            }
            else
            {
                ErrorMessage = "Failed to create role: " + string.Join("; ", result.Errors.Select(e => e.Description));
                await LoadRolesAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync([FromForm] string RoleId, [FromForm] string NewRoleName)
        {
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                ErrorMessage = "Role id missing for edit.";
                await LoadRolesAsync();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(NewRoleName))
            {
                ErrorMessage = "New role name cannot be empty.";
                await LoadRolesAsync();
                return Page();
            }

            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                ErrorMessage = "Role not found.";
                await LoadRolesAsync();
                return Page();
            }

            var trimmed = NewRoleName.Trim();
            if (role.Name == trimmed)
            {
                StatusMessage = "No changes detected.";
                return RedirectToPage();
            }

            var already = await _roleManager.FindByNameAsync(trimmed);
            if (already != null && already.Id != role.Id)
            {
                ErrorMessage = $"Another role with name '{trimmed}' already exists.";
                await LoadRolesAsync();
                return Page();
            }

            role.Name = trimmed;
            role.NormalizedName = _roleManager.NormalizeKey(trimmed);

            var updateResult = await _roleManager.UpdateAsync(role);
            if (updateResult.Succeeded)
            {
                StatusMessage = $"Role updated to '{trimmed}'.";
                return RedirectToPage();
            }
            else
            {
                ErrorMessage = "Failed to update role: " + string.Join("; ", updateResult.Errors.Select(e => e.Description));
                await LoadRolesAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] string RoleId)
        {
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                ErrorMessage = "Role id missing for delete.";
                await LoadRolesAsync();
                return Page();
            }

            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                ErrorMessage = "Role not found.";
                await LoadRolesAsync();
                return Page();
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole != null && usersInRole.Count > 0)
            {
                ErrorMessage = $"Cannot delete role '{role.Name}' because it has {usersInRole.Count} user(s) assigned. Remove users from the role first.";
                await LoadRolesAsync();
                return Page();
            }

            var deleteResult = await _roleManager.DeleteAsync(role);
            if (deleteResult.Succeeded)
            {
                StatusMessage = $"Role '{role.Name}' deleted.";
                return RedirectToPage();
            }
            else
            {
                ErrorMessage = "Failed to delete role: " + string.Join("; ", deleteResult.Errors.Select(e => e.Description));
                await LoadRolesAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResetDefaultsAsync()
        {
            var messages = new List<string>();
            var errors = new List<string>();

            foreach (var defaultRole in DefaultRoles)
            {
                try
                {
                    if (!await _roleManager.RoleExistsAsync(defaultRole))
                    {
                        var createResult = await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                        if (createResult.Succeeded)
                        {
                            messages.Add($"Created role '{defaultRole}'.");
                        }
                        else
                        {
                            errors.Add($"Failed to create '{defaultRole}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        messages.Add($"Role '{defaultRole}' already exists.");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error creating '{defaultRole}': {ex.Message}");
                }
            }

            var allRoles = _roleManager.Roles.ToList();
            foreach (var role in allRoles)
            {
                if (DefaultRoles.Contains(role.Name, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                    if (usersInRole == null || usersInRole.Count == 0)
                    {
                        var del = await _roleManager.DeleteAsync(role);
                        if (del.Succeeded)
                        {
                            messages.Add($"Deleted extraneous role '{role.Name}'.");
                        }
                        else
                        {
                            errors.Add($"Failed to delete '{role.Name}': {string.Join(", ", del.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        errors.Add($"Did not delete '{role.Name}' because it has {usersInRole.Count} user(s).");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error processing '{role.Name}': {ex.Message}");
                }
            }

            StatusMessage = string.Join(" ", messages);
            if (errors.Any())
            {
                ErrorMessage = string.Join(" ", errors);
            }

            return RedirectToPage();
        }
    }
}