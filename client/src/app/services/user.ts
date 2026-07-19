import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5164/api/user';

  getMyProfile() {
    return this.http.get<User>(this.apiUrl + '/me');
  }
  getProfile(id: number) {
    return this.http.get<User>(this.apiUrl + '/' + id);
  }
}

export interface User {
  id: number;
  username: string;
  email: string;
  password: string;
  role: string;
  registeredAt: Date;
}
