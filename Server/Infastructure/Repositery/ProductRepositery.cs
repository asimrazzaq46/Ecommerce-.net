using Core.Interfaces;
using Core.Models;
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositery;

public class ProductRepositery(StoreContext _db) : IProductRepositery
{
    public void AddProduct(Product product)
    {
        _db.Products.Add(product);
    }

    public void DeleteProduct(Product product)
    {
        _db.Products.Remove(product);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await _db.Products
            .Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _db.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand ,string? type,string? sort)
    {
        var query = _db.Products.AsQueryable();

        // if we search the product by brand

        if(!string.IsNullOrWhiteSpace(brand)) query = query.Where(p=>p.Brand == brand);

        // if we search the product by Type


        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(p=>p.Type == type);

        // if we SORT product by price

        query = sort switch
            {
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDsc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(x => x.Name)
            };
        


        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await _db.Products
            .Select(p => p.Type)
            .Distinct()
            .ToListAsync();

    }

    public bool ProductExists(int id)
    {
        return _db.Products.Any(x => x.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
       return await _db.SaveChangesAsync() > 0;
    }

    public void UpdateProduct(Product product)
    {
         _db.Entry(product).State = EntityState.Modified;
    }
}
