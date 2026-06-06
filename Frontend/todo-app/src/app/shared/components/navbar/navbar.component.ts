import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {
  @Output() menuToggle = new EventEmitter<void>();
  constructor(public authService: AuthService) { }

  logout(): void {
    this.authService.logout();
  }
}