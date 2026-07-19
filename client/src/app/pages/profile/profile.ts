import { Component, inject, signal } from '@angular/core';
import { UserService, User } from '../../services/user';
import { BookService, Book } from '../../services/book';
import { AuthService } from '../../services/auth';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  private userService = inject(UserService);
  private bookService = inject(BookService);
  public authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  isLoading = signal(false);
  user = signal<User | null>(null);
  books = signal<Book[]>([]);
  errorMessage = signal('');


  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.isLoading.set(true);
    if (id) {
      this.userService.getProfile(+id).subscribe({
        next: (data) => {
          this.user.set(data);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.log(err);
          this.isLoading.set(false);
        },
        complete: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  onDelete() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.userService.deleteProfile(+id).subscribe({
        next: () => {
          localStorage.removeItem('token');
          this.router.navigate(['/']);
        },
        error: (err) => console.log(err)
      });
    }
  }
}
