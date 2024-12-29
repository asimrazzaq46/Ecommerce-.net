import { Component, inject, signal } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-test-error',
  imports: [MatButton],
  templateUrl: './test-error.component.html',
  styleUrl: './test-error.component.scss',
})
export class TestErrorComponent {
  baseurl = environment.baseUrl;
  private http = inject(HttpClient);
  validationError = signal<string[] | undefined>(undefined);

  get404() {
    this.http.get(this.baseurl + '/buggy/notfound').subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }

  get400BadRequest() {
    this.http.get(this.baseurl + '/buggy/badrequest').subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }

  get400() {
    this.http.post(this.baseurl + '/buggy/validationerror', {}).subscribe({
      next: (res) => console.log(res),
      error: (err) => this.validationError.set(err),
    });
  }

  get401() {
    this.http.get(this.baseurl + '/buggy/unauthorized').subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }

  get500() {
    this.http.get(this.baseurl + '/buggy/internalerror').subscribe({
      next: (res) => console.log(res),
      error: (err) => console.log(err),
    });
  }
}
