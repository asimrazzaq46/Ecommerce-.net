using Core.Interfaces;
using Core.Models;
using Infastructure.Data;
using Infastructure.SpecificationEval;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infastructure.Repositery;

public class GenericRepositery<T>(StoreContext _db) : IGenericRepositery<T> where T : BaseModel
{

    internal DbSet<T> _dbset = _db.Set<T>();

    public void Add(T entity)
    {
        _dbset.Add(entity);
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var query = _dbset.AsQueryable();
        query = spec.ApplyCriteria(query);
        return await query.CountAsync();
    }
     
    public bool Exists(int id)
    {
        return _dbset.Any(x => x.Id == id);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbset.FindAsync(id);
    }

    public async Task<T?> GetEntityBySpecAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<TResult?> GetEntityBySpecAsync<TResult>(ISpecification<T, TResult> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();

    }

    public async Task<IReadOnlyList<T>> GetListBySpecAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();

    }

    public async Task<IReadOnlyList<TResult>> GetListBySpecAsync<TResult>(ISpecification<T, TResult> spec)
    {
       return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {

        return await _dbset.ToListAsync();

    }


    public void Remove(T entity)
    {
        _dbset.Remove(entity);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public void Update(T entity)
    {
        _dbset.Attach(entity);
        _db.Entry(entity).State = EntityState.Modified;
    }


    private  IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbset.AsQueryable(), spec);
    }
    
    private  IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T,TResult> spec)
    {
        return SpecificationEvaluator<T>.GetQuery<T,TResult>(_dbset.AsQueryable(), spec);
    }

}
