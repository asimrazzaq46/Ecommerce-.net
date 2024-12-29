import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackBarService } from '../services/snack-bar.service';
import { state } from '@angular/animations';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackBarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err) {
        switch (err.status) {
          case 400:
            if (err.error.errors) {
              const modelStateErrors = [];
              for (const key in err.error.errors) {
                if (err.error.errors[key]) {
                  modelStateErrors.push(err.error.errors[key]);
                }
              }

              throw modelStateErrors.flat();
            } else {
              snackbar.error(err.error || err.error);
            }
            break;

          case 401:
            snackbar.error(err.error.title || err.error);
            break;

          case 404:
            router.navigateByUrl('not-found');
            break;

          case 500:
            const navigationExtras: NavigationExtras = {
              state: { error: err.error },
            };
            router.navigateByUrl('server-error', navigationExtras);
            break;

          default:
            snackbar.error('something unexpected happened');
            break;
        }
      }

      return throwError(() => err);
    })
  );
};
