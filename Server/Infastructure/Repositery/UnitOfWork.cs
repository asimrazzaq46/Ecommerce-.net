using Core.Interfaces;
using Core.Models;
using Infastructure.Data;
using System.Collections.Concurrent;

namespace Infastructure.Repositery;

public class UnitOfWork(StoreContext _db) : IUnitOfWork
{

    
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    public async Task<bool> Complete()
    {
       return await _db.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    public IGenericRepositery<TEntity> Repositery<TEntity>() where TEntity : BaseModel
    {
       var type = typeof(TEntity).Name; // gives the name of entity e.g Product, Order etc

        // returning dictionary like this {"Product" , GenericRepositery<Product>()}...if it already have than it will return
        // with the key "Product" other wise it will create one
        return (IGenericRepositery<TEntity>)_repositories.GetOrAdd(type, t =>
        {

            // this code create dynamically generic repositery by using reflection ===> GenericRepositery<Product>
            var repositeryType = typeof(GenericRepositery<>).MakeGenericType(typeof(TEntity));

            // this will return the instance of above created generic repositery ====> new GenericRepositery<Product>()
            //createInstance take parameters first the class we want to create and other parameters are the args of
            //the constructor of that class

            return Activator.CreateInstance(repositeryType, _db) ??
            throw new InvalidOperationException($"Could not create repositery instance for {t}");
        });
    }
}
