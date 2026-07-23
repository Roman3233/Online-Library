import { Component, inject, signal } from '@angular/core';
import { BookService, Book } from '../../services/book';
import { RouterLink } from '@angular/router';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-book-list',
  imports: [RouterLink],
  templateUrl: './book-list.html',
  styleUrl: './book-list.css',
})
export class BookList {
  private bookService = inject(BookService);
  private route = inject(ActivatedRoute);

  isLoading = signal(false);
  books = signal<Book[]>([]);

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const search = params['search'];
      this.isLoading.set(true);
      this.bookService.getAllBooks(search).subscribe({
        next: (data) => {
          this.books.set(data);
          this.isLoading.set(false);
        }
      });
    });
  }
}
