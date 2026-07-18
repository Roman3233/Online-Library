import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookService, Book } from '../../services/book';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-book',
  imports: [FormsModule],
  templateUrl: './add-book.html',
  styleUrl: './add-book.css',
})
export class AddBook {
  private bookService = inject(BookService);
  title = '';
  selectedFile: File | null = null;


}
