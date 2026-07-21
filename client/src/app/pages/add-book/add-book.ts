import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookService, Book } from '../../services/book';
import { Router } from '@angular/router';
import { ToastService } from '../../services/toast';

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
  title = '';
  description = '';
  author = '';
  selectedFile: File | null = null;

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.selectedFile = input.files[0];
    }
  }

  onSubmit() {
    this.bookService.createBook(this.title, this.description, this.author).subscribe({
      next: (data) => {
        this.bookService.uploadFile(data.id, this.selectedFile!).subscribe({
          next: () => {
            this.toastService.showSuccess('Book created successfully');
            this.router.navigate(['/book/' + data.id]);
          }
        });
      }
    })
  }
}
