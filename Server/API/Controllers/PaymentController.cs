using API.Extensions;
using API.SignalR;
using Core.Interfaces;
using Core.Models;
using Core.Models.OrderAggregate;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentController(IPaymentService _paymentService,
    IUnitOfWork _unitOfWork,
    ILogger<PaymentController> _logger,
    IConfiguration config,
    IHubContext<NotificationHub> _hubContext
    ) : BaseApiController
{

    private readonly string _whSecret = config["StripeSetting:whSecret"];

    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await _paymentService.CreateirUpdatePaymentIntent(cartId);

        if (cart == null) return BadRequest("Problem with your cart");

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        return Ok(await _unitOfWork.Repositery<DeliveryMethod>().ListAllAsync());
    }


    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebHook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);
            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                return BadRequest("Invalid event data");
            }

            await HandlePaymentIntentSucceed(intent);

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "stripe webhook error occoured");
            return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error occured");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error occoured");
            return StatusCode(StatusCodes.Status500InternalServerError, "unexpected error occured");
            throw;
        }

    }

    private async Task HandlePaymentIntentSucceed(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            Console.WriteLine("intent is : ", intent);
            var spec = new OrderSpecification(intent.Id, true);

            var order = await _unitOfWork.Repositery<Order>().GetEntityBySpecAsync(spec)
                ?? throw new Exception("Order not found");

            var orderTotal = (long)Math.Round(order.GetTotal() * 100, MidpointRounding.AwayFromZero);

            if (orderTotal != intent.Amount)
            {
                order.OrderStatus = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.OrderStatus = OrderStatus.PaymentReceived;
            }

            await _unitOfWork.Complete();

            //  SignalR notify client about the payment

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("OrderCOmpleteNotification", order.ToDto());
            }

        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Failed to construct stripe event");
            throw new StripeException("Invalid Signature");
        }
    }
}
