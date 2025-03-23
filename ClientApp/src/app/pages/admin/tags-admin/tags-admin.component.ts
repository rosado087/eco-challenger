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
import { SuccessModel } from '../../../models/success-model';
import { ActivatedRoute, Router } from '@angular/router';
import { TagFormModalComponent } from "../tag-form-modal/tag-form-modal.component";

@Component({
  selector: 'app-tags-admin',
  imports: [NgIcon, TableComponent, ButtonComponent, EcoPointsIconComponent, TagComponent, TagFormModalComponent],
  providers: [PopupLoaderService, provideIcons({ heroPencil, heroTrash, heroMagnifyingGlass }),],
  templateUrl: './tags-admin.component.html',
  styleUrl: './tags-admin.component.css'
})
export class TagsAdminComponent implements OnInit {
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  route = inject(ActivatedRoute)
  router = inject(Router)
  tags: Tag[] = []

  // Modal form props
  modalIsEditMode: boolean = false
  modalTagId: number = 0
  modalShow: boolean = true
  
  ngOnInit(): void {
    this.loadTags()

    // Check the route to decided whether to open
    // the modal form or not
    this.route.url.subscribe(() => {
      const url = this.route.snapshot.routeConfig?.path;

      // Open modal form in create mode
      if (url === 'create')
        return this.openModal()
      
      // Open modal form in edit mode with the corresponding ID
      if (url === 'edit/:id') {
        const id: string | null = this.route.snapshot.paramMap.get('id');
        
        if (!id) { //Invalid id, lets redirect
          this.router.navigate(['/admin/tags'])
          return
        }

        this.openModal(true, Number(id))        
      }
    })
  }

  openModal(isEdit: boolean = false, id?: number): void {
    this.modalIsEditMode = isEdit
    if(isEdit && id)
      this.modalTagId = id

    this.modalShow = true
  }

  closeModal(): void {
    this.modalShow = false
    this.modalTagId = 0
    this.modalIsEditMode = false
  }

  loadTags(): void {
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

  handleAddTag(): void {
    this.router.navigate(['/admin/tags/create'])
  }

  handleEditTag(id: number): void {
    this.router.navigate([`/admin/tags/edit/${id}`])
  }  

  removeTag(id: number): void {
    const remove = () => {
      this.netApi
      .post<SuccessModel>('Tag', 'remove', undefined, id.toString())
      .subscribe({
          next: (data) => {
              if (!data || !data.success)
                return this.popupLoader.showPopup(
                  'Erro!',
                  data.message || 'Ocorreu um erro desconhecido ao remover a tag.'
                )

                this.popupLoader.showPopup('Tag removida com sucesso!')
          },
          error: () => this.popupLoader.showPopup(
            'Erro!',
            'Ocorreu um erro desconhecido ao remover a tag.'
          )
      })
    }
    
    this.popupLoader.showPopup(
      'Remover Tag',
      'Tem a certeza que pretende remover a Tag?',
      [
          {
              type: 'yes',
              text: 'Sim',
              callback: remove
          },
          {
              type: 'cancel',
              text: 'Cancelar'
          }
      ]
    )
  }
}
