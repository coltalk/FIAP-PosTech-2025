using Microsoft.EntityFrameworkCore;
using UsersApi.Domain.Entities;
using UsersAPI.Data.Repositories.Generic;
using UsersAPI.Domain.Interfaces.Repository;

namespace UsersAPI.Data.Repositories;

public class UsuarioPerfilRepository : GenericEntityRepository<UsuarioPerfil>, IUsuarioPerfilRepository
{
    public UsuarioPerfilRepository(UsersApiDbContext context) : base(context)
    {
    }

    public UsuarioPerfil? BuscarPorUsuarioId(Guid usuarioId)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(p => p.UsuarioId == usuarioId);
    }

    public UsuarioPerfil? BuscarPorIdEUsuario(Guid id, Guid usuarioId)
    {
        return _dbSet.FirstOrDefault(p => p.Id == id && p.UsuarioId == usuarioId);
    }
}
