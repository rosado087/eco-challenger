import { Component, inject, OnInit } from '@angular/core';
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { Router } from '@angular/router'
import { NetApiService } from '../../services/net-api/net-api.service'
import { AuthService } from '../../services/auth/auth.service'
import { ChallengeListModel } from '../../models/gamification-model';
import { ChallengeCardComponent } from '../../components/challenge-card/challenge-card.component';
import { ChallengeModel } from '../../models/challenge-model';
import { SuccessModel } from '../../models/success-model';

@Component({
  selector: 'app-challenges',
  imports: [
    ChallengeCardComponent,
  ],
  templateUrl: './challenges.component.html',
  styleUrl: './challenges.component.css',
  providers: [
          PopupLoaderService
      ]
})
export class ChallengesComponent implements OnInit {
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)
    router = inject(Router)
    authService = inject(AuthService)
    dailyChallenges: { challenge: ChallengeModel, wasConcluded: boolean }[] = []
    weeklyChallenges: {challenge: ChallengeModel, progress: number, wasConcluded: boolean}[] = []
    selectedChallenge: ChallengeModel | null = null

    ngOnInit(): void {
        this.getChallenges()
    }

    getChallenges() {
        this.netApi
        .get<ChallengeListModel>('Gamification', 'GetChallenges')
        .subscribe({
            next: (data) => {
                if (data.success) {
                    this.dailyChallenges = data.dailyChallenges
                    this.weeklyChallenges = data.weeklyChallenges
                } else {
                    this.popupLoader.showPopup(
                        'Erro',
                        data.message || 'Ocorreu um erro!'
                    )
                }
            },
            error: () => {
                this.popupLoader.showPopup(
                    'Erro',
                    'Não foi possível carregar os dados do perfil.'
                )
            }
        })
    }

    completeChallenge (id: number) {
        this.netApi
        .get<SuccessModel>('Gamification', 'CompleteChallenge', undefined, id.toString())
        .subscribe({
            next: (data) => {
                if (data.success) {
                    this.popupLoader.showPopup(
                        'Sucesso',
                        data.message || 'Concluído com sucesso'
                    )
                    this.getChallenges()
                } else {
                    this.popupLoader.showPopup(
                        'Erro',
                        data.message || 'Ocorreu um erro!'
                    )
                }
            },
            error: () => {
                this.popupLoader.showPopup(
                    'Erro',
                    'Não foi possível concluir o desafio.'
                )
            }
        })
    }

    addProgress (id: number) {
        this.netApi
        .get<SuccessModel>('Gamification', 'AddProgress', undefined, id.toString())
        .subscribe({
            next: (data) => {
                if (data.success) {
                    this.popupLoader.showPopup(
                        'Sucesso',
                        data.message || 'Progresso adicionado com sucesso!'
                    )
                    this.getChallenges()
                } else {
                    this.popupLoader.showPopup(
                        'Erro',
                        data.message || 'Ocorreu um erro!'
                    )
                }
            },
            error: () => {
                this.popupLoader.showPopup(
                    'Erro',
                    'Não foi possível adicionar progresso ao desafio.'
                )
            }
        })
    }

}
