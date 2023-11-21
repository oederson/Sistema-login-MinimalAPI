using APIMinima;
using APIMinima.Data;
using APIMinima.Data.DTO;
using APIMinima.Models;
using APIMinima.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddDbContext<AppDbContext>(opts => {opts.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao"));});
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
        IssuerSigningKey = new SymmetricSecurityKey(key),        
        ValidateIssuerSigningKey = false,
        ValidateAudience = true,
        ValidAudience = "http://127.0.0.1:5173/"
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
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("pt-BR");
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = new List<CultureInfo> { new("pt-BR") };
    options.SupportedUICultures = new List<CultureInfo> { new("pt-BR") };
});

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
    var usuariobd = await userManager.FindByNameAsync(usuario.Username); 
    await userManager.AddToRoleAsync(usuariobd, "User");
    return Results.Ok(TokenService.GenerateToken(usuariobd, await userManager.GetRolesAsync(usuariobd)));
});

app.MapPost("/login", async (
    [FromBody] LoginUsuarioDTO usuario,
    [FromServices] SignInManager<Usuario> signInManager,
    [FromServices] UserManager<Usuario> userManager) =>
{
    
    if (usuario == null)
        return Results.NotFound(new { message = "username ou password invalidos" });
    var user = await userManager.FindByNameAsync(usuario.Username);
    if (!await userManager.CheckPasswordAsync(user, usuario.Password))
        return Results.BadRequest("Deu merda");
    return Results.Ok(TokenService.GenerateToken(user, await userManager.GetRolesAsync(user)));
});

app.MapGet("/usuarios",[Authorize] (
    ClaimsPrincipal userPrincipal,
    [FromServices] UserManager<Usuario> userManager) =>
{
    return Results.Ok(userManager.Users.ToList());
}).RequireAuthorization("Admin");

app.MapDelete("/usuario", [Authorize] async (
    ClaimsPrincipal usePrincipal,
    [FromBody] JsonElement requestBody,
    [FromServices] UserManager<Usuario> userManager) =>
{    
    if (requestBody.TryGetProperty("id", out var idProperty) && idProperty.ValueKind == JsonValueKind.String)
    {
        var usuario = await userManager.FindByIdAsync(idProperty.GetString());
        if (usuario == null)
            return Results.NotFound($"Usuário  não encontrado.");      
        if (await userManager.DeleteAsync(usuario) == IdentityResult.Success ) 
            return Results.Ok($"Usuário  excluído com sucesso.");
        else
            return Results.BadRequest($"Falha ao excluir o usuário.");
    }
    else
    return Results.BadRequest("Corpo da solicitação inválido. Certifique-se de incluir uma propriedade 'id' válida.");    
}).RequireAuthorization("Admin");

app.MapPut("/atualizar-dados-usuario", [Authorize] async (
    ClaimsPrincipal userPrincipal,
    [FromServices] UserManager<Usuario> userManager,
    [FromBody] AtualizarUsuarioDTO dadosParaAtualizar) =>
{
    if (dadosParaAtualizar.Username.IsNullOrEmpty())
        return Results.BadRequest($"Falha ao atualizar o usuário.Dados nulos ou vazios!");
    else 
    {
        var user = await userManager.FindByIdAsync(userPrincipal.FindFirstValue("Id"));
        if (user != null) 
        {
            user.UserName = dadosParaAtualizar.Username;
            if (await userManager.UpdateAsync(user) == IdentityResult.Success)
                return Results.Ok($"Usuário  atualizado com sucesso.");
        }
        return Results.NotFound($"Usuário  não encontrado.");
    }  
}).RequireAuthorization("User");

app.MapPut("/alterar-senha", [Authorize] async (
    ClaimsPrincipal userPrincipal,
    [FromServices] UserManager<Usuario> userManager,
    [FromBody] AlterarSenhaDTO alterarSenhaDTO) =>
{
    var usuario = await userManager.FindByIdAsync(userPrincipal.FindFirstValue("Id"));
    if (usuario == null)
        return Results.NotFound($"Usuário não encontrado.");
    var senhaAtualCorreta = await userManager.CheckPasswordAsync(usuario, alterarSenhaDTO.SenhaAtual);
    if (!senhaAtualCorreta)
        return Results.BadRequest("Senha atual incorreta.");
    var resultadoAlteracao = await userManager.ChangePasswordAsync(usuario, alterarSenhaDTO.SenhaAtual, alterarSenhaDTO.NovaSenha);
    if (resultadoAlteracao.Succeeded)
        return Results.Ok($"Senha do usuário  alterada com sucesso.");
    else
        return Results.BadRequest("Falha ao alterar a senha. Verifique os requisitos de senha.");
}).RequireAuthorization("User");

app.Run();

static async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using var scope = scopedFactory.CreateScope();
    var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
    await service.SeedRolesAsync();
    await service.SeedUserAsync();
}