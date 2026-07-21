import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './components/header/header';
import { Toasts } from './components/toasts/toasts';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Toasts],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('client');
}
