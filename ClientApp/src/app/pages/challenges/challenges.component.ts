import { Component, inject, OnInit } from '@angular/core';
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { NetApiService } from '../../services/net-api/net-api.service'
import { ActivatedRoute, Router } from '@angular/router'
import { AuthService } from '../../services/auth/auth.service'
import { NgFor, NgIf } from '@angular/common'
import { ChallengeModel } from '../../models/challenges-model'
import { ButtonComponent } from '../../components/button/button.component'
import { ChallengeResponse } from '../../models/challenge-response';

@Component({
  selector: 'app-challenges',
  imports: [  
    NgFor,
    NgIf,
    ButtonComponent],
  templateUrl: './challenges.component.html',
  styleUrl: './challenges.component.css',
  providers: [
    PopupLoaderService
  ]
})
export class ChallengesComponent implements OnInit {
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  route = inject(ActivatedRoute)
  authService = inject(AuthService)

  dailyChallengesToDo: ChallengeModel[] = []
  dailyChallengesDone: ChallengeModel[] = []
  weeklyChallengesToDo: ChallengeModel[] = []
  weeklyChallengesDone: ChallengeModel[] = []

  ngOnInit(): void {
    const userId = this.authService.getUserInfo().id
    this.loadChallenges(userId)
  }

  loadChallenges(userId: number) {
    this.netApi
      .get<ChallengeResponse>('Challenges', 'GetUserChallenges', undefined, userId.toString())
      .subscribe({
        next: (data) => {
          if (data.success) {
            this.dailyChallengesToDo = data.dailyChallengesToDo
            this.dailyChallengesDone = data.dailyChallengesDone
            this.weeklyChallengesToDo = data.weeklyChallengesToDo
            this.weeklyChallengesDone = data.weeklyChallengesDone
          } else {
            this.popupLoader.showPopup('Erro', data.message || 'Erro ao carregar desafios.')
          }
        },
        error: () => {
          this.popupLoader.showPopup('Erro', 'Erro de rede ao carregar desafios.')
        }
      })
  }
}
