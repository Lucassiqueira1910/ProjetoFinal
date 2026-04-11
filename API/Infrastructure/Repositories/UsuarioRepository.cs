using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using USUARIOminimalSolution.Domain.Entities;
using USUARIOminimalSolution.Application.DTOs;
using USUARIOminimalSolution.Infrastructure.Persistence;

namespace USUARIOminimalSolution.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _ctx;
        public UsuarioRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(Usuario usuario)
        {
            _ctx.Usuarios.Add(usuario);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id) =>
            await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Id_Usuario == id);

        public async Task UpdateAsync(Usuario usuario)
        {
            _ctx.Usuarios.Update(usuario);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Id_Usuario == id);
            if (e != null) { _ctx.Usuarios.Remove(e); await _ctx.SaveChangesAsync(); }
        }

        public async Task<(IEnumerable<Usuario>, int total)> SearchAsync(UsuarioSearchQuery q)
        {
            var query = _ctx.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Nome))
                query = query.Where(u => u.Nome.Contains(q.Nome));

            if (!string.IsNullOrWhiteSpace(q.Email))
                query = query.Where(u => u.Email.Contains(q.Email));

            var total = await query.CountAsync();

            // ordering
            if (q.SortBy?.ToLower() == "nome") query = q.Asc ? query.OrderBy(u => u.Nome) : query.OrderByDescending(u => u.Nome);
            else query = q.Asc ? query.OrderBy(u => u.Id_Usuario) : query.OrderByDescending(u => u.Id_Usuario);

            var items = await query
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
