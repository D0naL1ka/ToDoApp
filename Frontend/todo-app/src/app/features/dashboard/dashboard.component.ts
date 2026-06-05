import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="d-flex align-items-center justify-content-center min-vh-100">
      <h2>Dashboard</h2>
    </div>
  `
})
export class DashboardComponent {}