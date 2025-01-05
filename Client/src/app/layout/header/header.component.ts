import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatBadge } from '@angular/material/badge';
import {
  MatMenuContent,
  MatMenuModule,
  MatMenuTrigger,
} from '@angular/material/menu';
import { MatProgressBar } from '@angular/material/progress-bar';
import { BusyService } from '../../core/services/busy.service';
import { CartService } from '../../core/services/cart.service';
import { AccountService } from '../../core/services/account.service';
import { MatDivider } from '@angular/material/divider';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    MatIcon,
    MatButton,
    MatBadge,
    MatProgressBar,
    MatMenuTrigger,
    MatDivider,
    MatMenuModule,
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  busyService = inject(BusyService);
  cartService = inject(CartService);
  accountService = inject(AccountService);
  user = this.accountService.currentUser();
  router = inject(Router);

  logout() {
    this.accountService.logout().subscribe({
      next: (_) => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/');
      },
    });
  }
}
