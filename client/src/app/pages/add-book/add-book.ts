import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookService, Book } from '../../services/book';
import { Router } from '@angular/router';
import { ToastService } from '../../services/toast';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-add-book',
  imports: [FormsModule],
  templateUrl: './add-book.html',
  styleUrl: './add-book.css',
})
export class AddBook {
  private bookService = inject(BookService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  isLoading = signal(false);
  title = '';
  description = '';
  author = '';
  selectedFile: File | null = null;
  coverFile: File | null = null;

  getFile(event: Event): File | null {
    const input = event.target as HTMLInputElement;
    return input.files?.[0] || null;
  }

  onSubmit() {
    if (this.isLoading()) return;

    if (!this.selectedFile) {
      this.toastService.showError('Please choose a PDF file.');
      return;
    }

    this.isLoading.set(true);
    this.bookService.createBook(this.title, this.description, this.author, this.selectedFile, this.coverFile)
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.toastService.showSuccess('Book created successfully');
          this.router.navigate(['/book/' + data.id]);
        }
      });
  }
}
