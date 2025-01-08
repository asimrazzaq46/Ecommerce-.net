import { Component, inject, OnInit, output } from '@angular/core';
import { MatRadioModule } from '@angular/material/radio';

import { CheckoutService } from '../../../core/services/checkout.service';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethod';

@Component({
  selector: 'app-checkout-delivery',
  imports: [MatRadioModule, CurrencyPipe],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss',
})
export class CheckoutDeliveryComponent implements OnInit {
  checkoutService = inject(CheckoutService);
  cartService = inject(CartService);

  checkoutDeliveryComplete = output<boolean>();

  ngOnInit(): void {
    this.checkoutService.getDeliveryMethods().subscribe({
      next: (methods) => {
        if (this.cartService.cart()?.deliverMethodId) {
          const method = methods.find(
            (x) => x.id == this.cartService.cart()?.deliverMethodId
          );
          if (method) {
            this.cartService.selectedDelivery.set(method);
            this.checkoutDeliveryComplete.emit(true);
          }
        }
      },
    });
  }

  updateDeliverymethod(method: DeliveryMethod) {
    this.cartService.selectedDelivery.set(method);
    const cart = this.cartService.cart();
    if (cart) {
      cart.deliverMethodId = method.id;
      this.cartService.setCart(cart);
      this.checkoutDeliveryComplete.emit(true);
    }
  }
}
