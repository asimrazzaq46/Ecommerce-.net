import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';

@Pipe({
  name: 'address',
  standalone: true,
})
export class AddressPipe implements PipeTransform {
  transform(
    value: ConfirmationToken['shipping'] | null | undefined,
    ...args: unknown[]
  ): unknown {
    if (value?.address && value.name) {
      const { line1, line2, city, country, state, postal_code } = value.address;
      return `${value?.name},${line1}${
        line2 ? ', ' + line2 : ''
      },${city},${state},${state},${postal_code},${country}`;
    } else {
      return 'unknow address';
    }
  }
}