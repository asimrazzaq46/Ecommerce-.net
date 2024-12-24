using API.RequestHelpers;
using Core.Interfaces;
using Core.Models;
using Core.Params;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IGenericRepositery<Product> _repo) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecsSearchParams searchParams)
    {

        var specs = new ProductSpecification(searchParams);

   
        return await CreatePageResult(_repo,specs,searchParams.pageIndex,searchParams.pageSize);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _repo.Add(product);
        if (!await _repo.SaveAllAsync()) return BadRequest("Product Cannot be Added");
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExist(id)) return BadRequest("Cannot update this product");
        _repo.Update(product);
        if (await _repo.SaveAllAsync()) return NoContent();


        return BadRequest("Product cannot be Updated");

    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _repo.GetByIdAsync(id);

        if (product == null) return NotFound();

        _repo.Remove(product);
        if (await _repo.SaveAllAsync()) return NoContent();

        return BadRequest("Product Cannot be deleted");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var specs = new BrandListSpecification();

        return Ok(await _repo.GetListBySpecAsync(specs));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {

        var specs = new TypeListSpecification();

        return Ok(await _repo.GetListBySpecAsync(specs));

    }



    private bool ProductExist(int id)
    {
        return _repo.Exists(id);
    }

}
