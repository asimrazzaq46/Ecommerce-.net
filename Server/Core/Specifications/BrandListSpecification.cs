using Core.Models;
using System.Linq.Expressions;

namespace Core.Specifications;

public class BrandListSpecification : BaseSpecification<Product, string>
{
    public BrandListSpecification()
    {
        AddSelect(p => p.Brand);
        ApplyDitinct();
    }
}
