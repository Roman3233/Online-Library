import { Routes } from '@angular/router';
import { BookList } from './pages/book-list/book-list';
import { BookDetail } from './pages/book-detail/book-detail';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { AddBook } from './pages/add-book/add-book';
import { Profile } from './pages/profile/profile';

export const routes: Routes = [
    { path: '', component: BookList },
    { path: 'book/:id', component: BookDetail },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'add-book', component: AddBook },
    { path: 'profile', component: Profile }
];
