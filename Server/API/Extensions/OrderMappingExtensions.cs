using API.DTOs;
using Core.Models.OrderAggregate;

namespace API.Extensions;

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            BuyerEmail = order.BuyerEmail,
            OrderDate = order.OrderDate,
            DeliveryMethod = order.DeliveryMethod.Description,
            OrderItems = order.OrderItems.Select(x => x.ToDto()).ToList(),
            ShippingAddress = order.ShippingAddress,
            PaymentIntentId = order.PaymentIntentId,
            PaymentSummary = order.PaymentSummary,
            ShippingPrice = order.DeliveryMethod.Price,
            SubTotal = order.SubTotal,
            OrderStatus = order.OrderStatus.ToString(),
        };
    }

    public static OrderItemDto ToDto(this OrderItem orderItem)
    {

        return new OrderItemDto
        {

            PictureUrl = orderItem.ItemOrdered.PictureUrl,
            ProductId = orderItem.ItemOrdered.ProductId,
            ProductName = orderItem.ItemOrdered.ProductName,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity,
        };

    }
}
