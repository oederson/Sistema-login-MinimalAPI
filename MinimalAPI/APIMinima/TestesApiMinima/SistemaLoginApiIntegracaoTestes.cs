using APIMinima.Data.DTO;
using APIMinima.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        var token = await PegaToken("Admin", "Senha1234@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        // Act
        var response = await _client.GetAsync("/usuarios");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task NaoDeveRetornarListaDeUsuariosParaUserPadrao()
    {
        //Arrange
        var token = await PegaToken("UserPadrao", "Senha1234@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
        var token = await PegaToken("Admin", "Senha1234@");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var idUser = await PegaId();

        // Criação da mensagem de requisição com o corpo
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/usuario")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new UserDelete { Id = idUser }), Encoding.UTF8, "application/json")
        };

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private async Task<string> PegaToken(string username, string password)
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

    private async Task<string> PegaId()
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
        public string Token { get; set; }
        public string Role { get; set; }
    }
    public class UserDelete 
    {
        public string Id { get; set; }
    }
}
