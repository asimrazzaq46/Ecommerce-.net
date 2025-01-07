import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  baseUrl = environment.baseUrl;
  private http = inject(HttpClient);

  deliveryMethods = signal<DeliveryMethod[]>([]);

  getDeliveryMethods() {
    if (this.deliveryMethods().length > 0) return of(this.deliveryMethods());

    return this.http
      .get<DeliveryMethod[]>(this.baseUrl + '/payment/delivery-methods')
      .pipe(
        map((methods) => {
          const sortedDeliveryMethodsArray = methods.sort(
            (a, b) => b.price - a.price
          );

          this.deliveryMethods.set(sortedDeliveryMethodsArray);
          return methods;
        })
      );
  }
}
