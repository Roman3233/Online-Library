import { Component, inject, signal } from '@angular/core';
import { Auth } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private authService = inject(Auth);
  private router = inject(Router);
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
