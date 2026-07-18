import { Component, inject, signal } from '@angular/core';
import { Auth } from '../../services/auth';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private authService = inject(Auth);
  isLoading = signal(false);
  email = '';
  password = '';

  onLogin() {
    this.isLoading.set(true);
    this.authService.login(this.email, this.password).subscribe({
      next: (data) => {
        localStorage.setItem('token', data.token);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.log(err);
        this.isLoading.set(false);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    })
  }
}
