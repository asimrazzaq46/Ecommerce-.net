using Core.Interfaces;
using Core.Models;
using Core.Params;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IUnitOfWork _unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecsSearchParams searchParams)
    {

        var specs = new ProductSpecification(searchParams);

   
        return await CreatePageResult(_unitOfWork.Repositery<Product>(),specs,searchParams.pageIndex,searchParams.pageSize);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _unitOfWork.Repositery<Product>().GetByIdAsync(id);
        if (product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _unitOfWork.Repositery<Product>().Add(product);
        if (!await _unitOfWork.Complete()) return BadRequest("Product Cannot be Added");
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExist(id)) return BadRequest("Cannot update this product");
        _unitOfWork.Repositery<Product>().Update(product);
        if (await _unitOfWork.Complete()) return NoContent();


        return BadRequest("Product cannot be Updated");

    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _unitOfWork.Repositery<Product>().GetByIdAsync(id);

        if (product == null) return NotFound();

        _unitOfWork.Repositery<Product>().Remove(product);
        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Product Cannot be deleted");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var specs = new BrandListSpecification();

        return Ok(await _unitOfWork.Repositery<Product>().GetListBySpecAsync(specs));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {

        var specs = new TypeListSpecification();

        return Ok(await _unitOfWork.Repositery<Product>().GetListBySpecAsync(specs));

    }



    private bool ProductExist(int id)
    {
        return _unitOfWork.Repositery<Product>().Exists(id);
    }

}
