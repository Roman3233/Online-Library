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
