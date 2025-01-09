using Core.Models;
using System.ComponentModel;

namespace Core.Interfaces;

public interface IUnitOfWork: IDisposable
{
    IGenericRepositery<TEntity> Repositery<TEntity>() where TEntity : BaseModel;
    Task<bool> Complete();
}
