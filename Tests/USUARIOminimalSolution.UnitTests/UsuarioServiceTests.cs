using Xunit;
using Moq;
using System.Threading.Tasks;
using USUARIOminimalSolution.Application.DTOs;
using USUARIOminimalSolution.Application.Services;
using USUARIOminimalSolution.Domain.Entities;

using USUARIOminimalSolution.Infrastructure.Repositories;

public class UsuarioServiceTests
{
private Usuario CriarUsuarioValido()
{
return new Usuario(
"Lucas Teste",
"[teste@email.com](mailto:teste@email.com)",
"123456"
);
}


[Fact]
public async Task GetByIdAsync_UsuarioExiste_DeveRetornarUsuario()
{
    // Arrange
    var usuario = CriarUsuarioValido();

    var mockRepo = new Mock<IUsuarioRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(usuario);

    var service = new UsuarioService(mockRepo.Object);

    // Act
    var result = await service.GetByIdAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(usuario.Nome, result.Nome);
}

[Fact]
public async Task UpdateAsync_UsuarioExiste_DeveAtualizar()
{
    // Arrange
    var usuario = CriarUsuarioValido();

    var mockRepo = new Mock<IUsuarioRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(usuario);

    mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

    var service = new UsuarioService(mockRepo.Object);

    // Act
    var request = new UpdateUsuarioRequest
    {
            Nome = "Novo Nome",
            Email = "novo@email.com",
            Senha = "123456"
    };

    await service.UpdateAsync(1, request); 

    // Assert
    mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
}

[Fact]
public async Task DeleteAsync_UsuarioExiste_DeveDeletar()
{
    // Arrange
    var usuario = CriarUsuarioValido();

    var mockRepo = new Mock<IUsuarioRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(usuario);

    mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

    var service = new UsuarioService(mockRepo.Object);

    // Act
    await service.DeleteAsync(1);

    // Assert
    mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once);
}


}
