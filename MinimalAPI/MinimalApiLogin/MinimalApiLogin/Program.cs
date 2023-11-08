using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApiLogin.Data;
using MinimalApiLogin.Data.DTO;
using MinimalApiLogin.Models;
using MinimalApiLogin.Service;
using System.Text;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5173").AllowAnyHeader()
                                .AllowAnyMethod();                                
                      });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opts => { opts.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")); });
builder.Services.AddIdentity<Usuario, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SDASDASDFDSFDSFSDFSD2132314312312312")),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminUserRole",
        policy => policy.RequireRole("Admin", "User"));   
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);

await CriarPerfisUsuariosAsync(app);

app.MapPost("/registro",[AllowAnonymous] async (
    SignInManager<Usuario> signInManager,
    UserManager<Usuario> userManager,
    TokenService tokenService,
    CriarUsuarioDTO usuario,
    IMapper mapper) =>
{
    if (usuario == null)
        return Results.BadRequest("Usuario nao informado");
    Usuario user = mapper.Map<Usuario>(usuario);
    var result = await userManager.CreateAsync(user, usuario.Password);
    if (!result.Succeeded)
        return Results.BadRequest(result.Errors);
    var usuariobd = signInManager.UserManager.Users.FirstOrDefault(usuarioNoDb => usuarioNoDb.NormalizedUserName == usuario.Username);
    await userManager.AddToRoleAsync(usuariobd, "User");
    IList<string> rolee = await userManager.GetRolesAsync(usuariobd);
    return Results.Ok(tokenService.GenerateToken(usuariobd, rolee));

}).ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("RegistroUsuario")
.WithTags("Usuario");

app.MapPost("/login", [AllowAnonymous] async (
    SignInManager<Usuario> signInManager,

    UserManager<Usuario> userManager,
    TokenService tokenService,
    LoginUsuarioDTO usuario,
    IMapper mapper) =>
{
    if (usuario == null)
        return Results.BadRequest("Usuario nao informado");
    if(await signInManager.PasswordSignInAsync(usuario.Username, usuario.Password, false, false) != Microsoft.AspNetCore.Identity.SignInResult.Success)
        return Results.BadRequest("Deu merda");
    var usuariobd =  signInManager.UserManager.Users.FirstOrDefault(usuarioNoDb => usuarioNoDb.NormalizedUserName == usuario.Username);
    IList<string> rolee = await userManager.GetRolesAsync(usuariobd);    
    return Results.Ok(tokenService.GenerateToken(usuariobd, rolee));

}).ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("LoginUsuario")
.WithTags("Usuario");

app.MapGet("/acesso",[Authorize (Roles ="Admin")]() => { return Results.Ok(); } ).ProducesValidationProblem()
                                                                                 .Produces(StatusCodes.Status200OK)
                                                                                 .Produces(StatusCodes.Status403Forbidden)
                                                                                 .WithName("Acesso")
                                                                                 .WithTags("Usuario");

app.Run();

async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        await service.SeedRolesAsync();
        await service.SeedUserAsync();
    }
}

