namespace Core.Models.OrderAggregate;

public class OrderItem:BaseModel
{
    public ProductItemOrdered ItemOrdered { get; set; } = null;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
