using APIMinima.Data.DTO;
using APIMinima.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace TestesApiMinima;

[TestFixture, NonParallelizable]
public class SistemaLoginApiIntegracaoTestes
{
    private SistemaLoginApiApplication _application;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _application = new SistemaLoginApiApplication();
        _client = _application.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {        
        _client.Dispose();
    }

    [Test]
    public async Task LoginComSucesso()
    {
        //Arrange
        var user = new LoginUsuarioDTO { Username = "UserPadrao", Password = "Senha1234@" };
        //Act
        var result = await _client.PostAsJsonAsync("/login", user);
        //Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task LoginComSenhaErradaDeveFalhar()
    {
        //Arrange       
        var user = new LoginUsuarioDTO { Username = "UserPadrao", Password = "---------" };
        //Act
        var result = await _client.PostAsJsonAsync("/login", user);
        //Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RegistroNaoDeveCadastrarUsuario()
    {       
        //Arrange        
        var user = new CriarUsuarioDTO{ Username = "", Password ="", RePassword="" };
        //Act
        var res = await _client.PostAsJsonAsync("/registro", user);
        //Assert
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    [Test]
    public async Task RegistroNaoDeveCadastrarUsuarioComSenhaInvalida()
    {
        //Arrange        
        var user = new CriarUsuarioDTO { Username = "Fulano", Password = "senhainvalida", RePassword = "senhainvalida" };
        //Act
        var res = await _client.PostAsJsonAsync("/registro", user);
        //Assert
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task RegistroDeveCadastrarUsuarioComSucesso()
    {
        //Arrange
        var user = new CriarUsuarioDTO { Username = "Aleatorio", Password = "Padrao123-", RePassword = "Padrao123-" };
        //Act
        var res = await _client.PostAsJsonAsync("/registro", user);
        //Assert
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));

    }
    [Test]
    public async Task ListaDeUsuariosSomenteParaAdmin()
    {
        //Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("Admin", "Senha1234@"));
        // Act
        var response = await _client.GetAsync("/usuarios");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task NaoDeveRetornarListaDeUsuariosParaUserPadrao()
    {
        //Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("UserPadrao", "Senha1234@"));
        // Act
        var response = await _client.GetAsync("/usuarios");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        var qkq =await  PegaId();
        await Console.Out.WriteLineAsync(qkq);
    }
    [Test]
    public async Task DeletarUsuarioComSucesso()
    {
        // Arrange        
        var idUser = await PegaId();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("Admin", "Senha1234@"));
        // Cria mensagem de requisição com o corpo
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/usuario")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new UserDelete { id = idUser }), Encoding.UTF8, "application/json")
        };
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    [Test]
    public async Task DeletarUsuarioNaoDeveDeletarQuandoARoleForUser()
    {
        // Arrange
        var idUser = await PegaId();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("UserPadrao", "Senha1234@"));
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/usuario")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new UserDelete { id = idUser }), Encoding.UTF8, "application/json")
        };
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
    [Test]
    public async Task DeletarUsuarioNaoDeveDeletarSeReceberIdInexistente()
    {
        // Arrange
        var idUser = "23sda1234s";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("Admin", "Senha1234@"));
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/usuario")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new UserDelete { id = idUser }), Encoding.UTF8, "application/json")
        };
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task AtualizarUsernameDoUsuarioComSucesso()
    {
        //Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("UserPadrao", "Senha1234@"));
        AtualizarUsuarioDTO att = new() { Username = "Atualizado"};
        //Act
        var res = await _client.PutAsJsonAsync("/atualizar-dados-usuario", att);
        //Assert 
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    [Test]
    public async Task AtualizarUsernameDoUsuarioDeveFalharComDadosVazios()
    {
        //Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("UserPadrao", "Senha1234@"));
        AtualizarUsuarioDTO att = new() { Username = "" };
        //Act
        var res = await _client.PutAsJsonAsync("/atualizar-dados-usuario", att);
        //Assert 
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    [Test]
    public async Task AtualizarUsernameDoUsuarioDeveFalharSeUsuarioForInvalido()
    {
        //Arrange
        var tokenUserInvalido = await PegaToken("UserPadrao", "Senha1234@");
        AtualizarUsuarioDTO att = new() { Username = "Aleatorio" };
        var idUser = await PegaId();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("Admin", "Senha1234@"));
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/usuario")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new UserDelete { id = idUser }), Encoding.UTF8, "application/json")
        };
        _ = await _client.SendAsync(request);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUserInvalido);
        //Act
        var res = await _client.PutAsJsonAsync("/atualizar-dados-usuario", att);
        //Assert 
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task AlterarSenhaDoUsuarioDeveFalharSeUsuarioForInvalido()
    {
        //Arrange
        var tokenUserInvalido = await PegaToken("UserPadrao", "Senha1234@");
        AlterarSenhaDTO senhaParaAtualizar = new() { SenhaAtual = "Senha1234@", NovaSenha = "NovaSenha123@" };
        var idUser = await PegaId();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await PegaToken("Admin", "Senha1234@"));
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/usuario")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new UserDelete { id = idUser }), Encoding.UTF8, "application/json")
        };
        _ = await _client.SendAsync(request);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUserInvalido);
        //Act
        var res = await _client.PutAsJsonAsync("/alterar-senha", senhaParaAtualizar);
        //Assert 
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    [Test]
    public async Task AlterarSenhaDoUsuarioDeveFalharSeASenhaAtualForInvalida()
    {
        //Arrange
        AlterarSenhaDTO senhaParaAtualizar = new() { SenhaAtual = "SenhaErrada", NovaSenha = "NovaSenha123@" };
        var token = await PegaToken("UserPadrao", "Senha1234@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //Act
        var res = await _client.PutAsJsonAsync("/alterar-senha", senhaParaAtualizar);
        //Assert 
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    [Test]
    public async Task AlterarSenhaDoUsuarioComSucesso()
    {
        //Arrange
        AlterarSenhaDTO senhaParaAtualizar = new() { SenhaAtual = "Senha1234@", NovaSenha = "NovaSenha123@" };
        var token = await PegaToken("UserPadrao", "Senha1234@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //Act
        var res = await _client.PutAsJsonAsync("/alterar-senha", senhaParaAtualizar);
        //Assert 
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    [Test]
    public async Task AlterarSenhaDoUsuarioDeveFalharSeANovaSenhaForInvalida()
    {
        //Arrange
        AlterarSenhaDTO senhaParaAtualizar = new() { SenhaAtual = "SenhaErrada", NovaSenha = "Novasenha" };
        var token = await PegaToken("UserPadrao", "Senha1234@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //Act
        var res = await _client.PutAsJsonAsync("/alterar-senha", senhaParaAtualizar);
        //Assert 
        Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    private async Task<string?> PegaToken(string username, string password)
    {
        var loginInfo = new { Username = username, Password = password };
        var response = await _client.PostAsJsonAsync("/login", loginInfo);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return responseData?.Token;            
        }
        return null;
    }

    private async Task<string?> PegaId()
    {
        var token = await PegaToken("Admin", "Senha1234@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/usuarios");

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadFromJsonAsync<List<Usuario>>();
            var userPadrao = responseData?.FirstOrDefault(u => u.UserName == "UserPadrao");

            if (userPadrao != null)
            {
                return userPadrao.Id;
            }
        }
        return null;
    }
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required string Role { get; set; }
    }
    public class UserDelete 
    {
        public required string id { get; set; }
    }
}