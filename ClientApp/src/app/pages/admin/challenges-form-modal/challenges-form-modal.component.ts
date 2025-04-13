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
import { NetApiService } from '../../../services/net-api/net-api.service'
import { SuccessModel } from '../../../models/success-model'
import { Router } from '@angular/router'
import { ChallengeModel, ChallengeType } from '../../../models/challenge-model'
import { GetChallengeResponse } from '../../../models/challenge-response'

@Component({
    selector: 'app-challenge-form-modal',
    imports: [ButtonComponent, ReactiveFormsModule],
    templateUrl: './challenges-form-modal.component.html',
    styleUrl: './challenges-form-modal.component.css'
})
export class ChallengeFormModalComponent implements OnInit {
    challengeModalRef = viewChild('challengeModal')
    router = inject(Router)
    netApi = inject(NetApiService)
    open = input<boolean>(false)
    challengeId = input<number | null>(null)
    isEditMode = input<boolean>(false)
    closeClick = output()

    loadedChallenge: ChallengeModel | null = null
    formError: string | null = null

    challengeForm = new FormGroup({
        title: new FormControl('', [Validators.required]),
        description: new FormControl('', [Validators.required]),
        type: new FormControl<ChallengeType>('Daily', [Validators.required]),
        points: new FormControl(0, [Validators.required, Validators.min(0)])
    })

    doStateChange = effect(() => {
        const isOpen = this.open()

        untracked(() => {
            const elem =
                this.challengeModalRef() as ElementRef<HTMLDialogElement>

            if (isOpen) {
                if (!this.isEditMode()) {
                    this.resetForm()
                }

                elem.nativeElement.showModal()
            } else {
                elem.nativeElement.close()
            }
        })
    })

    doIdChange = effect(() => {
        const id = this.challengeId()
        if (!id) return

        untracked(() => this.loadChallenge(id))
    })

    ngOnInit(): void {
        if (!this.isEditMode() || !this.challengeId()) return
        this.loadChallenge(this.challengeId()!)
    }

    resetForm() {
        this.challengeForm.reset({
            title: '',
            description: '',
            type: 'Daily',
            points: 0
        })

        this.formError = null
        this.loadedChallenge = null
    }

    loadChallenge(id: number) {
        this.netApi
            .get<GetChallengeResponse>(
                'Challenge',
                'GetChallenge',
                undefined,
                id.toString()
            )
            .subscribe({
                next: (data) => {
                    if (!data)
                        this.formError =
                            'Não foi encontrada nenhuma tag com o ID correspondente.'
                    else {
                        this.loadedChallenge = data.challenge
                        this.updateFormChallengeData(data.challenge)
                    }
                },
                error: () =>
                    (this.formError =
                        'Não foi encontrada nenhuma tag com o ID correspondente.')
            })
    }

    updateFormChallengeData(challenge: ChallengeModel): void {
        this.challengeForm.reset({
            title: challenge.title,
            description: challenge.description,
            type: challenge.type,
            points: challenge.points
        })
    }

    submitRecord() {
        this.challengeForm.markAllAsTouched()
        if (!this.challengeForm.valid) return

        const formData = new FormData()
        const formValues = this.challengeForm.value

        formData.append('Title', formValues.title || '')
        formData.append('Description', formValues.description || '')
        formData.append('Type', formValues.type || 'Daily')
        formData.append('Points', formValues.points?.toString() || '0')

        if (this.isEditMode()) this.editChallenge(formData)
        else this.createChallenge(formData)
    }

    createChallenge(formData: FormData) {
        this.netApi
            .post<SuccessModel>('Challenge', 'CreateChallenge', formData)
            .subscribe({
                next: (data) => {
                    if (!data.success)
                        this.formError =
                            data.message ?? 'Erro ao guardar o desafio.'
                    else {
                        this.formError = null
                        this.closeClick.emit()
                    }
                },
                error: () => {
                    this.formError = 'Erro ao guardar o desafio.'
                }
            })
    }

    editChallenge(formData: FormData) {
        this.netApi
            .post<SuccessModel>(
                'Challenge',
                'EditChallenge',
                formData,
                this.challengeId()!.toString()
            )
            .subscribe({
                next: (data) => {
                    if (!data.success)
                        this.formError =
                            data.message ?? 'Erro ao guardar o desafio.'
                    else this.closeClick.emit()
                },
                error: () => {
                    this.formError = 'Erro ao guardar o desafio.'
                }
            })
    }
}
