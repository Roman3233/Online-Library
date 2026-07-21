import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastService } from '../../services/toast';


@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private authService = inject(AuthService);
  private router = inject(Router);
  private toastService = inject(ToastService);
  isLoading = signal(false);
  username = '';
  email = '';
  password = '';

  onRegister() {
    this.isLoading.set(true);
    this.authService.register(this.email, this.username, this.password).subscribe({
      next: (data) => {
        this.router.navigate(['/login']);
        this.isLoading.set(false);
        this.toastService.showSuccess('User registered successfully');
      }
    })
  }
}
