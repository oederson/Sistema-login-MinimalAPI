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
using System.Text.Json;

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
    opt.SaveToken = false;
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
    //Desativa o cookie no cabeçalho de resposta 
    signInManager.Context.Response.Cookies.Delete(".AspNetCore.Identity.Application");
    return Results.Ok(TokenService.GenerateToken(usuariobd, await userManager.GetRolesAsync(usuariobd)));
});

app.MapGet("/usuarios", (
    ClaimsPrincipal userPrincipal,
    [FromServices] UserManager<Usuario> userManager) =>
{
    return Results.Ok(userManager.Users.ToList());
}).RequireAuthorization("Admin");

app.MapDelete("/usuario", async (
    ClaimsPrincipal usePrincipal,
    [FromBody] JsonElement requestBody,
    [FromServices] UserManager<Usuario> userManager) =>
{    
    if (requestBody.TryGetProperty("id", out var idProperty) && idProperty.ValueKind == JsonValueKind.String)
    {
        var id = idProperty.GetString();

        var usuario = await userManager.FindByIdAsync(id);

        if (usuario == null)
        {
            return Results.NotFound($"Usuário com ID {id} não encontrado.");
        }

        var result = await userManager.DeleteAsync(usuario);

        if (result.Succeeded)
        {
            return Results.Ok($"Usuário com ID {id} excluído com sucesso.");
        }
        else
        {
            return Results.BadRequest($"Falha ao excluir o usuário com ID {id}.");
        }
    }
    else
    {
        return Results.BadRequest("Corpo da solicitação inválido. Certifique-se de incluir uma propriedade 'id' válida.");
    }
}).RequireAuthorization("Admin");

app.MapPut("/atualizar-dados-usuario", async (
    ClaimsPrincipal userPrincipal,
    [FromServices] UserManager<Usuario> userManager,
    [FromBody] AtualizarUsuarioDTO dadosAtualizados) =>
{

    var userId = userPrincipal.FindFirstValue("Id");
    // Obtém o usuário do banco de dados
    var usuario = await userManager.FindByIdAsync(userId);

    if (usuario == null)
    {
        return Results.NotFound($"Usuário com ID {userId} não encontrado.");
    }

    // Atualiza as informações do usuário com base nos dados fornecidos no corpo da requisição
    usuario.UserName = dadosAtualizados.Username;

    // Salva as alterações no banco de dados
    var result = await userManager.UpdateAsync(usuario);

    if (result.Succeeded)
    {
        return Results.Ok($"Usuário com ID {userId} atualizado com sucesso.");
    }
    else
    {
        return Results.BadRequest($"Falha ao atualizar o usuário com ID {userId}.");
    }
}).RequireAuthorization("User");

app.MapPut("/alterar-senha", async (
    ClaimsPrincipal userPrincipal,
    [FromServices] UserManager<Usuario> userManager,
    [FromBody] AlterarSenhaDTO alterarSenhaDTO) =>
{
    var userId = userPrincipal.FindFirstValue("Id");
    var usuario = await userManager.FindByIdAsync(userId);

    if (usuario == null)
    {
        return Results.NotFound($"Usuário com ID {userId} não encontrado.");
    }

    // Verifique se a senha atual fornecida está correta
    var senhaAtualCorreta = await userManager.CheckPasswordAsync(usuario, alterarSenhaDTO.SenhaAtual);

    if (!senhaAtualCorreta)
    {
        return Results.BadRequest("Senha atual incorreta.");
    }

    // Altere a senha
    var resultadoAlteracao = await userManager.ChangePasswordAsync(usuario, alterarSenhaDTO.SenhaAtual, alterarSenhaDTO.NovaSenha);

    if (resultadoAlteracao.Succeeded)
    {
        return Results.Ok($"Senha do usuário com ID {userId} alterada com sucesso.");
    }
    else
    {
        return Results.BadRequest("Falha ao alterar a senha. Verifique os requisitos de senha.");
    }
}).RequireAuthorization("User");


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