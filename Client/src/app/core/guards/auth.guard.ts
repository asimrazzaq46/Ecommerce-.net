import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { SnackBarService } from '../services/snack-bar.service';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const snackBarService = inject(SnackBarService);
  const router = inject(Router);

  if (accountService.currentUser()) {
    return of(true);
  } else {
    snackBarService.error('First login to access this resource.');
    return accountService.getAuthState().pipe(
      map((auth) => {
        if (auth.isAuthenticated) {
          return true;
        } else {
          router.navigate(['/account/login'], {
            queryParams: { returnUrl: state.url },
          });
          return false;
        }
      })
    );
  }
};
