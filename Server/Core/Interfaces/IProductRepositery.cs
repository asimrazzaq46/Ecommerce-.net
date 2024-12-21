
using Core.Models;

namespace Core.Interfaces;

public interface IProductRepositery
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type,string? sort);
    Task<Product?> GetProductByIdAsync(int id);

    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();

    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    bool ProductExists(int id);
    Task<bool> SaveChangesAsync();


}
