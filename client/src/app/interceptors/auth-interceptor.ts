import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');

  if (token) {
    const clonned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    })
    return next(clonned);
  }
  return next(req);
};
