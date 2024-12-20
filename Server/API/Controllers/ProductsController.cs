using Core.Models;
using Infastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController(StoreContext _db) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _db.Products.ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _db.Products.Add(product); 
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProduct) , new { id = product.Id},product);
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id,Product product)
    {
        if (product.Id != id || !ProductExist(id)) return BadRequest("Cannot update this product");
        _db.Entry(product).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return NoContent();

    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product == null) return NotFound();

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return NoContent();
    }



    private bool ProductExist(int id)
    {
        return _db.Products.Any(p=>p.Id == id);
    }

}
