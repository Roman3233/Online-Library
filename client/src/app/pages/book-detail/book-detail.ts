import { Component, signal, inject } from '@angular/core';
import { BookService, Book } from '../../services/book';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-book-detail',
  imports: [],
  templateUrl: './book-detail.html',
  styleUrl: './book-detail.css',
})
export class BookDetail {
  private route = inject(ActivatedRoute);
  private bookService = inject(BookService);

  isLoading = signal(false);
  book = signal<Book | null>(null);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.isLoading.set(true);
    this.bookService.getBook(+id!).subscribe({
      next: (data) => {
        this.book.set(data);
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
