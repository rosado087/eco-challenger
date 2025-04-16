/* eslint-disable @typescript-eslint/no-explicit-any */

import { Component, inject, OnInit } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import { heroUser, heroLockClosed } from '@ng-icons/heroicons/outline'
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { NetApiService } from '../../services/net-api/net-api.service'
import {
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators
} from '@angular/forms'
import { Router, RouterLink } from '@angular/router'
import { AuthService } from '../../services/auth/auth.service'
import { LoginResponseModel } from '../../models/login-response-model'
import { GAuthLoginModel } from '../../models/gauth-login'

/*
  This google variable is filled through a script tag that is placed
  in index.html and must be here, from my understanding this is an awful hack
  since google GIS doesn't have any proper packages to implement this.
*/
/* eslint-disable no-var */
declare var google: any

@Component({
    selector: 'app-login',
    imports: [NgIcon, ReactiveFormsModule, RouterLink],
    providers: [provideIcons({ heroUser, heroLockClosed }), PopupLoaderService],
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
        password: new FormControl('', [
            Validators.required,
            Validators.minLength(6)
        ])
    })

    ngOnInit(): void {
        this.netApi.get<GAuthLoginModel>('Login', 'GetGoogleId').subscribe({
            next: (data) => {
                google.accounts.id.initialize({
                    client_id: data.clientId,
                    callback: (resp: unknown) => this.handleLoginGoogle(resp)
                })

                google.accounts.id.renderButton(
                    document.getElementById('google-btn'),
                    {
                        theme: 'filled_green',
                        size: 'large',
                        shape: 'rectangle'
                    }
                )
            },
            error: () => {
                this.popupLoader.showPopup(
                    'Erro',
                    'Ocorreu um erro ao inicializar o serviço de autenticação google.'
                )
            }
        })
    }

    loginUser() {
        this.loginForm.markAllAsTouched()

        if (!this.loginForm.valid)
            return this.popupLoader.showPopup(
                'Erro',
                'Por favor, preencha todos os campos corretamente.'
            )

        const params = {
            Email: this.loginForm.get('email')?.value,
            Password: this.loginForm.get('password')?.value
        }

        this.disableSubmit = true

        this.netApi
            .post<LoginResponseModel>('Login', 'Login', params)
            .subscribe({
                next: (response) => {
                    if (response.success) {
                        // Init user auth service
                        this.authService.login(response.user, response.token)
                      this.router.navigate(['/challenges/' + this.authService.getUserInfo().id.toString()])
                        return
                    }

                    this.popupLoader.showPopup(
                        'Erro ao fazer login',
                        response.message ||
                            'Ocorreu um erro ao tentar fazer login.'
                    )

                    this.disableSubmit = false
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro ao fazer login',
                        'Ocorreu um erro ao tentar fazer login. Por favor, tente novamente mais tarde.'
                    )
                    this.disableSubmit = false
                }
            })
    }

    handleLoginGoogle(response: any) {
        if (!response) return
        const info = JSON.parse(atob(response.credential.split('.')[1]))

        // Handle post GAuth, create user and get JWT
        this.netApi
            .post<LoginResponseModel>('Login', 'AuthenticateGoogle', {
                GoogleToken: info.sub,
                Email: info.email
            })
            .subscribe({
                next: (data) => {
                    if (data.success) {
                        this.authService.login(data.user, data.token)
                        this.router.navigate(['/'])
                        return
                    }

                    this.popupLoader.showPopup(
                        'Erro',
                        data.message ||
                            'Houve um problema ao autenticar com conta Google.'
                    )
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
