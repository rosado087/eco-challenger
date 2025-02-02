import { Component, inject } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import { heroUser, heroLockClosed } from '@ng-icons/heroicons/outline'
import { ButtonComponent } from '../../components/button/button.component'
import { PopupLoaderService } from '../../services/popup-loader.service'
import { NetApiService } from '../../services/net-api.service'
import { SuccessModel } from '../../models/success-model'
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms'
import { Router } from '@angular/router' // Import Router

@Component({
    selector: 'app-login',
    imports: [NgIcon, ButtonComponent, ReactiveFormsModule],
    providers: [
        provideIcons({ heroUser, heroLockClosed }),
        PopupLoaderService
    ],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css'
})
export class LoginComponent {
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)
    router = inject(Router) // Inject the Router service
    disableSubmit = false

    loginForm = new FormGroup({
        email: new FormControl('', [Validators.required, Validators.email]),
        password: new FormControl('', [Validators.required, Validators.minLength(6)])
    })

    getEmail() {
        return this.loginForm.get('email')
    }

    getPassword() {
        return this.loginForm.get('password')
    }

    loginUser() {
      this.loginForm.markAllAsTouched();
      if (!this.loginForm.valid) return;

      this.disableSubmit = true;
      this.netApi.post<SuccessModel>('Login', 'Authenticate', {
        Email: this.getEmail()?.value,
        Password: this.getPassword()?.value
    })

          .subscribe({
              next: () => {
                  this.popupLoader.showPopup(
                      'Login Bem-Sucedido',
                      'Você foi autenticado com sucesso!'
                  );
                  this.disableSubmit = false;

                  // Redirect to the Home Page
                  this.router.navigate(['/']);
              },
              error: () => {
                  this.popupLoader.showPopup(
                      'Erro de Autenticação',
                      'Credenciais inválidas ou erro no servidor.'
                  );
                  this.disableSubmit = false;
              }
          });
  }


}
