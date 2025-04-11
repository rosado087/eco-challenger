import {
    Component,
    effect,
    ElementRef,
    inject,
    input,
    OnInit,
    output,
    untracked,
    viewChild
} from '@angular/core'
import { ButtonComponent } from '../../components/button/button.component'
import { TagComponent } from '../../components/tag/tag.component'
import { Tag } from '../../models/tag'
import { NetApiService } from '../../services/net-api/net-api.service'
import { SuccessModel } from '../../models/success-model'
import { Router } from '@angular/router'

@Component({
    selector: 'app-profile-tag-selector-modal',
    imports: [ButtonComponent, TagComponent],
    templateUrl: './profile-tag-selector-modal.component.html',
    styleUrl: './profile-tag-selector-modal.component.css',
})
export class TagFormModalComponent implements OnInit {
    tagModalRef = viewChild('tagModal')
    router = inject(Router)
    netApi = inject(NetApiService)
    open = input<boolean>(false)
    tags = input<Tag[]>([])
    closeClick = output()
    selectedTags: Tag[] = []

    formError: string | null = null

    loadedTag: Tag | null = null

    doStateChange = effect(() => {
        const isOpen = this.open()

        untracked(() => {
            const elem = this.tagModalRef() as ElementRef<HTMLDialogElement>

            if (isOpen) {
                this.loadSelectedTags()
                elem.nativeElement.showModal()
            }
            else {
                this.selectedTags = []
                elem.nativeElement.close()
            }
        })
    })

    ngOnInit(): void {
        this.loadSelectedTags()
    }

    loadSelectedTags() {
        this.tags().forEach(t => {
            if(t.isBeingUsed) this.selectedTags.push(t)
        })
    }

    tagStateChange(event: Event, tag: Tag) {
        const checked = (event.target as HTMLInputElement).checked

        if(checked) this.selectedTags.push(tag)
        else this.selectedTags = this.selectedTags.filter(t => t.id !== tag.id)
    }

    submit() {
        this.netApi
            .post<SuccessModel>('Profile', 'select-tags', { tagIds: this.selectedTags.map(t => t.id)})
            .subscribe({
                next: (data) => {
                    if (data.success) {
                        this.closeClick.emit()
                        return
                    }
                    
                    this.formError = data.message || 'Ocorreu um erro!'
                },
                error: () => this.formError = 'Não foi possível guardar as tags selecionadas.'
            })
    }
}
