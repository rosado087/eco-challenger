import { Component, inject } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import {
    heroAtSymbol,
    heroUserCircle,
    heroKey
} from '@ng-icons/heroicons/outline'
import { ButtonComponent } from '../../components/button/button.component'
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { NetApiService } from '../../services/net-api/net-api.service'
import { SuccessModel } from '../../models/success-model'
import {
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators
} from '@angular/forms'

@Component({
    selector: 'app-forgot-password',
    imports: [NgIcon, ButtonComponent, ReactiveFormsModule],
    providers: [
        provideIcons({ heroAtSymbol, heroUserCircle, heroKey }),
        PopupLoaderService
    ],
    templateUrl: './forgot-password.component.html',
    styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)
    disableSubmit = false

    emailForm = new FormGroup({
        email: new FormControl('', [Validators.required, Validators.email])
    })

    getEmail() {
        return this.emailForm.get('email')
    }

    sendEmail() {
        this.emailForm.markAllAsTouched()
        if (!this.emailForm.valid) return

        this.disableSubmit = true
        this.netApi
            .post<SuccessModel>('RecoverPassword', 'SendRecoveryEmail', {
                Email: this.emailForm.get('email')?.value
            })
            .subscribe({
                next: () => {
                    // Always send this, even if the account doesn't exist
                    this.popupLoader.showPopup(
                        'Recuperação de Palavra-Passe',
                        'Foi feito o envio de um email com instruções de recuperação de password!'
                    )

                    this.disableSubmit = false
                },
                error: () =>
                    this.popupLoader.showPopup(
                        'Whops :(',
                        'Ocorreu um erro desconhecido ao enviar o email.'
                    )
            })
    }
}
