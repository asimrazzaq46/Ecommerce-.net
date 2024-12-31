import {
  APP_INITIALIZER,
  ApplicationConfig,
  importProvidersFrom,
  inject,
  provideAppInitializer,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { errorInterceptor } from './core/interceptor/error.interceptor';
import { loadingInterceptor } from './core/interceptor/loading.interceptor';
import { InitService } from './core/services/init.service';
import { lastValueFrom } from 'rxjs';

function initalizeApp() {
  const initService = inject(InitService);

  return lastValueFrom(initService.init()).finally(() => {
    const splash = document.getElementById('initial-splash');
    if (splash) {
      splash.remove();
    }
  });
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor, loadingInterceptor])),
    importProvidersFrom(),
    provideAnimationsAsync(),

    provideAppInitializer(initalizeApp),
  ],
};
