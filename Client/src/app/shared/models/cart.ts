import { nanoid } from 'nanoid';
export interface CartType {
  id: string;
  items: CartItem[];
  clientSecret?: string;
  paymentIntentId?: string;
  deliverMethodId?: number;
}

export interface CartItem {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
  type: string;
}

export class Cart implements CartType {
  id = nanoid();
  items: CartItem[] = [];
  clientSecret?: string;
  paymentIntentId?: string;
  deliverMethodId?: number;
}
