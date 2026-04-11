namespace USUARIOminimalSolution.Application.DTOs
{
    public class UsuarioSearchQuery
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public bool Asc { get; set; } = false;
    }
}
