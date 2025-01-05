import { Address } from './address';

export interface User {
  firstName: string;
  lastName: string;
  email: string;
  address: Address;
}
