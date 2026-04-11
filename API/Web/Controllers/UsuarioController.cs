using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using USUARIOminimalSolution.Application.DTOs;
using USUARIOminimalSolution.Application.Services;

namespace USUARIOminimalSolution.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(UsuarioService service, ILogger<UsuarioController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUsuarioRequest req)
        {
            _logger.LogInformation("Criando usuário com nome {Nome} e email {Email}", req.Nome, req.Email);

            var dto = await _service.CreateAsync(req);

            _logger.LogInformation("Usuário criado com sucesso. Id: {Id}", dto.Id_Usuario);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id_Usuario }, dto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Buscando usuário com Id {Id}", id);

            var dto = await _service.GetByIdAsync(id);

            if (dto == null)
            {
                _logger.LogWarning("Usuário não encontrado: {Id}", id);
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioRequest req)
        {
            _logger.LogInformation("Atualizando usuário {Id}", id);

            var ok = await _service.UpdateAsync(id, req);

            if (!ok)
            {
                _logger.LogWarning("Falha ao atualizar. Usuário não encontrado: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Usuário atualizado com sucesso: {Id}", id);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deletando usuário {Id}", id);

            var ok = await _service.DeleteAsync(id);

            if (!ok)
            {
                _logger.LogWarning("Falha ao deletar. Usuário não encontrado: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Usuário deletado com sucesso: {Id}", id);

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? nome,
            [FromQuery] string? email,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool asc = false)
        {
            _logger.LogInformation("Buscando usuários. Nome: {Nome}, Email: {Email}, Page: {Page}", nome, email, page);

            var query = new UsuarioSearchQuery
            {
                Nome = nome ?? string.Empty,
                Email = email ?? string.Empty,
                Page = page <= 0 ? 1 : page,
                PageSize = pageSize <= 0 ? 10 : pageSize,
                SortBy = sortBy ?? "Id_Usuario",
                Asc = asc
            };

            var result = await _service.SearchAsync(query);

            // HATEOAS
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/Usuario/search";

            result.Links.Add(new LinkDto("self", $"{baseUrl}?nome={nome}&email={email}&page={query.Page}&pageSize={query.PageSize}&sortBy={query.SortBy}&asc={query.Asc}"));
            result.Links.Add(new LinkDto("first", $"{baseUrl}?nome={nome}&email={email}&page=1&pageSize={query.PageSize}&sortBy={query.SortBy}&asc={query.Asc}"));
            result.Links.Add(new LinkDto("last", $"{baseUrl}?nome={nome}&email={email}&page={result.TotalPages}&pageSize={query.PageSize}&sortBy={query.SortBy}&asc={query.Asc}"));

            if (query.Page > 1)
                result.Links.Add(new LinkDto("prev", $"{baseUrl}?nome={nome}&email={email}&page={query.Page - 1}&pageSize={query.PageSize}&sortBy={query.SortBy}&asc={query.Asc}"));

            if (query.Page < result.TotalPages)
                result.Links.Add(new LinkDto("next", $"{baseUrl}?nome={nome}&email={email}&page={query.Page + 1}&pageSize={query.PageSize}&sortBy={query.SortBy}&asc={query.Asc}"));

            // 🔥 LOG CORRIGIDO (SEM TotalCount)
            _logger.LogInformation("Busca realizada com sucesso.");

            return Ok(result);
        }
    }
}