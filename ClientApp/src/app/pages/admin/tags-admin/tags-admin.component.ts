import { Component, inject, OnInit } from '@angular/core'
import { TableComponent } from '../../../components/table/table.component'
import { ButtonComponent } from '../../../components/button/button.component'
import { NetApiService } from '../../../services/net-api/net-api.service'
import { Tag } from '../../../models/tag'
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service'
import { EcoPointsIconComponent } from '../../../components/eco-points-icon/eco-points-icon.component'
import { TagComponent } from '../../../components/tag/tag.component'

import { NgIcon, provideIcons } from '@ng-icons/core'
import {
    heroPencil,
    heroTrash,
    heroMagnifyingGlass
} from '@ng-icons/heroicons/outline'
import { SuccessModel } from '../../../models/success-model'
import { ActivatedRoute, Router, UrlSegment } from '@angular/router'
import { TagFormModalComponent } from '../tag-form-modal/tag-form-modal.component'
import { FormControl, ReactiveFormsModule } from '@angular/forms'

@Component({
    selector: 'app-tags-admin',
    imports: [
        NgIcon,
        TableComponent,
        ButtonComponent,
        EcoPointsIconComponent,
        TagComponent,
        TagFormModalComponent,
        ReactiveFormsModule
    ],
    providers: [
        PopupLoaderService,
        provideIcons({ heroPencil, heroTrash, heroMagnifyingGlass })
    ],
    templateUrl: './tags-admin.component.html',
    styleUrl: './tags-admin.component.css'
})
export class TagsAdminComponent implements OnInit {
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)
    route = inject(ActivatedRoute)
    router = inject(Router)
    tags: Tag[] = []

    searchControl = new FormControl('')

    // Modal form props
    modalIsEditMode: boolean = false
    modalTagId: number | null = null
    modalShow: boolean = false

    ngOnInit(): void {
        this.loadTags()

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
                        this.router.navigate(['/admin/tags'])
                        return
                    }

                    this.openModal(true, Number(paths[1]))
                }
            })
        })
    }

    openModal(isEdit: boolean = false, id?: number): void {
        this.modalIsEditMode = isEdit
        if (isEdit && id) this.modalTagId = id
        else this.modalTagId = null

        this.modalShow = true
    }

    closeModal(): void {
        this.modalShow = false
        this.modalTagId = null
        this.modalIsEditMode = false
        this.router.navigate(['/admin/tags'])
        this.loadTags()
    }

    loadTags(searchQuery?: string | null): void {
        // Load tags
        this.netApi
            .get<
                Tag[]
            >('Tag', 'all', searchQuery ? { q: searchQuery } : undefined)
            .subscribe({
                next: (data) => {
                    if (!data || data.length == 0) return

                    this.tags = data
                },
                error: () =>
                    this.popupLoader.showPopup(
                        'Erro!',
                        'Ocorreu um erro desconhecido ao carregar as tags.'
                    )
            })
    }

    handleSearch(): void {
        this.loadTags(this.searchControl.value)
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
                                data.message ||
                                    'Ocorreu um erro desconhecido ao remover a tag.'
                            )

                        this.loadTags()
                    },
                    error: () =>
                        this.popupLoader.showPopup(
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
