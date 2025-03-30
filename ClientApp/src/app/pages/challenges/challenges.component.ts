import { Component, inject, OnInit } from '@angular/core';
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { Router } from '@angular/router'
import { NetApiService } from '../../services/net-api/net-api.service'
import { AuthService } from '../../services/auth/auth.service'
import { ChallengeListModel } from '../../models/gamification-model';
import { ChallengeCardComponent } from '../../components/challenge-card/challenge-card.component';
import { ChallengeModel } from '../../models/challenge-model';

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
                             console.log(this.dailyChallenges)
                             console.log(this.weeklyChallenges)
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

  completeChallenge() {
    this.netApi
        .post<ChallengeListModel>('Gamification', 'GetChallenges', 1)
        .subscribe({
                    next: (data) => {
                        if (data.success) {
                          this.popupLoader.showPopup(
                            'Sucesso',
                            data.message || 'Concluído com sucesso'
                        )
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
}
