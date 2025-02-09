import { Component, inject } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import { heroUser, heroLockClosed } from '@ng-icons/heroicons/outline'
import { PopupLoaderService } from '../../services/popup-loader.service'
import { NetApiService } from '../../services/net-api.service'
import { SuccessModel } from '../../models/success-model'
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms'
import { Router, RouterLink } from '@angular/router'
import { PopupButton } from '../../models/popup-button'
import { Result } from '../home/home.component'

@Component({
  selector: 'app-login',
  imports: [NgIcon, ReactiveFormsModule, RouterLink],
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
  router = inject(Router)
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
  // Mark all form controls as touched to trigger validation messages
  this.loginForm.markAllAsTouched();

  // Check if the form is valid
  if (!this.loginForm.valid) {
    this.popupLoader.showPopup('Erro', 'Por favor, preencha todos os campos corretamente.');
    return;
  }

  // Prepare the login parameters
  const params = {
    Email: this.getEmail()?.value,
    Password: this.getPassword()?.value
  };

  // Disable the submit button to prevent multiple submissions
  this.disableSubmit = true;

  // Make the API request to log in
  this.netApi.post<SuccessModel>('Login', 'Login', params).subscribe({
    next: (response) => {
      if (response.success) {
        // Define the popup button for successful login
        const popupButton: PopupButton[] = [
          {
            type: 'ok',
            text: 'Okay',
            callback: () => this.router.navigate(['/']) // Navigate to the home page
          }
        ];

        // Show success popup
        this.popupLoader.showPopup(
          'Login Bem-Sucedido',
          'Você entrou na sua conta com sucesso!',
          popupButton
        );
      } else {
        // Show error popup if login fails
        this.popupLoader.showPopup(
          'Erro ao fazer login',
           'Email ou senha incorretos.'
        );
      }

      // Re-enable the submit button
      this.disableSubmit = false;
    },
    error: () => {
      // Log the error for debugging
      //console.error('Login error:', error);

      // Show a generic error popup
      this.popupLoader.showPopup(
        'Erro ao fazer login',
        'Ocorreu um erro ao tentar fazer login. Por favor, tente novamente mais tarde.'
      );

      // Re-enable the submit button
      this.disableSubmit = false;
    }
  });
}

  handleLoginGoogle(response: any) {
    if (response) {
      const info = JSON.parse(atob(response.credential.split(".")[1]));
      sessionStorage.setItem("loggedInUser", JSON.stringify(info));

      this.netApi.post<Result>('Login', 'AuthenticateGoogle', [info.sub, info.email]).subscribe({
        next: (data) => {
          if (data.success) this.router.navigate(['/']);
          else {
            this.router.navigate(['add-username']);
          }
        },
        error: () => {
          this.popupLoader.showPopup(
            'Erro',
            'Houve um problema ao autenticar com conta Google.'

          )
        }
      })

    }
  }

}
