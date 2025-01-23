import { Component, inject, OnInit } from '@angular/core'
import { PopupLoaderService } from '../../services/popup-loader.service'
import { NgIcon, provideIcons } from '@ng-icons/core'
import { ButtonComponent } from '../../components/button/button.component'
import { heroKey } from '@ng-icons/heroicons/outline'
import { ActivatedRoute, Router } from '@angular/router'
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms'
import { NetApiService } from '../../services/net-api.service'
import { SuccessModel } from '../../models/success-model'

@Component({
    selector: 'app-reset-password',
    imports: [NgIcon, ButtonComponent, ReactiveFormsModule],
    providers: [provideIcons({ heroKey }), PopupLoaderService],
    templateUrl: './reset-password.component.html',
    styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
    netApi = inject(NetApiService)
    router = inject(Router)
    route = inject(ActivatedRoute)
    popupLoader = inject(PopupLoaderService)
    resetToken: string | null = null

    passwordForm = new FormGroup({
        newPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
        confirmPassword: new FormControl('', [
            Validators.required, Validators.minLength(8), this.matchPasswordsValidator()])
    })

    // This is a custom validator to make sure the passwords match
    matchPasswordsValidator(): ValidatorFn {        
        return (control: AbstractControl): ValidationErrors | null => {    
            const match = (this.getNewPassword()?.value === control.value)

            return !match ? { matchPasswords: true } : null
        }
    }

    getNewPassword() {
        return this.passwordForm?.get('newPassword')
    }

    getConfirmPassword() {
        return this.passwordForm?.get('confirmPassword')
    }

    resetPassword() {
        this.passwordForm.markAllAsTouched()
        if(!this.passwordForm.valid) return

        const params = { 
            Password: this.getNewPassword()?.value, 
            Token: this.resetToken
        }

        this.netApi.post<SuccessModel>('RecoverPassword', 'SetNewPassword', params).subscribe({
            next: (data) => {
                if(data.success)
                    this.popupLoader.showPopup(
                        'Alteração de Palavra-Passe',
                        'A palavra passe foi alterada com sucesso!'
                    )
                else
                    this.popupLoader.showPopup(
                        'Alteração de Palavra-Passe',
                        'Não foi possível efetuar a alteração da palavra-passe!'
                    )
            },
            error: () => this.popupLoader.showPopup(
                'Whops :(',
                'Ocorreu um erro desconhecido ao recuperar a palavra-passe.'
            )
        })
        
    }

    ngOnInit(): void {
        // Here we want to fetch the token param from the route
        // and validate it, when it doesn't exist or is invalid
        // we redirect to the homepage
        this.resetToken = this.route.snapshot.paramMap.get('token')

        if (!this.resetToken) {
            this.router.navigate(['/'])
            return
        }
        
        this.netApi.get<SuccessModel>('RecoverPassword', 'CheckToken', this.resetToken).subscribe({
            next: (data) => {
                if(!data.success) this.router.navigate(['/'])
            },
            error: () => this.router.navigate(['/'])
        })
    }
}
