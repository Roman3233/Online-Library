import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5164/api/users';

  getMyProfile() {
    return this.http.get<User>(this.apiUrl + '/me');
  }
  getProfile(id: number) {
    return this.http.get<User>(this.apiUrl + '/' + id);
  }
  updateProfile(id: number, username: string) {
    return this.http.put<User>(this.apiUrl + '/' + id, { username });
  }
  deleteProfile(id: number) {
    return this.http.delete(this.apiUrl + '/' + id);
  }
}

export interface User {
  id: number;
  username: string;
  email: string;
  password: string;
  role: string;
  registeredAt: Date;
  likedBooksIds: number[];
}
