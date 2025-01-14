using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Issuing;

namespace Infastructure.Services;

public class PaymentService(IConfiguration _config,
    ICartService _cartService,
    IUnitOfWork _unitOfWork) : IPaymentService
{
    public async Task<ShoppingCart?> CreateirUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = _config["StripeSetting:SecretKey"];

        // getting the cart by it's id

        var cart = await _cartService.GetCartAsync(cartId) ?? throw new Exception("Cart unavailable");

        // Setting shipping price if we have a delivery method inside out cart otherwise it's 0
        var shippingPrice = await GetDeliveryPriceAsync(cart) ?? 0;

        await ValidateCartItemsAsync(cart);


        var subTotal = CalculateSubTotal(cart);

        if (cart.Coupon != null)
        {
            subTotal = await ApplyDiscount(cart.Coupon,subTotal);
        }

        var total = subTotal + shippingPrice;

        await CreateirUpdatePaymentIntentAsync(cart, total);

        await _cartService.SetCartAsync(cart);

        return cart;

    }

    private static async Task CreateirUpdatePaymentIntentAsync(ShoppingCart cart, long total)
    {
        var service = new PaymentIntentService();


        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = total,
                Currency = "usd",
                PaymentMethodTypes = ["card"],
            };

            var intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = total
            };

             await service.UpdateAsync(cart.PaymentIntentId, options);
        }

    }

    private async Task<long> ApplyDiscount(AppCoupon appCoupon, long subTotal)
    {
        var couponService = new Stripe.CouponService();

        var coupon = await couponService.GetAsync(appCoupon.CouponId);

        if (coupon.AmountOff.HasValue) {
            if (coupon.AmountOff.Value > subTotal) throw new Exception("discount Amount cannot exceed the actual amount");
        subTotal -= (long)coupon.AmountOff.Value ;
        }

        if (coupon.PercentOff.HasValue)
        {
            var discount = (subTotal * coupon.PercentOff.Value) / 100;
            subTotal -= (long)discount;
        }

        return subTotal;

    }

    private static long CalculateSubTotal(ShoppingCart cart)
    {
       var itemTotal = cart.Items.Sum(x=>x.Quantity * x.Price * 100);
        return (long)itemTotal;
    }


    //Validatin the items in Cart
    private async Task ValidateCartItemsAsync(ShoppingCart cart)
    {
        // looping through each item from the cart
        foreach (var item in cart.Items)
        {
            //get the product from database to compare with the item inside the cart e.g price.

            var productItem = await _unitOfWork.Repositery<Core.Models.Product>().GetByIdAsync(item.ProductId)
                ?? throw new Exception("Problem getting product in cart");

            // if the price is not same than we update the cart item price with orignal product price
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }

    }


    //Getting the delivery method and return it's price
    private async Task<long?> GetDeliveryPriceAsync(ShoppingCart cart)
    {
        // Checking do we have a delivery method inside our cart? if we do than we get the price of that delivery option
        if (cart.DeliverMethodId.HasValue)
        {
            var deliveryMethod = await _unitOfWork.Repositery<DeliveryMethod>().GetByIdAsync(cart.DeliverMethodId.Value)
                ?? throw new Exception("Problem with delivery method");


        return (long)deliveryMethod.Price * 100;
        }

        return null;
    }
}
