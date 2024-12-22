using Core.Interfaces;
using System.Linq.Expressions;

namespace Core.Specifications;

public class BaseSpecification<T>(Expression<Func<T, bool>>? _criteria) : ISpecification<T>
{
    protected BaseSpecification() : this(null) { }

    public Expression<Func<T, bool>>? Criteria => _criteria; // _criteria ==>  x => x.Brand == Brand which we pass as a ctor

    public Expression<Func<T, object>>? Orderby { get; private set; }

    public Expression<Func<T, object>>? OrderbyDescending { get; private set; }

    public bool isDistinct { get; private set; }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        Orderby = orderByExpression;
    }
    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderbyDescending = orderByDescExpression;
    }

    protected void ApplyDitinct()
    {
        isDistinct = true;
    }
}


public class BaseSpecification<T, TResult>(Expression<Func<T, bool>>? _criteria)
    : BaseSpecification<T>(_criteria), ISpecification<T, TResult>
{
    public BaseSpecification() : this(null)
    {
        
    }
    public Expression<Func<T, TResult>>? Select { get; private set; }

    protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
    {
        Select = selectExpression;
    }
}