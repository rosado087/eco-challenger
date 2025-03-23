import { Component, inject, OnInit } from '@angular/core';
import { TableComponent } from "../../../components/table/table.component";
import { ButtonComponent } from "../../../components/button/button.component";
import { NetApiService } from '../../../services/net-api/net-api.service';
import { Tag } from '../../../models/tag';
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service';
import { EcoPointsIconComponent } from "../../../components/eco-points-icon/eco-points-icon.component";
import { TagComponent } from "../../../components/tag/tag.component";

import { NgIcon, provideIcons } from '@ng-icons/core'
import {
  heroPencil,
  heroTrash,
  heroMagnifyingGlass
} from '@ng-icons/heroicons/outline'

@Component({
  selector: 'app-tags-admin',
  imports: [NgIcon, TableComponent, ButtonComponent, EcoPointsIconComponent, TagComponent],
  providers: [PopupLoaderService, provideIcons({ heroPencil, heroTrash, heroMagnifyingGlass }),],
  templateUrl: './tags-admin.component.html',
  styleUrl: './tags-admin.component.css'
})
export class TagsAdminComponent implements OnInit {
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  tags: Tag[] = []
  
  ngOnInit(): void {
    // Load tags
    this.netApi
    .get<Tag[]>('Tag', 'all')
    .subscribe({
        next: (data) => {
            if (!data || data.length == 0) return

            this.tags = data
        },
        error: () => this.popupLoader.showPopup(
          'Erro!',
          'Ocorreu um erro desconhecido ao carregar as tags.'
        )
    })
  }

  addTag() {

  }
}
