using Core.Interfaces;
using System.Linq.Expressions;

namespace Core.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>? Orderby { get; }
    Expression<Func<T, object>>? OrderbyDescending { get; }

    bool IsDistinct { get; }

    int Take { get; }
    int Skip { get; }
    bool IsPagingEnable { get; }
    IQueryable<T> ApplyCriteria(IQueryable<T> criteria);


}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T,TResult>>? Select {  get; } 
}