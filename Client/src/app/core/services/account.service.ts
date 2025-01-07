import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { Address } from '../../shared/models/address';
import { map, pipe, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.baseUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);

    return this.http.post<User>(this.baseUrl + '/login', values, { params });
  }

  register(values: any) {
    return this.http.post(this.baseUrl + '/account/register', values);
  }

  getUserInfo() {
    return this.http.get<User>(this.baseUrl + '/account/user-info').pipe(
      map((user) => {
        this.currentUser.set(user);
        return user;
      })
    );
  }

  logout() {
    return this.http.post(this.baseUrl + '/account/logout', {});
  }

  updateAddress(address: Address) {
    return this.http.post(this.baseUrl + '/account/address', address).pipe(
      tap(() => {
        this.currentUser.update((prevUser) => {
          if (prevUser) prevUser.address = address;
          return prevUser;
        });
      })
    );
  }

  getAuthState() {
    return this.http.get<{ isAuthenticated: boolean }>(
      this.baseUrl + '/account/auth-status'
    );
  }
}
