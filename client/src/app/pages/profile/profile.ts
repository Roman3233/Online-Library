import { Component, inject, signal } from '@angular/core';
import { UserService, User } from '../../services/user';
import { BookService, Book } from '../../services/book';
import { AuthService } from '../../services/auth';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-profile',
  imports: [FormsModule, RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  private userService = inject(UserService);
  private bookService = inject(BookService);
  public authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toastService = inject(ToastService);
  isLoading = signal(false);
  user = signal<User | null>(null);
  books = signal<Book[]>([]);
  errorMessage = signal('');
  editUsername = '';
  isEditing = signal(false);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.isLoading.set(true);
    if (id) {
      this.userService.getProfile(+id).subscribe({
        next: (data) => {
          this.user.set(data);
          this.editUsername = data.username;
          this.bookService.getBooksByUserId(+id).subscribe({
            next: (data) => {
              this.books.set(data);
              this.isLoading.set(false);
            }
          });
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
          this.toastService.showSuccess('Profile deleted successfully');
        }
      });
    }
  }

  toggleEdit() {
    this.isEditing.set(!this.isEditing());
  }

  onEdit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.userService.updateProfile(+id, this.editUsername).subscribe({
        next: () => {
          this.user.set({ ...this.user()!, username: this.editUsername });
          this.isEditing.set(false);
          this.router.navigate(['/profile/' + id]);
          this.toastService.showSuccess('Profile updated successfully');
        }
      });
    }
  }
  onLogout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
