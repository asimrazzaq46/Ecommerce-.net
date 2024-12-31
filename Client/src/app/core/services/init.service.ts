import { inject, Injectable, OnInit } from '@angular/core';
import { CartService } from './cart.service';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InitService  {


  private cartService = inject(CartService)

  init() {
    const cart_id = localStorage.getItem("cart_id");
    const cart$ = cart_id ? this.cartService.getCart(cart_id) : of(null);

    return cart$;
  }
 
}
