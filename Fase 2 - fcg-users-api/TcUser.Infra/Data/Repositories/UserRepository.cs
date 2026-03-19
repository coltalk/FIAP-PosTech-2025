using Microsoft.EntityFrameworkCore;
using TcUser.Application.Interfaces;
using TcUser.Domain.Entities;
using TcUser.Infra.Data.Context.Sql.Context;

namespace TcUser.Infra.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly LojaGamesDbContext _db;

    public UserRepository(LojaGamesDbContext db)
        => _db = db;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }
}      
