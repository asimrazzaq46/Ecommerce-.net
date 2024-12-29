import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { Product } from '../../../shared/models/product.model';
import { ActivatedRoute } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';

@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe, MatButton, MatIcon, MatFormField, MatInput,MatLabel,MatDivider],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  shopService = inject(ShopService);
  router = inject(ActivatedRoute);
  product = signal<Product | undefined>(undefined);

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    const id = this.router.snapshot.paramMap.get('id');

    if (!id) return;

    this.shopService.getProductById(+id).subscribe({
      next: (res) => this.product.set(res),
      error: (err) => console.log(err),
    });
  }
}
