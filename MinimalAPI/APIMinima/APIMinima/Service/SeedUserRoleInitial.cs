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
            IdentityRole role = new IdentityRole();
            role.Name = "Admin";
            role.NormalizedName = "ADMIN";
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            var roleResult = await _roleManager.CreateAsync(role);
            
        }
    }
    public async Task SeedUserAsync()
    {
        if (await _userManager.FindByNameAsync("Admin") == null)
        {
            Usuario user = new Usuario();
            user.UserName = "Admin";
            user.Email = "emailadmin@emailadmin.com";

            IdentityResult result = await _userManager.CreateAsync(user, "Senha1234@");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                
            }
        }
    }
}
