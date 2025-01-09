using API.DTOs;
using API.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Models.OrderAggregate;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class OrdersController(IUnitOfWork _unit, ICartService _cartService) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
    {
        var userEmail = User.GetEmail();

        var cart = await _cartService.GetCartAsync(orderDto.CartId);

        if (cart == null) return BadRequest("Cart not found");

        if (cart.PaymentIntentId == null) return BadRequest("no payment intent for this order");

        var items = new List<OrderItem>();

        foreach (var item in cart.Items)
        {

            var productItem = await _unit.Repositery<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return BadRequest("Problem with the order");

            var itemOrders = new ProductItemOrdered
            {
                ProductId = productItem.Id,
                PictureUrl = productItem.PictureUrl,
                ProductName = productItem.Name
            };

            var orderItem = new OrderItem
            {
                Price = productItem.Price,
                Quantity = item.Quantity,
                ItemOrdered = itemOrders
            };

            items.Add(orderItem);

        }


        var deliveryMethod = await _unit.Repositery<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
        if (deliveryMethod == null) return BadRequest("No delivery method selected");

        var order = new Order
        {
            BuyerEmail = userEmail,
            DeliveryMethod = deliveryMethod,
            PaymentIntentId = cart.PaymentIntentId,
            OrderItems = items,
            ShippingAddress = orderDto.ShippingAddress,
            PaymentSummary = orderDto.PaymentSummary,
            SubTotal = items.Sum(item => item.Quantity * item.Price),
        };


        _unit.Repositery<Order>().Add(order);

        if (await _unit.Complete())
        {
            return order;
        }
        return BadRequest("Problem With the order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrderForUser()
    {
        var specs = new OrderSpecification(User.GetEmail());

        var orders = await _unit.Repositery<Order>().GetListBySpecAsync(specs);
        if (orders == null) return BadRequest("Order list is not found");

        var orderToReturn = orders.Select(o=>o.ToDto()).ToList(); 

        return Ok(orderToReturn);

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var spec = new OrderSpecification(User.GetEmail(),id);

        var order = await _unit.Repositery<Order>().GetEntityBySpecAsync(spec);

        if (order == null) return NotFound();

        return order.ToDto();
    }



}
