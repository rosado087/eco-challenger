import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth/auth.service';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService)
  
  const authReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${authService.getAuthToken()}`
    }
  })

  return next(authReq);
};
