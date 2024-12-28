import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Product } from '../../shared/models/product.model';
import { Pagination } from '../../shared/models/pagination.model';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  baseUrl = environment.baseUrl;

  private http = inject(HttpClient);

  brands = signal<string[]>([]);

  types = signal<string[]>([]);

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();

    if (shopParams.brands.length > 0) {
      params = params.append('brands', shopParams.brands.join(','));
    }
    if (shopParams.types.length > 0) {
      params = params.append('types', shopParams.types.join(','));
    }
    if (shopParams.sort) {
      params = params.append('sort', shopParams.sort);
    }

    if (shopParams.search) {
      params = params.append('search', shopParams.search);
    }

    params = params.append('pageSize', shopParams.pageSize);
    params = params.append('pageIndex', shopParams.pageNumber);

    return this.http.get<Pagination<Product>>(this.baseUrl + '/products', {
      params,
    });
  }

  getTypes() {
    if (this.types().length > 0) return;

    return this.http.get<string[]>(this.baseUrl + '/products/types').subscribe({
      next: (res) => this.types.set(res),
    });
  }

  getBrands() {
    if (this.brands().length > 0) return;
    return this.http
      .get<string[]>(this.baseUrl + '/products/brands')
      .subscribe({
        next: (res) => this.brands.set(res),
      });
  }
}
