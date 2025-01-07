

using Core.Models;
using System.Text.Json;

namespace Infastructure.Data;
public class SeedStoreContext()
{
    public static async Task SeedAsync(StoreContext _db)
    {
        if (!_db.Products.Any())
        {
           
            string path =  "../Infastructure/Data/SeedData/products.json";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Seed data file not found at path: {path}");
            }

            var productsData = await File.ReadAllTextAsync(path);

            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products is null) return;

            await _db.Products.AddRangeAsync(products);

            await _db.SaveChangesAsync();

        }

        if (!_db.DeliveryMethods.Any()) {

            string path = "../Infastructure/Data/SeedData/delivery.json";

            if (!File.Exists(path)) {
                throw new FileNotFoundException($"Seed Data file not found at path: {path}");
            }

            var deliveryData = await File.ReadAllTextAsync(path);

            var deliveries = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

            if (deliveries is null) return;

            await _db.DeliveryMethods.AddRangeAsync(deliveries);

            await _db.SaveChangesAsync();
        
        }
    }
}
