using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApiLogin.Data;
using MinimalApiLogin.Data.DTO;
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

//app.UseAuthorization();

app.MapPost("/registro", async (
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
    var tokenJwt = tokenService.GenerateToken(user);
    return Results.Ok(tokenJwt);

}).ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("RegistroUsuario")
.WithTags("Usuario");

app.MapPost("/login", async (
    SignInManager<Usuario> signInManager,
    UserManager<Usuario> userManager,
    TokenService tokenService,
    LoginUsuarioDTO usuario,
    IMapper mapper) =>
{
    if (usuario == null)
        return Results.BadRequest("Usuario nao informado");
    Usuario user = mapper.Map<Usuario>(usuario);
    var result = await signInManager.PasswordSignInAsync(usuario.Username, usuario.Password, false, false);
    if (!result.Succeeded)
        return Results.BadRequest(result);
    var tokenJwt = tokenService.GenerateToken(user);
    return Results.Ok(tokenJwt);

}).ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("LoginUsuario")
.WithTags("Usuario");

app.Run();


