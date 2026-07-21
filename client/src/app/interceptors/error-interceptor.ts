import { HttpInterceptorFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ToastService } from '../services/toast';

export const errorInterceptor: HttpInterceptorFn = (req, next): Observable<HttpEvent<unknown>> => {
  const router = inject(Router);
  const toastService = inject(ToastService);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Something went wrong';
      if (error) {
        switch (error.status) {
          case 401:
            localStorage.removeItem('token');
            router.navigate(['/login']);
            errorMessage = 'Please log in to continue.';
            break;
          case 403:
            errorMessage = 'You do not have access to this action.';
            break;
          case 404:
            errorMessage = 'Requested resource not found.';
            break;
          case 409:
            errorMessage = error.error?.message || 'Conflict occurred.';
            break;
          case 400:
            if (error.error && error.error.errors) {
              errorMessage = Object.values(error.error.errors).flat().join('\n');
            } else if (error.error && error.error.detail) {
              errorMessage = error.error.detail;
            } else {
              errorMessage = 'Invalid request.';
            }
            break;
          case 500:
            errorMessage = 'Internal server error.';
            break;
          case 0:
            errorMessage = 'No connection to the server. Check your internet.';
            break;
          default:
            errorMessage = `Error: ${error.error?.message || error.message}`;
            break;
        }
      }
      toastService.showError(errorMessage);
      console.error(errorMessage);
      return throwError(() => error);
    })
  );
};
