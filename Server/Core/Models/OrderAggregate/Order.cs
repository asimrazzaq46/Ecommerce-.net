﻿namespace Core.Models.OrderAggregate;

public class Order:BaseModel
{
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public required string BuyerEmail { get; set; }
    public ShippingAddress ShippingAddress { get; set; } = null!;
    public DeliveryMethod DeliveryMethod { get; set; } = null!;
    public PaymentSummary PaymentSummary { get; set; }=null!;
    public List<OrderItem> OrderItems { get; set; } = [];
    public decimal SubTotal { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public required string PaymentIntentId { get; set; }
    public decimal Discount { get; set; }

    public decimal GetTotal()
    {
        
        return SubTotal - Discount + DeliveryMethod.Price;
    }


}
