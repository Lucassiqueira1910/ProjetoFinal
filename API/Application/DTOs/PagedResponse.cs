namespace USUARIOminimalSolution.Application.DTOs
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public List<LinkDto> Links { get; set; } = new();
    }

    public class LinkDto
    {
        public string Rel { get; set; }
        public string Href { get; set; }

        public LinkDto(string rel, string href)
        {
            Rel = rel;
            Href = href;
        }
    }
}