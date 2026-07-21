using Microsoft.EntityFrameworkCore;
using UsersApi.Domain.Entities;
using UsersAPI.Data.Repositories.Generic;
using UsersAPI.Domain.Interfaces.Repository;

namespace UsersAPI.Data.Repositories;

public class EnderecoRepository : GenericEntityRepository<Endereco>, IEnderecoRepository
{
    public EnderecoRepository(UsersApiDbContext context) : base(context)
    {
    }

    public List<Endereco> ListarPorUsuario(Guid usuarioId)
    {
        return _dbSet
            .AsNoTracking()
            .Where(e => e.UsuarioId == usuarioId)
            .ToList();
    }

    public Endereco? BuscarPorIdEUsuario(Guid id, Guid usuarioId)
    {
        return _dbSet
            .FirstOrDefault(e => e.Id == id && e.UsuarioId == usuarioId);
    }
}
