import { Component, inject } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { NetApiService } from '../../services/net-api.service';
import { ButtonComponent } from '../../components/button/button.component';
import { Router } from '@angular/router';

export interface Result {
  success: boolean
}

@Component({
  selector: 'app-add-username',
  imports: [FormsModule, ButtonComponent, ReactiveFormsModule],
  templateUrl: './add-username.component.html',
  styleUrl: './add-username.component.css'
})
export class AddUsernameComponent {
  netApi = inject(NetApiService)
  router = inject(Router)

  usernameForm = new FormGroup({
    username: new FormControl('', [
      Validators.required,
      this.userExists()
    ])
  })

  getUsername() {
    return this.usernameForm.get('username');
  }

  userExists() {
    


    return (control: AbstractControl): ValidationErrors | null => {
      this.netApi.post<Result>('user', 'user-exists', [this.getUsername()?.value]).subscribe({

        next: (data) => {

          return data.success ?{ usernameExists: true } : null
          
        },
        error: () => alert('An error ocurred')
      });

      return null;
    }
  }

  addUser() {
    var data = JSON.parse(`${sessionStorage.getItem("loggedInUser")}`);
    this.netApi.post<Result>('user', 'signup-google', [this.getUsername()?.value, data.email, data.sub]).subscribe({

      next: (data) => {

        if (data.success) this.router.navigate(['main-page']);
        else alert('there was no success')

      },
      error: () => alert('An error ocurred')
    });
  }
}
