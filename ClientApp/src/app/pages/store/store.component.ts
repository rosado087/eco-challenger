import { Component, inject, OnInit } from '@angular/core';
import { StoreTagComponent } from "../../components/store-tag/store-tag.component";
import { TagComponent } from "../../components/tag/tag.component";
import { NetApiService } from '../../services/net-api/net-api.service';
import { Tag } from '../../models/tag';
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service';
import { EcoPointsIconComponent } from "../../components/eco-points-icon/eco-points-icon.component";
import { AuthService } from '../../services/auth/auth.service';

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
  tags: Tag[] = []
    
  ngOnInit(): void {
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

  buyTag(id: number): void {

  }
}
