using Core.Interfaces;
using Core.Models;

namespace Infastructure.SpecificationEval;

public class SpecificationEvaluator<T> where T : BaseModel
{
    public static IQueryable<T> GetQuery(IQueryable<T> query,ISpecification<T> spec)
    {
        if(spec.Criteria  != null)
        {
            query = query.Where(spec.Criteria);  // spec.Criteria = x => x.Brand == Brand
        }

        if (spec.Orderby != null) { 
        
            query = query.OrderBy(spec.Orderby);

        }

        if (spec.OrderbyDescending != null) { 
        
            query = query.OrderByDescending(spec.OrderbyDescending);
        }

        if (spec.IsDistinct)
        {
            query = query.Distinct();
        }

        if (spec.IsPagingEnable) { 
        
        query = query.Skip(spec.Skip).Take(spec.Take);

        }

        return query;   
    }
    
    public static IQueryable<TResult> GetQuery<TSpec,TResult>(IQueryable<T> query,ISpecification<T,TResult> spec)
    {
        if(spec.Criteria  != null)
        {
            query = query.Where(spec.Criteria);  // spec.Criteria = x => x.Brand == Brand
        }

        if (spec.Orderby != null) { 
        
            query = query.OrderBy(spec.Orderby);

        }

        if (spec.OrderbyDescending != null) { 
        
            query = query.OrderByDescending(spec.OrderbyDescending);
        }


        var selectQuery = query as IQueryable<TResult>;
        if (spec.Select != null) {

            selectQuery = query.Select(spec.Select); 
        }

        if (spec.IsDistinct)
        {
            selectQuery = selectQuery?.Distinct();
        }

        if (spec.IsPagingEnable) {

            selectQuery = selectQuery?.Skip(spec.Skip).Take(spec.Take);
        }


        return selectQuery ?? query.Cast<TResult>();   
    }

}
