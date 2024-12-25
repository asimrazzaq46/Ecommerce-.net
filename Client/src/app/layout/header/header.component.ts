import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
// import { MatIconModule } from '@angular/material/icon';
// import { MatButtonModule } from '@angular/material/button';
// import { MatBadgeModule } from '@angular/material/badge';

@Component({
    selector: 'app-header',
    imports: [
        RouterLink,
        RouterLinkActive,
    ],
    templateUrl: './header.component.html',
    styleUrl: './header.component.scss'
})
export class HeaderComponent {}
