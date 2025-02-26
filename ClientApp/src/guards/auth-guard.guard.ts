import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../app/services/auth.service';


export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const router: Router = inject(Router);
  const authService: AuthService = inject(AuthService);
  const protectedRoutes: string[] = ['/']; // Rotas protegidas

  if (protectedRoutes.includes(state.url) && !authService.isLoggedIn) {
    router.navigate(['/login']); 
    return false;
  }
  
  return true;
};