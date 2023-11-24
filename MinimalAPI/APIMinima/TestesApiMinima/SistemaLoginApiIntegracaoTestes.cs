using APIMinima.Data.DTO;
using System.Net;
using System.Net.Http.Json;

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
        _application.Dispose();
        _client.Dispose();
    }

    [Test]
    public async Task LoginComSucesso()
    {
        //Arrange
        var user = new LoginUsuarioDTO { Username = "Admin", Password = "Senha1234@" };
        //Act
        var result = await _client.PostAsJsonAsync("/login", user);
        //Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK)); 
    }

    [Test]
    public async Task LoginComSenhaErradaDeveFalhar()
    {
        //Arrange       
        var user = new LoginUsuarioDTO { Username = "Admin", Password = "---------" };

        //Act
        var result = await _client.PostAsJsonAsync("/login", user);
        //Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
