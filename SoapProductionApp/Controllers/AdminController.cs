using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoapProductionApp.Extensions;
using SoapProductionApp.Models;
using SoapProductionApp.Models.Warehouse;
using SoapProductionApp.Services;

namespace SoapProductionApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditService _auditService;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IAuditService auditService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userList);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles,
                AllRoles = allRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var userRolesBeforeJson = userRoles.ToSafeJson();
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            var userRolesAfterJson = model.SelectedRoles.ToSafeJson();
            await _auditService.LogAsync("Update", "User", null, userRolesBeforeJson, userRolesAfterJson);

            return RedirectToAction(nameof(Index));
        }
    }
}
