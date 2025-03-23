import { Component, inject, OnInit } from '@angular/core';
import { StoreTagComponent } from "../../components/store-tag/store-tag.component";
import { TagComponent } from "../../components/tag/tag.component";
import { NetApiService } from '../../services/net-api/net-api.service';
import { Tag } from '../../models/tag';
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service';
import { EcoPointsIconComponent } from "../../components/eco-points-icon/eco-points-icon.component";
import { AuthService } from '../../services/auth/auth.service';
import { SuccessModel } from '../../models/success-model';

@Component({
  selector: 'app-store',
  imports: [StoreTagComponent, TagComponent, EcoPointsIconComponent],
  providers: [PopupLoaderService],
  templateUrl: './store.component.html',
  styleUrl: './store.component.css'
})
export class StoreComponent implements OnInit {
  authService = inject(AuthService)
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  tags: Tag[] | null = null
  userPoints: number = 0
    
  ngOnInit(): void {    
    this.loadStoreTags()
    this.loadUserPoints()
  }

  loadStoreTags(): void {
    // Load tags
    this.netApi
    .get<Tag[]>('Store', 'tags')
    .subscribe({
        next: (data) => {
            if (!data || data.length == 0) return

            this.tags = data
        },
        error: () => this.popupLoader.showPopup(
          'Erro!',
          'Ocorreu um erro desconhecido ao carregar as tags de loja.'
        )
    })
  }

  loadUserPoints(): void {
    // Fetch user points
    this.netApi
    .get<number>('Profile', 'points')
    .subscribe({
        next: (data) => this.userPoints = data,
        error: () => this.popupLoader.showPopup(
          'Erro!',
          'Ocorreu um erro ao obter os pontos do utilizador.'
        )
    })
  }

  buyTag(id: number): void {
    this.netApi
    .post<SuccessModel>('Store', 'purchase', undefined, id.toString())
    .subscribe({
        next: (data) => {
          if(!data.success)
            return this.popupLoader.showPopup(
              'Não foi possível realizar a compra.',
              data.message || 'Ocorreu um erro ao realizar a compra.'
            )          

          this.popupLoader.showPopup(
            'Compra realizada com sucesso!'
          )

          this.ngOnInit() //Reload the store page data
        },
        error: () => this.popupLoader.showPopup(
          'Erro!',
          'Ocorreu um erro ao realizar a compra.'
        )
    })
  }
}
