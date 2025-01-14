using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CouponController(ICouponService _couponService) : BaseApiController
{

    [HttpGet("{code}")]
    public async Task<ActionResult<AppCoupon>> ValidateCoupon(string code)
    {
        var coupon = await _couponService.GetCouponFromPromoCode(code);

        if (coupon == null) return BadRequest("Invalid voucher code");

        return coupon;
    } 

}
