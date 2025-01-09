using Core.Interfaces;
using System.Linq.Expressions;

namespace Core.Specifications;

public class BaseSpecification<T>(Expression<Func<T, bool>>? _criteria) : ISpecification<T>
{
    protected BaseSpecification() : this(null) { }

    public Expression<Func<T, bool>>? Criteria => _criteria; // _criteria ==>  x => x.Brand == Brand which we pass in a constructor

    public Expression<Func<T, object>>? Orderby { get; private set; }

    public Expression<Func<T, object>>? OrderbyDescending { get; private set; }

    public bool IsDistinct { get; private set; }

    public int Take { get; private set; }

    public int Skip { get; private set; }

    public bool IsPagingEnable { get; private set; }

    public List<Expression<Func<T, object>>> Includes { get; } = [];

    public List<string> ThenIncludeStrings { get; } = [];

    public IQueryable<T> ApplyCriteria(IQueryable<T> query)
    {
      if(Criteria != null)
        {
            query = query.Where(Criteria);  
        }
      return query;
    }


    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeStringExpression) //for thenInclude method
    {
        ThenIncludeStrings.Add(includeStringExpression);
    }

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
        IsDistinct = true;
    }

    protected void ApplyPagging(int skip,int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnable = true;
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