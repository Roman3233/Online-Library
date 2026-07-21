import { Component, inject } from '@angular/core';
import { ToastService, Toast } from '../../services/toast';

@Component({
  selector: 'app-toasts',
  imports: [],
  templateUrl: './toasts.html',
  styleUrl: './toasts.css',
})
export class Toasts {
  toastService = inject(ToastService);
  toasts = this.toastService.toasts;
}
