import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);
  private toastService = inject(ToastService);
  isLoading = signal(false);
  email = '';
  password = '';

  onLogin() {
    this.isLoading.set(true);
    this.authService.login(this.email, this.password).subscribe({
      next: (data) => {
        localStorage.setItem('token', data.token);
        this.router.navigate(['/']);
        this.isLoading.set(false);
        this.toastService.showSuccess('Login successful');
      }
    })
  }
}
