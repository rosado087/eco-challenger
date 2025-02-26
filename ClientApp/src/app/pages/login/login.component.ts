import { Component, inject, OnInit } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import { heroUser, heroLockClosed } from '@ng-icons/heroicons/outline'
import { PopupLoaderService } from '../../services/popup-loader.service'
import { NetApiService } from '../../services/net-api.service'
import { SuccessModel } from '../../models/success-model'
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms'
import { Router, RouterLink } from '@angular/router'
import { PopupButton } from '../../models/popup-button'
import { AuthService } from '../../services/auth.service';


declare var google: any;

export interface Result {
  success: any
}

interface LoginResponseModel {
  success: boolean;
  message: string;
  token: string;
  username: string;
  email: string;
}


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
export class LoginComponent implements OnInit {

  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  router = inject(Router)
  authService = inject(AuthService)
  disableSubmit = false

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)])
  })

  ngOnInit(): void {

    let value;

    this.netApi.get<Result>('Login', 'GetGoogleId').subscribe({

      next: (data) => {

        google.accounts.id.initialize({
          client_id: data.success,
          callback: (resp: any) => this.handleLoginGoogle(resp)
        });

        google.accounts.id.renderButton(document.getElementById("google-btn"), {
          theme: 'filled_green',
          size: 'large',
          shape: 'rectangle'
        })
      },
      error: () => {
        this.popupLoader.showPopup(
          'Erro',
          'Isto é um problema.'

        )
      }
    });


  }

  getEmail() {
    return this.loginForm.get('email')
  }

  getPassword() {
    return this.loginForm.get('password')
  }

  loginUser() {
    this.loginForm.markAllAsTouched();

    if (!this.loginForm.valid) {
      this.popupLoader.showPopup('Erro', 'Por favor, preencha todos os campos corretamente.');
      return;
    }

    const params = {
      Email: this.getEmail()?.value,
      Password: this.getPassword()?.value
    };

    this.disableSubmit = true;

    this.netApi.post<LoginResponseModel>('Login', 'Login', params).subscribe({
      next: (response) => {
        const popupButton: PopupButton[] = [
          {
            type: 'ok',
            text: 'Okay',
            callback: () => this.router.navigate(['/'])
          }
        ]
        if (response.success) {
          const userInfo = {
            Username: response.username,
            Email: response.email
          };

          this.authService.login(userInfo, response.token); // Use AuthService
          this.popupLoader.showPopup(
            'Login Bem-Sucedido',
            'Entrou na sua conta!',
            popupButton
          )
        } else {
          this.popupLoader.showPopup('Erro ao fazer login', 'Email ou senha incorretos.');
        }

        this.disableSubmit = false;
      },
      error: () => {
        this.popupLoader.showPopup(
          'Erro ao fazer login',
          'Ocorreu um erro ao tentar fazer login. Por favor, tente novamente mais tarde.'
        );
        this.disableSubmit = false;
      }
    });
  }



  handleLoginGoogle(response: any) {
    if (response) {
      const info = JSON.parse(atob(response.credential.split(".")[1]));

      // Store user info in sessionStorage
      sessionStorage.setItem("loggedInUser", JSON.stringify(info));


      // Call API to authenticate Google user
      this.netApi.post<any>('Login', 'AuthenticateGoogle', [info.sub, info.email]).subscribe({
        next: (data) => {
          if (data.success) {
            // Update AuthService so the Header updates
            const userInfo = {
              Username: data.name || info.email.split("@")[0], // Use name if available
              Email: info.email
            };

            this.authService.login(userInfo, "google-token"); // Notify AuthService
            this.router.navigate(['/']);
          } else {
            this.router.navigate(['add-username']);
          }
        },
        error: () => {
          this.popupLoader.showPopup('Erro', 'Houve um problema ao autenticar com conta Google.');
        }
      });
    }
  }


}
