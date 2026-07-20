import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class BookService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5164/api';

  getAllBooks() {
    return this.http.get<Book[]>(this.apiUrl + '/books');
  }

  getBook(id: number) {
    return this.http.get<Book>(this.apiUrl + '/books/' + id);
  }

  createBook(title: string) {
    return this.http.post<Book>(this.apiUrl + '/books', { title });
  }

  uploadFile(id: number, file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(this.apiUrl + `/books/${id}/upload`, formData);
  }

  downloadFile(id: number) {
    return this.http.get(this.apiUrl + `/books/${id}/download`, { responseType: 'blob' });
  }
  getAllComments(bookId: number) {
    return this.http.get<Comment[]>(this.apiUrl + `/comments/book/${bookId}`);
  }

  addComment(text: string, bookId: number) {
    return this.http.post<Comment>(this.apiUrl + `/comments`, { text, bookId });
  }
  getBooksByUserId(userId: number) {
    return this.http.get<Book[]>(this.apiUrl + `/books/user/${userId}`);
  }
  deleteComment(commentId: number) {
    return this.http.delete(this.apiUrl + `/comments/${commentId}`);
  }
  updateBook(id: number, title: string) {
    return this.http.put(this.apiUrl + `/books/${id}`, { title });
  }
  deleteBook(id: number) {
    return this.http.delete(this.apiUrl + `/books/${id}`);
  }
}

export interface Book {
  id: number;
  title: string;
  uploadedAt: Date;
  userId: number;
  contentType: string;
  fileName: string;
  filePath: string;
  fileSize: number;
}

export interface Comment {
  id: number;
  text: string;
  createdAt: Date;
  bookId: number;
  username: string;
  userId: number;
}
