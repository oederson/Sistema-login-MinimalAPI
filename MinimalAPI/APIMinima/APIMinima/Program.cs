using APIMinima;
using APIMinima.Data;
using APIMinima.Data.DTO;
using APIMinima.Models;
using APIMinima.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddDbContext<AppDbContext>(opts => { opts.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")); });
builder.Services.AddIdentity<Usuario, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer( opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(key),        
        ValidateIssuerSigningKey = false,
        ValidateAudience = false,
    };
});
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    opts.AddPolicy("User", policy => policy.RequireRole("User"));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
await CriarPerfisUsuariosAsync(app);

app.MapPost("/registro", async (
    [FromBody] CriarUsuarioDTO usuario,
    [FromServices] SignInManager<Usuario> signInManager,
    [FromServices] UserManager<Usuario> userManager,
    [FromServices] IMapper mapper) =>
{
    if (usuario == null)
        return Results.BadRequest("Usuario nao informado");
    if (await userManager.CreateAsync(mapper.Map<Usuario>(usuario), usuario.Password) != IdentityResult.Success)
        return Results.BadRequest("Usuario não pode ser cadastrado");
    var usuariobd = signInManager.UserManager.Users.FirstOrDefault(usuarioNoDb => usuarioNoDb.NormalizedUserName == usuario.Username);
    await userManager.AddToRoleAsync(usuariobd, "User");
    return Results.Ok(TokenService.GenerateToken(usuariobd, await userManager.GetRolesAsync(usuariobd)));
});

app.MapPost("/login", async (
    [FromBody] LoginUsuarioDTO usuario,
    [FromServices] SignInManager<Usuario> signInManager,
    [FromServices] UserManager<Usuario> userManager) =>
{   
    if (usuario == null)
        return Results.NotFound(new { message = "Invalid username or password" });
    if (await signInManager.PasswordSignInAsync(usuario.Username, usuario.Password, false, false) != Microsoft.AspNetCore.Identity.SignInResult.Success)
        return Results.BadRequest("Deu merda");
    var usuariobd = signInManager.UserManager.Users.FirstOrDefault(usuarioNoDb => usuarioNoDb.NormalizedUserName == usuario.Username);
    return Results.Ok(TokenService.GenerateToken(usuariobd, await userManager.GetRolesAsync(usuariobd)));
});

app.MapGet("/usuarios", (
    ClaimsPrincipal userPrincipal,
    [FromServices] UserManager<Usuario> userManager) =>
{
    return Results.Ok(userManager.Users.ToList());
}).RequireAuthorization("Admin");

app.Run();

static async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        await service.SeedRolesAsync();
        await service.SeedUserAsync();
    }
}