import { nanoid } from 'nanoid';

export interface CartType {
  id: string;
  items: CartItem[];
  clientSecret?: string;
  paymentIntentId?: string;
  deliverMethodId?: number;
  coupon?: Coupon;
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

export interface Coupon {
  name: string;
  amountOff?: number;
  percentOff?: number;
  promotionCode: string;
  couponId: string;
}

export class Cart implements CartType {
  id = nanoid();
  items: CartItem[] = [];
  clientSecret?: string;
  paymentIntentId?: string;
  deliverMethodId?: number;
  coupon?: Coupon;
}
