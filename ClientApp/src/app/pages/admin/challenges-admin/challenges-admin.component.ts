import { Component, inject, OnInit } from '@angular/core'
import { CommonModule } from '@angular/common'
import { TableComponent } from '../../../components/table/table.component'
import { ButtonComponent } from '../../../components/button/button.component'
import { NetApiService } from '../../../services/net-api/net-api.service'
import { ChallengeModel } from '../../../models/challenge-model'
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service'
import { EcoPointsIconComponent } from '../../../components/eco-points-icon/eco-points-icon.component'

import { NgIcon, provideIcons } from '@ng-icons/core'
import {
    heroPencil,
    heroTrash,
    heroMagnifyingGlass
} from '@ng-icons/heroicons/outline'
import { SuccessModel } from '../../../models/success-model'
import { ActivatedRoute, Router, UrlSegment } from '@angular/router'
import { ChallengeFormModalComponent } from '../challenges-form-modal/challenges-form-modal.component'
import { FormControl, ReactiveFormsModule } from '@angular/forms'

@Component({
    selector: 'app-challenges-admin',
    imports: [
        NgIcon,
        CommonModule,
        TableComponent,
        ButtonComponent,
        EcoPointsIconComponent,
        ChallengeFormModalComponent,
        ReactiveFormsModule
    ],
    providers: [
        PopupLoaderService,
        provideIcons({ heroPencil, heroTrash, heroMagnifyingGlass })
    ],
    templateUrl: './challenges-admin.component.html',
    styleUrl: './challenges-admin.component.css'
})
export class ChallengesAdminComponent implements OnInit {
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)
    route = inject(ActivatedRoute)
    router = inject(Router)
    challenges: ChallengeModel[] = []

    searchControl = new FormControl('')

    // Modal form props
    modalIsEditMode: boolean = false
    modalChallengeId: number | null = null
    modalShow: boolean = false

    ngOnInit(): void {
        this.loadChallenges()

        // Check the route to decided whether to open
        // the modal form or not
        this.route.url.subscribe(() => {
            this.route.firstChild?.url.subscribe((segments: UrlSegment[]) => {
                const paths = segments.map((s) => s.path)

                // Open modal form in create mode
                if (paths[0] === 'create') return this.openModal()

                // Open modal form in edit mode with the corresponding ID
                if (paths[0] === 'edit') {
                    if (paths.length <= 1) {
                        //Invalid id, lets redirect
                        this.router.navigate(['/admin/challenges'])
                        return
                    }

                    this.openModal(true, Number(paths[1]))
                }
            })
        })
    }

    openModal(isEdit: boolean = false, id?: number): void {
        this.modalIsEditMode = isEdit
        if (isEdit && id) this.modalChallengeId = id
        else this.modalChallengeId = null

        this.modalShow = true
    }

    closeModal(): void {
        this.modalShow = false
        this.modalChallengeId = null
        this.modalIsEditMode = false
        this.router.navigate(['/admin/challenges'])
        this.loadChallenges()
    }

    loadChallenges(searchQuery?: string | null): void {
        // Load tags
        this.netApi
            .get<
                ChallengeModel[]
            >('Challenge', 'GetAllChallenges', searchQuery ? { q: searchQuery } : undefined)
            .subscribe({
                next: (data) => {
                    if (!data || data.length == 0) return

                    this.challenges = data
                },
                error: () =>
                    this.popupLoader.showPopup(
                        'Erro!',
                        'Ocorreu um erro desconhecido ao carregar os desafios.'
                    )
            })
    }

    handleSearch(): void {
        this.loadChallenges(this.searchControl.value)
    }

    handleAddChallenge(): void {
        this.router.navigate(['/admin/challenges/create'])
    }

    handleEditChallenge(id: number): void {
        this.router.navigate([`/admin/challenges/edit/${id}`])
    }

    removeChallenge(id: number): void {
        const remove = () => {
            this.netApi
                .post<SuccessModel>(
                    'Challenge',
                    'DeleteChallenge',
                    undefined,
                    id.toString()
                )
                .subscribe({
                    next: (data) => {
                        if (!data || !data.success)
                            return this.popupLoader.showPopup(
                                'Erro!',
                                data.message ||
                                    'Ocorreu um erro desconhecido ao remover o desafio.'
                            )
                        this.loadChallenges()
                    },
                    error: () =>
                        this.popupLoader.showPopup(
                            'Erro!',
                            'Ocorreu um erro desconhecido ao remover o desafio.'
                        )
                })
        }

        this.popupLoader.showPopup(
            'Remover Desafio',
            'Tem a certeza que pretende remover o Desafio?',
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
