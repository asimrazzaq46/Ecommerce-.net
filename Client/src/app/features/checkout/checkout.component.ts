import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummaryComponent } from '../../shared/components/order-summary/order-summary.component';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatButton } from '@angular/material/button';
import { Address } from '../../shared/models/address';
import {
  MatCheckboxChange,
  MatCheckboxModule,
} from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { StripeService } from '../../core/services/stripe.service';
import {
  ConfirmationToken,
  StripeAddressElement,
  StripeAddressElementChangeEvent,
  StripePaymentElement,
  StripePaymentElementChangeEvent,
} from '@stripe/stripe-js';
import { SnackBarService } from '../../core/services/snack-bar.service';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { AccountService } from '../../core/services/account.service';
import { firstValueFrom } from 'rxjs';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { CartService } from '../../core/services/cart.service';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    OrderSummaryComponent,
    MatStepperModule,
    MatButton,
    MatProgressSpinnerModule,
    RouterLink,
    MatCheckboxModule,
    CheckoutDeliveryComponent,
    CheckoutReviewComponent,
    CurrencyPipe,
  ],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private stripeService = inject(StripeService);
  private snackBarService = inject(SnackBarService);
  private accountService = inject(AccountService);
  private router = inject(Router);
  cartService = inject(CartService);

  addressElement?: StripeAddressElement;
  paymentElement?: StripePaymentElement;

  saveAddress = false;
  completionStatus = signal<{
    address: boolean;
    card: boolean;
    delivery: boolean;
  }>({ address: false, card: false, delivery: false });

  confirmationToken?: ConfirmationToken;
  loading = false;

  async ngOnInit() {
    try {
      // Creating AddressElement

      this.addressElement =
        await this.stripeService.createStripeAddressElement();

      this.addressElement.mount('#address-element');

      this.addressElement.on('change', this.handleAddressChange);

      // Creating PaymentElement

      this.paymentElement =
        await this.stripeService.createStripePaymentElement();

      this.paymentElement.mount('#payment-element');

      this.paymentElement.on('change', this.handlePaymentChange);
    } catch (error: any) {
      this.snackBarService.error(error.message);
    }
  }

  handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    this.completionStatus.update((state) => {
      state.address = event.complete;
      return state;
    });
  };

  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    this.completionStatus.update((state) => {
      state.card = event.complete;
      return state;
    });
  };

  handleDeliveryChange(event: boolean) {
    this.completionStatus.update((state) => {
      state.delivery = event;
      return state;
    });
  }

  onSaveAddressCheckboxChanged(event: MatCheckboxChange) {
    this.saveAddress = event.checked;
  }

  async getConfirmationToken() {
    try {
      if (
        Object.values(this.completionStatus()).every(
          (status) => status === true
        )
      ) {
        const result = await this.stripeService.createConfirmationToken();
        if (result.error) {
          throw new Error(result.error.message);
        }

        this.confirmationToken = result.confirmationToken;
        console.log(`confirmation token`, this.confirmationToken);
      }
    } catch (error: any) {
      this.snackBarService.error(error.message);
    }
  }

  async confirmPayment(stepper: MatStepper) {
    this.loading = true;
    try {
      if (this.confirmationToken) {
        const result = await this.stripeService.confirmPayment(
          this.confirmationToken
        );
        if (result.error) {
          throw new Error(result.error.message);
        } else {
          this.cartService.deleteCart();
          this.cartService.selectedDelivery.set(null);
          this.router.navigateByUrl('/checkout/success');
        }
      }
    } catch (error: any) {
      this.snackBarService.error(error.message);
      stepper.previous();
    } finally {
      this.loading = false;
    }
  }

  async onStepChange(event: StepperSelectionEvent) {
    if (event.selectedIndex === 1) {
      if (this.saveAddress) {
        const address = await this.getAddressFromStripeAddress();
        address && firstValueFrom(this.accountService.updateAddress(address));
      }
    }

    if (event.selectedIndex === 2) {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }

    if (event.selectedIndex === 3) {
      await this.getConfirmationToken();
    }
  }

  private async getAddressFromStripeAddress(): Promise<Address | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;

    if (address) {
      return {
        line1: address.line1,
        line2: address.line2 || undefined,
        city: address.city,
        country: address.country,
        postalCode: address.postal_code,
        state: address.state,
      };
    } else return null;
  }

  ngOnDestroy(): void {
    this.stripeService.disposeElements();
  }
}