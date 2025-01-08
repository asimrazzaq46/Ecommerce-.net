import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';

@Pipe({
  name: 'paymentPreview',
})
export class PaymentPreviewPipe implements PipeTransform {
  transform(
    value: ConfirmationToken['payment_method_preview'] | null | undefined,
    ...args: unknown[]
  ): unknown {
    if (value?.card) {
      const { brand, last4, exp_month, exp_year } = value.card;

      const expiray = exp_month + '/' + exp_year;

      return `${brand.toUpperCase()} **** **** **** ${last4},Exp: ${expiray}`;
    } else {
      return null;
    }
  }
}
