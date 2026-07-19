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
