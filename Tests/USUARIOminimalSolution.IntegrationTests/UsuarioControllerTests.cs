using Xunit;
using System.Net;
using System.Net.Http.Json;

public class UsuarioControllerTests : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client;

    public UsuarioControllerTests(ApiFixture factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsuario_Inexistente_DeveRetornar404()
    {
        // Act
        var response = await _client.GetAsync("/api/usuario/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostUsuario_Valido_DeveRetornarCreated()
    {
        // Arrange
        var request = new
        {
            Nome = "Teste",
            Email = "teste@email.com",
            Senha = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/usuario", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}