using Microsoft.AspNetCore.Identity;
using APIMinima.Models;

namespace APIMinima.Service;

public class SeedUserRoleInitial : ISeedUserRoleInitial
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedUserRoleInitial(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task SeedRolesAsync()
    {

        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            IdentityRole role = new()
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            _ = await _roleManager.CreateAsync(role);
        }
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            IdentityRole role = new()
            {
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            _ = await _roleManager.CreateAsync(role);

        }
    }
    public async Task SeedUserAsync()
    {
        if (await _userManager.FindByNameAsync("Admin") == null)
        {
            Usuario user = new()
            {
                UserName = "Admin",
                Email = "emailadmin@emailadmin.com"
            };

            IdentityResult result = await _userManager.CreateAsync(user, "Senha1234@");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                
            }
        }
        if (await _userManager.FindByNameAsync("UserPadrao") == null)
        {
            Usuario user = new()
            {
                UserName = "UserPadrao",
                Email = "emailuser@emailuser.com"
            };

            IdentityResult result = await _userManager.CreateAsync(user, "Senha1234@");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

            }
        }
    }
}
