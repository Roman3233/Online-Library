import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5164/api/auth';

  register(email: string, username: string, password: string) {
    return this.http.post(this.apiUrl + '/register', { email, username, password });
  }

  login(email: string, password: string) {
    return this.http.post<{ token: string }>(this.apiUrl + '/login', { email, password });
  }

  getCurrentUserId(): number | null {
    const token = localStorage.getItem('token');
    if (!token) return null;
    const decoded: any = jwtDecode(token);
    return Number(decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']);
  }
}
