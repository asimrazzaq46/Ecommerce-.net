using Core.Models;
using Core.Params;
using System.Linq.Expressions;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(ProductSpecsSearchParams searchParams) : base(x=>
        (string.IsNullOrEmpty(searchParams.Search) || x.Name.ToLower().Contains(searchParams.Search)) &&
        (searchParams.Brands.Count == 0 || searchParams.Brands.Contains(x.Brand)) &&
        (searchParams.Types.Count == 0 || searchParams.Types.Contains(x.Type))
        )
    {

        ApplyPagging(searchParams.pageSize * (searchParams.pageIndex - 1),searchParams.pageSize);


        switch (searchParams.Sort)
        {
            case "priceAsc":
                AddOrderBy(x => x.Price);
                break;
            case "priceDsc":
                AddOrderByDescending(x => x.Price);
                break;

            default:
                AddOrderBy(x=>x.Name);
                break;
        }
    }
}
