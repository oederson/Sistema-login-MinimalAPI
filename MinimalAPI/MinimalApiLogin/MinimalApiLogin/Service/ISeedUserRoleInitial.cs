namespace MinimalApiLogin.Service;

public interface ISeedUserRoleInitial
{
    Task SeedRolesAsync();
    Task SeedUserAsync();
}
