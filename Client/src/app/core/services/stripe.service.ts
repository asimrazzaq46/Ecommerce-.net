import { inject, Injectable } from '@angular/core';
import {
  ConfirmationToken,
  loadStripe,
  Stripe,
  StripeAddressElement,
  StripeAddressElementOptions,
  StripeCardElement,
  StripeElement,
  StripeElements,
  StripePaymentElement,
} from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart.service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class StripeService {
  private stripePromise: Promise<Stripe | null>;
  private stripeKey = environment.stripePublicKey;
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElement?: StripePaymentElement;

  private http = inject(HttpClient);
  private cartService = inject(CartService);
  private accountService = inject(AccountService);

  baseUrl = environment.baseUrl;

  constructor() {
    this.stripePromise = loadStripe(this.stripeKey);
  }

  private getStripeInstance(): Promise<Stripe | null> {
    return this.stripePromise;
  }

  private async initializeElements() {
    if (!this.elements) {
      const stripe = await this.getStripeInstance();
      if (stripe) {
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements({
          clientSecret: cart.clientSecret,
          appearance: { labels: 'floating', theme: 'flat' },
        });
      } else {
        throw new Error('stripe has not been loaded');
      }
    }
    return this.elements;
  }

  async createStripeAddressElement() {
    if (!this.addressElement) {
      const elements = await this.initializeElements();
      if (elements) {
        const user = this.accountService.currentUser();

        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};

        if (user) {
          defaultValues.name = user.firstName + ' ' + user.lastName;
        }

        if (user?.address) {
          defaultValues.address = {
            line1: user.address.line1,
            line2: user.address.line2,
            city: user.address.city,
            country: user.address.country,
            postal_code: user.address.postalCode,
            state: user.address.state,
          };
        }

        const options: StripeAddressElementOptions = {
          mode: 'shipping',
          defaultValues,
        };
        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Element instance has not been loaded!');
      }
    }

    return this.addressElement;
  }

  async createStripePaymentElement() {
    if (!this.paymentElement) {
      const elements = await this.initializeElements();
      if (elements) {
        this.paymentElement = elements.create('payment');
      } else throw new Error('Element instance has not been loaded');
    }
    return this.paymentElement;
  }

  // after we completed every step now we have to create a confirmation Payment token
  async createConfirmationToken() {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if (result.error) {
      throw new Error(result.error.message);
    }

    if (stripe) {
      return await stripe.createConfirmationToken({ elements });
    } else {
      throw new Error('stripe not available');
    }
  }

  async confirmPayment(confirmationToken: ConfirmationToken) {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit();

    if (result.error) {
      throw new Error(result.error.message);
    }

    const clientSecret = this.cartService.cart()?.clientSecret;

    if (stripe && clientSecret) {
      return await stripe.confirmPayment({
        clientSecret,
        confirmParams: { confirmation_token: confirmationToken.id },
        redirect: 'if_required',
      });
    } else {
      throw new Error('unable to load stripe');
    }
  }

  createOrUpdatePaymentIntent() {
    const cart = this.cartService.cart();

    if (!cart) throw new Error('Problem with cart');
    const hasClient = !!cart.clientSecret;

    return this.http.post<Cart>(this.baseUrl + '/payment/' + cart.id, {}).pipe(
      map((cart) => {
        if (!hasClient) {
          this.cartService.setCart(cart);
          return cart;
        }
        return cart;
      })
    );
  }

  disposeElements() {
    this.elements = undefined;
    this.addressElement = undefined;
    this.paymentElement = undefined;
  }
}
