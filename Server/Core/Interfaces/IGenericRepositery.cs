using Core.Models;

namespace Core.Interfaces;

public interface IGenericRepositery<T>  where T : BaseModel
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T?> GetEntityBySpecAsync(ISpecification<T> spec);
    Task<IReadOnlyList<T>> GetListBySpecAsync(ISpecification<T> spec);
    Task<TResult?> GetEntityBySpecAsync<TResult>(ISpecification<T,TResult> spec);
    Task<IReadOnlyList<TResult>> GetListBySpecAsync<TResult>(ISpecification<T,TResult> spec);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> SaveAllAsync();
    bool Exists(int id);

}
