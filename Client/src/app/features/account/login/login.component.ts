import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AccountService } from '../../../core/services/account.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatInput,
    MatLabel,
    MatButton,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private activatedROute = inject(ActivatedRoute);
  returnUrl = '/shop';

  constructor() {
    const url = this.activatedROute.snapshot.queryParams['returnUrl'];
    if (url) this.returnUrl = url;
  }

  private router = inject(Router);
  loginForm = this.fb.group({
    email: ['johdoe@test.com', Validators.required],
    password: ['Pa$$w0rd', Validators.required],
  });

  ngOnInit(): void {
    // this.loginForm();
  }

  onSubmit() {
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => {
        this.accountService.getUserInfo().subscribe();
        this.router.navigateByUrl(this.returnUrl);
      },
    });
  }
}
