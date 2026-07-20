import { Component, signal, inject } from '@angular/core';
import { BookService, Book } from '../../services/book';
import { ActivatedRoute } from '@angular/router';
import { Comment } from '../../services/book';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { User } from '../../services/user';

@Component({
  selector: 'app-book-detail',
  imports: [FormsModule, RouterLink],
  templateUrl: './book-detail.html',
  styleUrl: './book-detail.css',
})
export class BookDetail {
  private route = inject(ActivatedRoute);
  private bookService = inject(BookService);
  public authService = inject(AuthService);
  private router = inject(Router);
  isLoading = signal(false);
  book = signal<Book | null>(null);
  comments = signal<Comment[]>([]);
  newComment = '';
  editTitle = '';
  user = signal<User | null>(null);
  isEditing = signal(false);


  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.isLoading.set(true);
    this.bookService.getBook(+id!).subscribe({
      next: (data) => {
        this.book.set(data);
        this.editTitle = data.title;
        this.loadComments();
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

  onDownload() {
    const book = this.book();
    if (!book) return;
    this.bookService.downloadFile(book.id).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob); // creating a temporary url for the blob
        const a = document.createElement('a'); // creating an anchor tag
        a.href = url;
        a.download = this.book()!.title; // setting the download attribute to the book title
        a.click(); // triggering the download
        URL.revokeObjectURL(url); // revoking the temporary url
      },
      error: (err) => console.log(err)
    });

  }
  loadComments() {
    this.bookService.getAllComments(this.book()!.id).subscribe({

      next: (data) => {
        this.comments.set(data);
        console.log('comments:', data);
        console.log('currentUserId:', this.authService.getCurrentUserId());
      },
      error: (err) => console.log(err)
    });
  }
  onAddComment() {
    if (this.newComment) {
      this.bookService.addComment(this.newComment, this.book()!.id).subscribe({
        next: () => {
          this.newComment = '';
          this.loadComments();
        },
        error: (err) => console.log(err)
      });
    }
  }

  onDeleteComment(commentId: number) {
    this.bookService.deleteComment(commentId).subscribe({
      next: () => {
        this.loadComments();
      },
      error: (err) => console.log(err)
    });
  }
  onUpdateBook(id: number) {
    this.bookService.updateBook(id, this.editTitle).subscribe({
      next: () => {
        this.loadComments();
        this.book.set({ ...this.book()!, title: this.editTitle });
        this.isEditing.set(false);
        this.router.navigate(['/book/', id]);
      },
      error: (err) => console.log(err)
    });
  }
  toggleEdit() {
    this.isEditing.set(!this.isEditing());
  }
}


