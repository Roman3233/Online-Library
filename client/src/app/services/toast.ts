import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  toasts = signal<Toast[]>([]);

  removeToast(id: number) {
    this.toasts.update((toasts) => toasts.filter((toast) => toast.id !== id));
  }

  show(message: string, type: 'success' | 'error' | 'info') {
    const id = Date.now();
    this.toasts.update((toasts) => [...toasts, { id, message, type }]);
    setTimeout(() => this.removeToast(id), 3000);
  }

  showSuccess(message: string) {
    this.show(message, 'success');
  }

  showError(message: string) {
    this.show(message, 'error');
  }

  showInfo(message: string) {
    this.show(message, 'info');
  }
}

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'info';
}
