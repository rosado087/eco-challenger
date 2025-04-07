import { Component, inject, OnInit } from '@angular/core'
import { TableComponent } from '../../../components/table/table.component'
import { NetApiService } from '../../../services/net-api/net-api.service'
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service'
import {NgIcon, provideIcons } from '@ng-icons/core'
import {
    heroPencil,
    heroTrash,
    heroMagnifyingGlass
} from '@ng-icons/heroicons/outline'
import { ActivatedRoute, Router, UrlSegment } from '@angular/router'
import { FormControl, ReactiveFormsModule } from '@angular/forms'
import { User } from '../../../models/user'
import { SuccessModel } from '../../../models/success-model'
import { ButtonComponent } from '../../../components/button/button.component'
import { CommonModule } from '@angular/common'

@Component({
    selector: 'app-users-admin',
  imports: [
        NgIcon,
    TableComponent,
    ButtonComponent,
        CommonModule,
        ReactiveFormsModule
    ],
    providers: [
        PopupLoaderService,
        provideIcons({ heroPencil, heroTrash, heroMagnifyingGlass })
    ],
    templateUrl: './users-admin.component.html',
    styleUrl: './users-admin.component.css'
})
export class UsersAdminComponent implements OnInit {
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)
    route = inject(ActivatedRoute)
    router = inject(Router)
    users: User[] = []

    searchControl = new FormControl('')
    confirmControl = false

    ngOnInit(): void {
        
      this.loadUsers();
        
    }

    loadUsers(name?: string | null):void {
      this.netApi.get<User[]>('Users', 'users', name? {q: name}:undefined)
        .subscribe({
          next: (data) => {
            this.users = data
          },
          error: () => {
            this.popupLoader.showPopup(
              'Erro',
              'Ocorreu um erro ao carregar a lista de utilizadores'
            )
          }
        })
    }


    handleSearch(): void {

      this.loadUsers(this.searchControl.value);
    }

    changeBlock(id: number):void {
      this.netApi.post<SuccessModel>('Users', 'block', undefined, id.toString())
        .subscribe({
          next: (data) => {
            if(data.success) this.handleSearch()

            this.popupLoader.showPopup(
              data.success ? 'Sucesso' : 'Erro',
              data.message
            )
          },
          error: () =>{
            this.popupLoader.showPopup(
              'Erro',
              'Erro ao mudar o estado do utilizador'
            )
          }
        }
      )
    }
    /*
    changeAdmin(id: number): void {
      this.netApi.post <SuccessModel>('Users', 'admin',undefined, id.toString())
        .subscribe({
          next: (data) => {
            if (data.success) this.handleSearch()
            
            this.popupLoader.showPopup(
              data.success ?'Sucesso' : 'Erro',
              data.message
            )
          },
          error: () => {
            this.popupLoader.showPopup(
              'Erro',
              'Erro ao mudar o estado do utilizador'
            )
          }
        }
        )
    }*/
}
