import { Routes } from '@angular/router';
import { BookList } from './pages/book-list/book-list';
import { BookDetail } from './pages/book-detail/book-detail';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { AddBook } from './pages/add-book/add-book';
import { Profile } from './pages/profile/profile';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
    { path: '', component: BookList },
    { path: 'book/:id', component: BookDetail },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'add-book', component: AddBook, canActivate: [authGuard] },
    { path: 'profile/:id', component: Profile, canActivate: [authGuard] }
];
