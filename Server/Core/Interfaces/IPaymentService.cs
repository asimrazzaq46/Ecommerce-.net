using Core.Models;

namespace Core.Interfaces;

public interface IPaymentService
{
    Task<ShoppingCart?> CreateirUpdatePaymentIntent(string cartId);
}
