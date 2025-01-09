﻿using Core.Models.OrderAggregate;
using Core.Models;

namespace API.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public required string BuyerEmail { get; set; }
    public required ShippingAddress ShippingAddress { get; set; }
    public required string DeliveryMethod { get; set; }
    public decimal ShippingPrice { get; set; }
    public required PaymentSummary PaymentSummary { get; set; }
    public required List<OrderItemDto> OrderItems { get; set; }
    public decimal SubTotal { get; set; }
    public required string OrderStatus { get; set; }
    public required string PaymentIntentId { get; set; }

}