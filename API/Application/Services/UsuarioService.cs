using System.Linq;
using System.Threading.Tasks;
using USUARIOminimalSolution.Domain.Entities;

using USUARIOminimalSolution.Application.DTOs;
using USUARIOminimalSolution.Infrastructure.Repositories;

namespace USUARIOminimalSolution.Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // --------------------------------------------------------------
        // CREATE
        // --------------------------------------------------------------
        public async Task<UsuarioDto> CreateAsync(CreateUsuarioRequest request)
        {
            
            var usuario = new Usuario(
                request.Nome,
                request.Email,
                request.Senha
            );

            await _usuarioRepository.AddAsync(usuario);

            return new UsuarioDto
            {
                Id_Usuario = usuario.Id_Usuario,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }

        // --------------------------------------------------------------
        // GET BY ID
        // --------------------------------------------------------------
        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return null;

            return new UsuarioDto
            {
                Id_Usuario = usuario.Id_Usuario,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }

        // --------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------
        public async Task<bool> UpdateAsync(int id, UpdateUsuarioRequest request)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return false;

            // Usa métodos de domínio (setters privados)
            usuario.SetNome(request.Nome);
            usuario.SetEmail(request.Email);
            usuario.SetSenha(request.Senha);

            await _usuarioRepository.UpdateAsync(usuario);
            return true;
        }

        // --------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return false;

            await _usuarioRepository.DeleteAsync(id);
            return true;
        }

        // --------------------------------------------------------------
        // SEARCH 
        // --------------------------------------------------------------
        public async Task<PagedResponse<UsuarioDto>> SearchAsync(UsuarioSearchQuery query)
        {
            var (items, total) = await _usuarioRepository.SearchAsync(query);

            var dtoItems = items.Select(u => new UsuarioDto
            {
                Id_Usuario = u.Id_Usuario,
                Nome = u.Nome,
                Email = u.Email,
                Links = new Dictionary<string, string>
                {
                    { "get", $"/api/Usuario/{u.Id_Usuario}" },
                    { "put", $"/api/Usuario/{u.Id_Usuario}" },
                    { "delete", $"/api/Usuario/{u.Id_Usuario}" }
                }
            }).ToList();

            return new PagedResponse<UsuarioDto>
            {
                Items = dtoItems,
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
            };
        }
    }
}
