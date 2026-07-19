import { Component, inject, signal } from '@angular/core';
import { UserService, User } from '../../services/user';
import { BookService, Book } from '../../services/book';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {
  private userService = inject(UserService);
  private bookService = inject(BookService);
  isLoading = signal(false);
  user = signal<User | null>(null);
  books = signal<Book[]>([]);
  errorMessage = signal('');


  ngOnInit() {
    this.isLoading.set(true);
    this.userService.getMyProfile().subscribe({
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
