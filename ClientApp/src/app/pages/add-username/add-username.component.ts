import { Component, inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NetApiService } from '../../services/net-api.service';
import { ButtonComponent } from '../../components/button/button.component';
import { Router } from '@angular/router';
import { PopupLoaderService } from '../../services/popup-loader.service';
import { PopupButton } from '../../models/popup-button';
import { AuthService } from '../../services/auth.service';

export interface Result {
  success: boolean
}

@Component({
  selector: 'app-add-username',
  imports: [FormsModule, ButtonComponent, ReactiveFormsModule],
  providers: [PopupLoaderService],
  templateUrl: './add-username.component.html',
  styleUrl: './add-username.component.css'
})
export class AddUsernameComponent {
  netApi = inject(NetApiService)
  authService = inject(AuthService)
  router = inject(Router)
  popupLoader = inject(PopupLoaderService)

  usernameForm = new FormGroup({
    username: new FormControl('', [
      Validators.required
    ])
  })

  getUsername() {
    return this.usernameForm?.get('username');
  }

  addUser() {

    var data = JSON.parse(`${sessionStorage.getItem("loggedInUser")}`);

    const popupButton: PopupButton[] = [
      {
        type: 'ok',
        text: 'Okay',
        callback: () => {
          const userInfo = {
            Username: this.getUsername()?.value,
            Email: data.email
          };

          this.authService.login(userInfo, "google-token"); // Notify AuthService
          this.router.navigate(['/']);

        }
      }
    ];
    
    
    this.netApi.put<Result>('Login', 'SignUpGoogle', [this.getUsername()?.value, data.email, data.sub]).subscribe({

      next: (data) => {

        if (data.success) {
          this.popupLoader.showPopup('Login', 'Obrigado por se juntar ao Eco Challenger!', popupButton);

        } else
          this.popupLoader.showPopup(
            'Erro',
            'Este username já existe'

          )
      },
      error: () =>

        this.popupLoader.showPopup(
          'Whops',
          'Houve um problema a adicionar o utilizador.'

        )
    });

  }
}
