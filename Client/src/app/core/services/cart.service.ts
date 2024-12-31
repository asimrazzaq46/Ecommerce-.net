import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart, CartItem } from '../../shared/models/cart';
import { Product } from '../../shared/models/product.model';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baseurl = environment.baseUrl;
  private http = inject(HttpClient);

  cart = signal<Cart | null>(null);
  itemsCount = computed(() =>
    this.cart()?.items.reduce((sum, item) => sum + item.quantity, 0)
  );
  total = computed(() => {
    const cart = this.cart();
    if (!cart) return null;

    const subtotal = cart.items.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
    const shipping = 0;
    const discount = 0;
    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount,
    };
  });

  getCart(id: string) {
    return this.http.get<Cart>(this.baseurl + '/cart?id=' + id).pipe(
      map((data) => {
        this.cart.set(data);
        return data;
      })
    );
  }

  setCart(cart: Cart) {
    return this.http.post<Cart>(this.baseurl + '/cart', cart).subscribe({
      next: (cart) => this.cart.set(cart),
    });
  }

  deleteCart() {
    const cart = this.cart();
    if (!cart) return;
    return this.http.delete(this.baseurl + `/cart?id=${cart.id}`).subscribe({

      next: (_) => {
        localStorage.removeItem('cart_id');
        this.cart.set(null);
      },
    });
  }

  addItemToCart(item: CartItem | Product, quantity = 1) {
    //if we don't have any cart we will create it
    const cart = this.cart() ?? this.createCart();

    if (this.isProduct(item)) {
      item = this.mapProductToCartItem(item);
    }

    cart.items = this.addOrUpdateItem(cart.items, item, quantity);

    this.setCart(cart);
  }

  private createCart(): Cart {
    const cart = new Cart();

    // after creating the cart we save the cart id to our local storage for later getting it to checkout
    localStorage.setItem('cart_id', cart.id);

    return cart;
  }

  removeItemfromCart(productId: number, quantity = 1) {
    const cart = this.cart();
    if (!cart) return;

    const index = cart.items.findIndex((item) => item.productId === productId);
    if (index !== -1) {
      if (cart.items[index].quantity > quantity) {
        cart.items[index].quantity -= quantity;
      } else {
        cart.items.splice(index, 1);
      }
      if (cart.items.length === 0) {
        this.deleteCart();
      } else {
        this.setCart(cart);
      }
    }
  }

  // item is Product is return as a bool value base on our return result.
  private isProduct(item: CartItem | Product): item is Product {
    // (item as Product) is now behave like a product and we can check do we have id or not inside item
    // if item is really a product than it should have a id inside it otherwise it will be a Cartitem
    // because CartItem doesn't have any id property inside it...check the interfaces
    return (item as Product).id !== undefined;
  }

  private mapProductToCartItem(item: Product): CartItem {
    return {
      productId: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      type: item.type,
      brand: item.brand,
      quantity: 0,
    };
  }

  private addOrUpdateItem(
    items: CartItem[],
    item: CartItem,
    quantity: number
  ): CartItem[] {
    const index = items.findIndex((x) => x.productId === item.productId);
    if (index === -1) {
      item.quantity = quantity;
      items.push(item);
    } else {
      items[index].quantity += quantity;
    }

    return items;
  }
}
