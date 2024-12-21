using Core.Interfaces;
using Core.Models;
using Infastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController(IProductRepositery _productRepo) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        var products = await _productRepo.GetProductsAsync(brand, type, sort);

        return products.ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productRepo.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _productRepo.AddProduct(product);
        if (!await _productRepo.SaveChangesAsync()) return BadRequest("Product Cannot be Added");
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExist(id)) return BadRequest("Cannot update this product");
        _productRepo.UpdateProduct(product);
        if (await _productRepo.SaveChangesAsync()) return NoContent();


        return BadRequest("Product cannot be Updated");

    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _productRepo.GetProductByIdAsync(id);

        if (product == null) return NotFound();

        _productRepo.DeleteProduct(product);
        if (await _productRepo.SaveChangesAsync()) return NoContent();

        return BadRequest("Product Cannot be deleted");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var products = await _productRepo.GetBrandsAsync();
        return products.ToList();
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var products = await _productRepo.GetTypesAsync();
        return products.ToList();
    }



    private bool ProductExist(int id)
    {
        return _productRepo.ProductExists(id);
    }

}
