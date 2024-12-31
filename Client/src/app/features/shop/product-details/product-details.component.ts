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
import { FormsModule } from '@angular/forms';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-product-details',
  imports: [
    CurrencyPipe,
    MatButton,
    MatIcon,
    MatFormField,
    MatInput,
    MatLabel,
    MatDivider,
    FormsModule,
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  shopService = inject(ShopService);
  router = inject(ActivatedRoute);
  product = signal<Product | undefined>(undefined);
  private cartService = inject(CartService);

  quantity = 1;
  quantityInCart = 0;

  ngOnInit(): void {
    this.loadProduct();
    this.updateQtyInCart();
  }

  loadProduct() {
    const id = this.router.snapshot.paramMap.get('id');

    if (!id) return;

    this.shopService.getProductById(+id).subscribe({
      next: (res) => {
        this.product.set(res);
        this.updateQtyInCart();
      },
      error: (err) => console.log(err),
    });
  }

  updateCart() {
    if (!this.product()) return;

    if (this.quantity > this.quantityInCart) {
      const itemsToAdd = this.quantity - this.quantityInCart; // give the result of how many qantity we need to add 3 or 4 or 5
      this.quantityInCart += itemsToAdd; // then we add that quantity in quantityInCart
      this.cartService.addItemToCart(this.product() as Product, itemsToAdd);
    } else {
      const itemToRemove = this.quantityInCart - this.quantity;
      this.quantityInCart -= itemToRemove;
      this.cartService.removeItemfromCart(this.product()!.id, itemToRemove);
    }
  }

  updateQtyInCart() {
    const cart = this.cartService.cart();
    if (!cart) return;

    this.quantityInCart =
      cart.items.find((x) => x.productId === this.product()?.id)?.quantity || 0;

    this.quantity = this.quantityInCart || 1;
  }

  getButtontext() {
    return this.quantityInCart > 0 ? `Update Cart` : 'Add To Cart';
  }
}
