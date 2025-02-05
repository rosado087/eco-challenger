import { Component, inject } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import {
    heroAtSymbol,
    heroUser,
    heroKey
} from '@ng-icons/heroicons/outline'
import { ButtonComponent } from '../../components/button/button.component'
import { PopupLoaderService } from '../../services/popup-loader.service'
import { NetApiService } from '../../services/net-api.service'
import { SuccessModel } from '../../models/success-model'
import { ActivatedRoute, Router } from '@angular/router'
import { PopupButton } from '../../models/popup-button'
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms'


@Component({
  selector: 'app-register',
  imports: [NgIcon, ButtonComponent, ReactiveFormsModule],
  providers: [
    provideIcons({ heroAtSymbol, heroUser, heroKey }),
    PopupLoaderService
],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  router = inject(Router)
  route = inject(ActivatedRoute)
  disableSubmit = false

  registerForm = new FormGroup({
    userName: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  })

  getUserName(){
    return this.registerForm.get('userName')
  }

  getEmail(){
    return this.registerForm.get('email');
  }

  getPassword(){
    return this.registerForm.get('password');
  }

  registerAccount(){
    this.registerForm.markAllAsTouched()
    if (!this.registerForm.valid) return

    const params = {
      UserName: this.getUserName()?.value,
      Email: this.getEmail()?.value,
      Password: this.getPassword()?.value
    }

    this.disableSubmit = true
    this.netApi
    .post<SuccessModel>('Register', 'RegisterAccount', params)
      .subscribe({
          next: (response) => {
            const popupButton: PopupButton[] = [
              {
                type: 'ok',
                text: 'Okay',
                callback: () => this.router.navigate(['/'])
              }
            ]

            if(response.success){
              this.popupLoader.showPopup(
                'Registo Bem-Sucedido',
                'Você criou uma conta com sucesso!',
                popupButton
              )
              this.disableSubmit = false
            }else{
              this.popupLoader.showPopup(
                'Erro ao registar a conta',
                response.message || 'Ocorreu um erro!',
              )
            }
            this.disableSubmit = true
            
          },
          error: () =>{          
          this.popupLoader.showPopup(
            'Erro ao registar a conta',
            'O email já está a ser utilizado.'
          )
          this.disableSubmit = true
        }
      })
  }
}
