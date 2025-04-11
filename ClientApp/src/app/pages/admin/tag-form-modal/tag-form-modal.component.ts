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
import {
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators
} from '@angular/forms'
import { ButtonComponent } from '../../../components/button/button.component'
import { TagComponent } from '../../../components/tag/tag.component'
import { Tag, TagStyle } from '../../../models/tag'
import { NetApiService } from '../../../services/net-api/net-api.service'
import { SuccessModel } from '../../../models/success-model'
import { Router } from '@angular/router'

@Component({
    selector: 'app-tag-form-modal',
    imports: [ButtonComponent, ReactiveFormsModule, TagComponent],
    templateUrl: './tag-form-modal.component.html',
    styleUrl: './tag-form-modal.component.css'
})
export class TagFormModalComponent implements OnInit {
    tagModalRef = viewChild('tagModal')
    router = inject(Router)
    netApi = inject(NetApiService)
    open = input<boolean>(false)
    tagId = input<number | null>(null)
    isEditMode = input<boolean>(false)
    closeClick = output()

    loadedTag: Tag | null = null

    formError: string | null = null

    tagForm = new FormGroup({
        name: new FormControl(this.loadedTag?.name || 'Exemplo'),
        backgroundColor: new FormControl(
            this.loadedTag?.backgroundColor || '#FFDDDD',
            [Validators.required]
        ),
        textColor: new FormControl(this.loadedTag?.textColor || '#FF0000', [
            Validators.required
        ]),
        icon: new FormControl<File | null>(null),
        style: new FormControl(this.loadedTag?.style || 'normal', [
            Validators.required
        ]),
        price: new FormControl(this.loadedTag?.price || 0, [
            Validators.required
        ])
    })

    doStateChange = effect(() => {
        const isOpen = this.open()

        untracked(() => {
            const elem = this.tagModalRef() as ElementRef<HTMLDialogElement>

            this.resetForm()

            if (isOpen) elem.nativeElement.showModal()
            else elem.nativeElement.close()
        })
    })

    doIdChange = effect(() => {
        const tagId = this.tagId()
        if (!tagId) return

        untracked(() => {
            this.loadTag(tagId)
        })
    })

    ngOnInit(): void {
        // If edit mode, we need to load the tag
        if (!this.isEditMode() || !this.tagId()) return
        this.loadTag(this.tagId()!)
    }

    resetForm() {
        this.tagForm.reset({
            name: 'Exemplo',
            backgroundColor: '#FFDDDD',
            textColor: '#FF0000',
            icon: null,
            style: 'normal',
            price: 0
        })
        this.loadedTag = null
    }

    loadTag(id: number) {
        this.netApi
            .get<Tag>('Tag', 'per-id', undefined, id.toString())
            .subscribe({
                next: (data) => {
                    if (!data)
                        this.formError =
                            'Não foi encontrada nenhuma tag com o ID correspondente.'
                    else {
                        this.loadedTag = data
                        this.updateFormTagData(data)
                    }
                },
                error: () =>
                    (this.formError =
                        'Não foi encontrada nenhuma tag com o ID correspondente.')
            })
    }

    updateFormTagData(tag: Tag): void {
        this.tagForm.reset({
            name: tag.name,
            backgroundColor: tag.backgroundColor,
            textColor: tag.textColor,
            icon: null,
            style: tag.style,
            price: tag.price
        })
    }

    isIconValid(): boolean {
        const icon = this.tagForm.get('icon')?.value
        if (!icon) return false

        return icon.type.split('/')[0] == 'image'
    }

    getPreviewBackgroundColor(): string {
        return this.tagForm.get('backgroundColor')?.value || '#000000'
    }

    getPreviewTextColor(): string {
        return this.tagForm.get('textColor')?.value || '#FFFFFF'
    }

    getPreviewStyle(): TagStyle | null {
        return (this.tagForm.get('style')?.value as TagStyle) || null
    }

    onImagePicked(event: Event): void {
        const files = (event.target as HTMLInputElement).files
        if (!files) return

        // We need emitModelToViewChange so it doesnt update the DOM
        this.tagForm
            .get('icon')
            ?.setValue(files[0], { emitModelToViewChange: false })
    }

    submitRecord() {
        this.tagForm.markAllAsTouched()
        if (
            !this.tagForm.valid ||
            (!this.isIconValid() && this.tagForm.get('icon')?.value)
        )
            return

        const formData = new FormData()
        const formValues = this.tagForm.value

        formData.append('Name', formValues.name || '')
        formData.append('BackgroundColor', formValues.backgroundColor || '')
        formData.append('TextColor', formValues.textColor || '')
        formData.append('Price', formValues.price?.toString() || '0')
        formData.append('Style', formValues.style || 'normal')

        if (formValues.icon) formData.append('Icon', formValues.icon)

        if (this.isEditMode()) this.editTag(formData)
        else this.createTag(formData)
    }

    createTag(formData: FormData) {
        this.netApi.post<SuccessModel>('Tag', 'create', formData).subscribe({
            next: (data) => {
                if (!data.success)
                    this.formError =
                        'Ocorreu um erro desconhecido ao guardar a tag.'
            },
            error: () => {
                this.formError =
                    'Ocorreu um erro desconhecido ao guardar a tag.'
            }
        })
    }

    editTag(formData: FormData) {
        this.netApi
            .post<SuccessModel>(
                'Tag',
                'edit',
                formData,
                this.tagId()!.toString()
            )
            .subscribe({
                next: (data) => {
                    if (!data.success)
                        this.formError =
                            'Ocorreu um erro desconhecido ao guardar a tag.'
                },
                error: () => {
                    this.formError =
                        'Ocorreu um erro desconhecido ao guardar a tag.'
                }
            })
    }
}
