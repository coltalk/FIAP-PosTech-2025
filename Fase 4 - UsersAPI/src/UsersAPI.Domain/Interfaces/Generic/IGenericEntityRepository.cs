using System.Linq.Expressions;

namespace UsersAPI.Domain.Interfaces.Generic;

public interface IGenericEntityRepository<T>
{
    #region Methods

    bool Exists(Expression<Func<T, bool>> predicate);

    void Delete(T entity);

    Task<bool> DeleteById(Guid id);

    Task<T> Insert(T entity, CancellationToken cancellationToken = default);

    (T entity, bool success) Update(T entity);

    IQueryable<T> Get();

    T GetById(Guid id);

    T GetByIdInt(int id);

    List<T> GetContainsId(Expression<Func<T, bool>> predicate);

    int LastId(Expression<Func<T, int>> predicate);

    Task<List<T>> ListarPaginacao(int take, int skip);

    #endregion

    #region GraphQL

    Task<List<T>> BuscarPorIdsAsync<TKey>(
        IEnumerable<TKey> ids,
        Expression<Func<T, TKey>> keySelector);

    #endregion
}
