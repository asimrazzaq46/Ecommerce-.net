using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infastructure.Services;

public class CouponService(IConfiguration _config) : ICouponService
{
    
    public async Task<AppCoupon> GetCouponFromPromoCode(string code)
    {
        StripeConfiguration.ApiKey = _config["StripeSetting:SecretKey"];


        if (string.IsNullOrEmpty(code)) throw new Exception("PromoCode is not provided");

        var service = new PromotionCodeService();

        var options = new PromotionCodeListOptions { Code = code };

        var promotionInCodes = await service.ListAsync(options);

        var promotionCode = promotionInCodes.FirstOrDefault();

        if (promotionCode == null)
        {
            throw new Exception($"Promotion code '{code}' not found.");
        }



        if (promotionCode != null && promotionCode.Coupon != null)
        {

        return new AppCoupon
        {
            CouponId = promotionCode.Coupon.Id,
            PercentOff = promotionCode.Coupon.PercentOff,
            AmountOff = promotionCode.Coupon.AmountOff / 100,
            Name = promotionCode.Coupon.Name,
            PromotionCode = promotionCode.Code,

        };

        
        }

        return null;


    }

   
}
