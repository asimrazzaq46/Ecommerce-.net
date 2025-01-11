import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Order, orderToCreate } from '../../shared/models/order';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  baseUrl = environment.baseUrl;
  private http = inject(HttpClient);
  orderComplete = false;

  createOrder(orderToCreate: orderToCreate) {
    return this.http.post<Order>(this.baseUrl + '/orders', orderToCreate);
  }

  getOrdersForUser() {
    return this.http.get<Order[]>(this.baseUrl + '/orders');
  }

  getOrderDetail(id: number) {
    return this.http.get<Order>(this.baseUrl + `/orders/${id}`);
  }
}
