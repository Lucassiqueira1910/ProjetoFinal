using System;
using System.Threading.Tasks;
using USUARIOminimalSolution.Domain.Entities;
using System.Collections.Generic;
using USUARIOminimalSolution.Application.DTOs;

namespace USUARIOminimalSolution.Infrastructure.Repositories
{
    public interface IUsuarioRepository
    {
        Task AddAsync(Usuario usuario);
        Task<Usuario> GetByIdAsync(int id);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<(IEnumerable<Usuario>, int total)> SearchAsync(UsuarioSearchQuery q);
    }
}
