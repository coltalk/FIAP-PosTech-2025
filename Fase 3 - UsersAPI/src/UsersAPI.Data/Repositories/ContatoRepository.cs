using Microsoft.EntityFrameworkCore;
using UsersApi.Domain.Entities;
using UsersAPI.Data.Repositories.Generic;
using UsersAPI.Domain.Interfaces.Repository;

namespace UsersAPI.Data.Repositories;

public class ContatoRepository : GenericEntityRepository<Contato>, IContatoRepository
{
    public ContatoRepository(UsersApiDbContext context) : base(context)
    {
    }

    public List<Contato> ListarPorUsuario(Guid usuarioId)
    {
        return _dbSet
            .AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId)
            .ToList();
    }

    public Contato? BuscarPorIdEUsuario(Guid id, Guid usuarioId)
    {
        return _dbSet
            .FirstOrDefault(c => c.Id == id && c.UsuarioId == usuarioId);
    }
}
