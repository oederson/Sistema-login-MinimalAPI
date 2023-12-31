﻿using APIMinima.Data.DTO;
using APIMinima.Models;
using APIMinima.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace APIMinima.Endpoints;

public static class Endpoints
{
    public static void Map(WebApplication app) 
    {
        app.MapPost("/registro", async (
        [FromBody] CriarUsuarioDTO usuario,
        [FromServices] SignInManager<Usuario> signInManager,
        [FromServices] UserManager<Usuario> userManager,
        [FromServices] IMapper _mapper) =>{
            try{ 
            if (usuario.Username == "" || usuario.Password == "" || usuario.RePassword == "")
                return Results.BadRequest("Usuario nao informado");
            if (await userManager.CreateAsync(_mapper.Map<Usuario>(usuario), usuario.Password) != IdentityResult.Success)
                return Results.BadRequest("Usuario não pode ser cadastrado");
            Usuario? usuariobd = await userManager.FindByNameAsync(usuario.Username);
            _ = await userManager.AddToRoleAsync(usuariobd, "User");
            return Results.Ok(TokenService.GenerateToken(usuariobd, await userManager.GetRolesAsync(usuariobd)));
            }catch (Exception ex) {  
                return Results.BadRequest($"Erro inesperado no servidor : {ex}");}
        });

        app.MapPost("/login", async (
        [FromBody] LoginUsuarioDTO usuario,
        [FromServices] SignInManager<Usuario> signInManager,
        [FromServices] UserManager<Usuario> userManager) =>
        {
            try{
            if (usuario == null)
                return Results.NotFound(new { message = "username ou password invalidos" });
            var user = await userManager.FindByNameAsync(usuario.Username);
            if (!await userManager.CheckPasswordAsync(user, usuario.Password))
                return Results.BadRequest("Deu merda");
            return Results.Ok(TokenService.GenerateToken(user, await userManager.GetRolesAsync(user)));
            }
            catch (Exception ex){
                return Results.BadRequest($"Erro inesperado no servidor : {ex}");}
        });

        app.MapGet("/usuarios", [Authorize] (
        ClaimsPrincipal userPrincipal,
        [FromServices] UserManager<Usuario> userManager) =>
        {
            try
            {
                return Results.Ok(userManager.Users.ToList());
            }catch (Exception ex) {
                return Results.BadRequest($"Erro inesperado no servidor : {ex}");}
    }).RequireAuthorization("Admin");

        app.MapDelete("/usuario", [Authorize] async (
        ClaimsPrincipal usePrincipal,
        [FromBody] JsonElement requestBody,
        [FromServices] UserManager<Usuario> userManager) =>
        {
            try { 
            if (requestBody.TryGetProperty("id", out var idProperty) && idProperty.ValueKind == JsonValueKind.String)
            {
                var usuario = await userManager.FindByIdAsync(idProperty.GetString());
                if (usuario == null)
                    return Results.NotFound($"Usuário  não encontrado.");
                if (await userManager.DeleteAsync(usuario) == IdentityResult.Success)
                    return Results.Ok($"Usuário  excluído com sucesso.");
                else
                    return Results.BadRequest($"Falha ao excluir o usuário.");
            }
            else
                return Results.BadRequest("Corpo da solicitação inválido. Certifique-se de incluir uma propriedade 'id' válida.");
        }catch (Exception ex) {
            return Results.BadRequest($"Erro inesperado no servidor : {ex}");}
    }).RequireAuthorization("Admin");

        app.MapPut("/atualizar-dados-usuario", [Authorize] async (
        ClaimsPrincipal userPrincipal,
        [FromServices] UserManager<Usuario> userManager,
        [FromBody] AtualizarUsuarioDTO dadosParaAtualizar) =>
        {
         try { 
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
          }catch (Exception ex) {
            return Results.BadRequest($"Erro inesperado no servidor : {ex}");}
    }).RequireAuthorization("User");

        app.MapPut("/alterar-senha", [Authorize] async (
        ClaimsPrincipal userPrincipal,
        [FromServices] UserManager<Usuario> userManager,
        [FromBody] AlterarSenhaDTO alterarSenhaDTO) =>
        {
         try { 
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
         }catch (Exception ex) {
            return Results.BadRequest($"Erro inesperado no servidor : {ex}");}
    }).RequireAuthorization("User");
    }
}
