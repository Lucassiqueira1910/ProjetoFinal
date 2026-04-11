namespace USUARIOminimalSolution.Application.DTOs
{
    public class UsuarioDto
    {
        public int Id_Usuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Para os links HATEOAS do item
        public Dictionary<string, string>? Links { get; set; }
    }
}