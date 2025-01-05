import { inject, Injectable, OnInit } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);

  init() {
    const cart_id = localStorage.getItem('cart_id');
    const cart$ = cart_id ? this.cartService.getCart(cart_id) : of(null);
    
    return forkJoin({
      cart: cart$,
      user: this.accountService.getUserInfo(),
    });
  }
}
