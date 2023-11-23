using APIMinima.Data.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;


namespace EndpointsTestes
{
    public class EndpointsTestes : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EndpointsTestes(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        [Fact]
        public async Task Login_UsuarioExistenteComCredenciaisValidas_DeveRetornarToken()
        {
            // Arrange
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7023");
            var usuarioDTO = new
            {
                Username = "Admin",
                Password = "Senha1234@"
            };

            // Act
            var response = await client.PostAsJsonAsync("login", usuarioDTO);

            // Assert
            response.EnsureSuccessStatusCode();
            var token = response.Content.ReadAsStringAsync().Result;
            Assert.NotNull(token);
        }
    }
}
