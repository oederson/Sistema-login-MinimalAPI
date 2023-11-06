using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalApiLogin.Data;
using MinimalApiLogin.Models;
using MinimalApiLogin.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

//app.UseAuthorization();

app.MapPost("/registro", async (
    SignInManager<Usuario> signInManager,
    UserManager<Usuario> userManager,
    IOptions<TokenService> appJwtSettings,
    Usuario usuario) =>
{
    if (usuario == null)
        return Results.BadRequest("Usuario nao informado");
    var user = new Usuario
    { UserName = usuario.UserName,
        Email = usuario.Email  
    };
    var result = await userManager.CreateAsync(user, usuario.PasswordHash);
    if (!result.Succeeded)
        return Results.BadRequest(result.Errors);
    return Results.Ok();

}).ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("RegistroUsuario")
.WithTags("Usuario");

app.Run();


