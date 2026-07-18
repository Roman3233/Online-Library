import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5164/api/auth';

  register(email: string, username: string, password: string) {
    return this.http.post(this.apiUrl + '/register', { email, username, password });
  }

  login(email: string, password: string) {
    return this.http.post<{ token: string }>(this.apiUrl + '/login', { email, password });
  }
}
