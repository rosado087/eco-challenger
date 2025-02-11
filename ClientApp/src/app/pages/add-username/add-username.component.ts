import { Component, inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NetApiService } from '../../services/net-api.service';
import { ButtonComponent } from '../../components/button/button.component';
import { Router } from '@angular/router';
import { PopupLoaderService } from '../../services/popup-loader.service';

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
  router = inject(Router)
  popupLoader = inject(PopupLoaderService)

  usernameForm = new FormGroup({
    username: new FormControl('', [
      Validators.required,
      
    ])
  })

  getUsername() {
    return this.usernameForm?.get('username');
  }

  userExists() {


    if (!this.getUsername()?.value) return true

    return (control: AbstractControl): ValidationErrors | null => {
      this.netApi.post<Result>('Login', 'UserExists', [this.getUsername()?.value]).subscribe({

        next: (data) => {

          return data.success
          
        },
        error: () =>
          this.popupLoader.showPopup(
            'Whops',
            'Houve um problema a comparar nomes de utilizador.'

          )
      });

      return null;
    }
  }

  addUser() {
    if (!this.userExists()) {
      var data = JSON.parse(`${sessionStorage.getItem("loggedInUser")}`);

      this.netApi.put<Result>('Login', 'SignUpGoogle', [this.getUsername()?.value, data.email, data.sub]).subscribe({

        next: (data) => {

          if (data.success) this.router.navigate(['main-page']);


        },
        error: () =>

          this.popupLoader.showPopup(
            'Whops',
            'Houve um problema a adicionar o utilizador.'

          )
      });
    } else {
      this.popupLoader.showPopup(
        'Whops',
        'Este utilizador já está a ser utilizado.'

      )
    }
  }
}
