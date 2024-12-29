import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize } from 'rxjs';
import { BusyService } from '../services/busy.service';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busy = inject(BusyService);

  busy.startLoading();
  return next(req).pipe(
    delay(500),
    finalize(() => busy.stopLoading())
  );
};
