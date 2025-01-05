import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { SnackBarService } from '../services/snack-bar.service';

export const emptyCartGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService);
  const snackbarService = inject(SnackBarService);
  const router = inject(Router);

  if (!cartService.cart() || cartService.cart()?.items.length === 0) {
    snackbarService.error('Cart is empty');
    router.navigateByUrl('/cart');
    return false;
  }
  return true;
};
