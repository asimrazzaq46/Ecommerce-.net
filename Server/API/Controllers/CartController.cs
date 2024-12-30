using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController(ICartService _cartService) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        var cart = await _cartService.GetCartAsync(id);

        return Ok(cart ?? new ShoppingCart { Id = id});
        
    }


    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart = await _cartService.SetCartAsync(cart);

        if (updatedCart == null) return BadRequest("Problem with Cart");

        return Ok(updatedCart);
    }


    [HttpDelete]
    public async Task<ActionResult> DeleteCart(string id)
    {
        var result = await _cartService.DeleteCartAsync(id);

        if (!result) return BadRequest("Problem deleting cart");

        return Ok();

    }


}
