import { Routes } from '@angular/router';
import { BookList } from './pages/book-list/book-list';
import { BookDetail } from './pages/book-detail/book-detail';
import { Login } from './pages/login/login';

export const routes: Routes = [
    { path: '', component: BookList },
    { path: 'book/:id', component: BookDetail },
    { path: 'login', component: Login }
];
