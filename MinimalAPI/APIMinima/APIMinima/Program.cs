using APIMinima.Data;
using APIMinima.Endpoints;
using APIMinima.Models;
using APIMinima.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opts => { opts.UseSqlServer(builder.Configuration["ConnectionStrings:ConexaoPadrao"]);});
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
    opt.SaveToken = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["SymmetricSecurityKey"])),        
        ValidateIssuerSigningKey = false,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["CorsOrigin"]
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
                      policy.WithOrigins(builder.Configuration["CorsOrigin"])
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("pt-BR");
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = new List<CultureInfo> { new("pt-BR") };
    options.SupportedUICultures = new List<CultureInfo> { new("pt-BR") };
});
TokenService.Initialize(builder.Configuration);
var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    DateTime dateTimeBrasilia = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    context.Response.Headers.Add("Date", dateTimeBrasilia.ToString("G"));
    await next();
});
app.UseRequestLocalization(new RequestLocalizationOptions
{    
    ApplyCurrentCultureToResponseHeaders = true
});
await CriarPerfisUsuariosAsync(app);
Endpoints.Map(app);
app.Run();

static async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using var scope = scopedFactory.CreateScope();
    var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
    await service.SeedRolesAsync();
    await service.SeedUserAsync();
}
public partial class Program { }