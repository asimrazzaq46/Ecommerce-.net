import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../../core/services/account.service';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MatCard } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { Router } from '@angular/router';
import { SnackBarService } from '../../../core/services/snack-bar.service';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatButton,
    TextInputComponent,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService);
  private snackBar = inject(SnackBarService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  validationErrors?: string[];
  registerForm: FormGroup = new FormGroup({});

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.minLength(6),
          Validators.maxLength(28),
          Validators.required,
        ],
      ],
      confirmPassword: ['', [Validators.required, this.matchValue('password')]],
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: (_) =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity,
    });
  }

  matchValue(passwordValue: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(passwordValue)?.value
        ? null
        : { isMatching: true };
    };
  }

  onSubmit() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: (_) => {
        this.snackBar.success('Registeration Successfull - you can now login');
        this.router.navigateByUrl('/account/login');
      },
      error: (err) => (this.validationErrors = err),
    });
  }
}
