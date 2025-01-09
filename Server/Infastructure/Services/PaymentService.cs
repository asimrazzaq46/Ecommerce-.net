using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infastructure.Services;

public class PaymentService(IConfiguration _config,
    ICartService _cartService,
    IUnitOfWork _unitOfWork) : IPaymentService
{
    public async Task<ShoppingCart?> CreateirUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = _config["StripeSetting:SecretKey"];

        // getting the cart by it's id

        var cart = await _cartService.GetCartAsync(cartId);

        if (cart == null) return null;

        var shippingPrice = 0m;

        // Checking do we have a delivery method inside our cart? if we do than we get the price of that delivery option

        if (cart.DeliverMethodId.HasValue)
        {
            var deliveryMethod = await _unitOfWork.Repositery<DeliveryMethod>().GetByIdAsync(cart.DeliverMethodId.Value);

            if (deliveryMethod == null) return null;

            shippingPrice = deliveryMethod.Price;
        }


        // looping through each item from the cart

        foreach (var item in cart.Items)
        {

            //get the product from database to compare with the item inside the cart e.g price.

            var productItem = await _unitOfWork.Repositery<Core.Models.Product>().GetByIdAsync(item.ProductId);

            if (productItem == null) return null;

            // if the price is not same than we update the cart item price with orignal product price
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }

        }

        var service = new PaymentIntentService();

        PaymentIntent? intent = null;

        if (string.IsNullOrEmpty(cart.PaymentIntentId)) {

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(x=>x.Quantity * (x.Price * 100)) + (long)(shippingPrice * 100),
                Currency = "usd",
                PaymentMethodTypes = ["card"],
            };

            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {

            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)(shippingPrice * 100),
            };

            intent = await service.UpdateAsync(cart.PaymentIntentId, options);

        }


        await _cartService.SetCartAsync(cart);

        return cart;




    }
}
