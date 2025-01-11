import { Component, inject, OnInit, signal } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCard } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address.pipe';
import { PaymentPreviewPipe } from '../../../shared/pipes/payment-preview.pipe';

@Component({
  selector: 'app-order-detail',
  imports: [
    MatCard,
    MatButton,
    DatePipe,
    CurrencyPipe,
    AddressPipe,
    PaymentPreviewPipe,
    RouterLink
  ],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.scss',
})
export class OrderDetailComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);

  order = signal<Order | null>(null);

  ngOnInit(): void {
    this.loadOrder();
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.orderService.getOrderDetail(+id).subscribe({
      next: (data) => this.order.set(data),
    });
  }
}
