import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { PaymentSummary } from '../models/order';

@Pipe({
  name: 'paymentPreview',
})
export class PaymentPreviewPipe implements PipeTransform {
  transform(
    value:
      | ConfirmationToken['payment_method_preview']
      | PaymentSummary
      | null
      | undefined,
    ...args: unknown[]
  ): unknown {
    if (value && 'card' in value) {
      const { brand, last4, exp_month, exp_year } = (
        value as ConfirmationToken['payment_method_preview']
      ).card!;

      const expiray = exp_month + '/' + exp_year;

      return `${brand.toUpperCase()} **** **** **** ${last4},Exp: ${expiray}`;
    } else if (value && 'brand' in value) {
      const { brand, last4, expMonth, expYear } = value as PaymentSummary;

      const expiray = expMonth + '/' + expYear;

      return `${brand.toUpperCase()} **** **** **** ${last4},Exp: ${expiray}`;
    } else {
      return null;
    }
  }
}
