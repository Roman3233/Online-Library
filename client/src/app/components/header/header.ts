import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-header',
  imports: [RouterLink, FormsModule],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  public authService = inject(AuthService);
  private router = inject(Router);
  searchQuery = '';

  onLogout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
  onSearch() {
    this.router.navigate(['/'], {
      queryParams: { search: this.searchQuery || null }
    });
  }
}
