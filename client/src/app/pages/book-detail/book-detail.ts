import { Component, signal, inject } from '@angular/core';
import { BookService, Book } from '../../services/book';
import { ActivatedRoute } from '@angular/router';
import { Comment } from '../../services/book';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { User } from '../../services/user';
import { ToastService } from '../../services/toast';

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
  private toastService = inject(ToastService);
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
      }
    });

  }
  loadComments() {
    this.bookService.getAllComments(this.book()!.id).subscribe({

      next: (data) => {
        this.comments.set(data);
      }
    });
  }
  onAddComment() {
    if (this.newComment) {
      this.bookService.addComment(this.newComment, this.book()!.id).subscribe({
        next: () => {
          this.newComment = '';
          this.loadComments();
          this.toastService.showSuccess('Comment added successfully');
        }
      });
    }
  }

  onDeleteComment(commentId: number) {
    this.bookService.deleteComment(commentId).subscribe({
      next: () => {
        this.loadComments();
        this.toastService.showSuccess('Comment deleted successfully');
      }
    });
  }
  onUpdateBook(id: number) {
    this.bookService.updateBook(id, this.editTitle).subscribe({
      next: () => {
        this.loadComments();
        this.book.set({ ...this.book()!, title: this.editTitle });
        this.isEditing.set(false);
        this.router.navigate(['/book/', id]);
        this.toastService.showSuccess('Book updated successfully');
      }
    });
  }
  toggleEdit() {
    this.isEditing.set(!this.isEditing());
  }

  onDeleteBook(id: number) {
    this.bookService.deleteBook(id).subscribe({
      next: () => {
        this.router.navigate(['/']);
        this.toastService.showSuccess('Book deleted successfully');
      }
    });
  }
  onLike(id: number) {
    this.bookService.likeBook(id).subscribe({
      next: (data) => {
        this.book.set({ ...this.book()!, hasLiked: data.hasLiked, likeCount: data.likeCount });
        this.toastService.showSuccess(data.hasLiked ? 'Book liked successfully' : 'Book unliked successfully');
      }
    });
  }
}


