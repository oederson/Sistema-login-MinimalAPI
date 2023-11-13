namespace APIMinima.Service;

public interface ISeedUserRoleInitial
{
    Task SeedRolesAsync();
    Task SeedUserAsync();
}
