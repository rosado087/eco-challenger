import { Component, inject, OnInit } from '@angular/core'
import { NgIcon, provideIcons } from '@ng-icons/core'
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { NetApiService } from '../../services/net-api/net-api.service'
import { ActivatedRoute, Router } from '@angular/router'
import { AuthService } from '../../services/auth/auth.service'
import { FormsModule } from '@angular/forms'
import { NgFor, NgIf } from '@angular/common'
import {
    heroUser,
    heroLockClosed,
    heroPencil
} from '@ng-icons/heroicons/outline'
import { UserModel } from '../../models/user-model'
import {
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators
} from '@angular/forms'
import { ButtonComponent } from '../../components/button/button.component'
import { LoginResponseModel } from '../../models/login-response-model'
import {
    FriendsResponse,
    UserList,
    AddFriendResponse,
    RemoveFriendResponse
} from '../../models/user-profile-model'
import { TagFormModalComponent } from '../profile-tag-selector-modal/profile-tag-selector-modal.component'
import { Tag } from '../../models/tag'
import { TagComponent } from '../../components/tag/tag.component'
import { TopUserMostPointsComponent } from "../../components/charts/top-users-most-points/top-users-most-points.component";

@Component({
    selector: 'app-user-profile',
    standalone: true,
    imports: [
    NgIcon,
    NgFor,
    NgIf,
    ReactiveFormsModule,
    ButtonComponent,
    FormsModule,
    TagFormModalComponent,
    TagComponent,
    TopUserMostPointsComponent
],
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.css'],
    providers: [
        provideIcons({ heroUser, heroLockClosed, heroPencil }),
        PopupLoaderService
    ]
})
export class UserProfileComponent implements OnInit {
    netApi = inject(NetApiService)
    popupLoader = inject(PopupLoaderService)
    router = inject(Router)
    route = inject(ActivatedRoute)
    authService = inject(AuthService)
    userId: number = 0
    username: string = ''
    friendId: number = 0
    email: string = ''
    id: number = 0
    points: string = ''
    tags: Tag[] = []
    friendsList: { username: string; id: number }[] = []
    isEditing: boolean = false
    userList: string[] = []
    showAddFriendModal: boolean = false
    showRemoveFriendModal: boolean = false
    selectedFriendIndex: number | null = null
    searchUsername: string = ''
    selectedUser: string = ''

    // Tag modal form props
    modalIsEditMode: boolean = false
    modalTagId: number | null = null
    modalShow: boolean = false

    editForm = new FormGroup({
        editUserName: new FormControl('', [Validators.required])
    })

    getUserName() {
        return this.editForm.get('editUserName')
    }

    getTag() {
        return this.editForm.get('tag')
    }

    ngOnInit(): void {
        this.route.url.subscribe((paths) => {
            // Check if we are viewing our own profile or someone else's
            if (paths[0].path == 'profile')
                this.userId = this.id = this.authService.getUserInfo().id
            else {
                // The last path should be the ID
                this.userId = Number(paths[paths.length - 1].path)
                this.id = this.authService.getUserInfo().id
            }

            this.loadUserProfile()
            this.loadUserFriends()
            this.loadUserTags()
        })
    }

    openTagsModal(): void {
        if (this.tags.length == 0)
            return this.popupLoader.showPopup('Não existem tags para escolher.')
        this.modalShow = true
    }

    closeTagsModal(): void {
        this.modalShow = false
        this.loadUserTags()
    }

    loadUserTags(): void {
        // Load tags
        this.netApi
            .get<Tag[]>('Tag', 'user', undefined, this.userId.toString())
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

    /**
     * Load user profile details from API.
     */
    loadUserProfile() {
        this.netApi
            .get<UserModel>(
                'Profile',
                'GetUserInfo',
                undefined,
                this.userId.toString()
            )
            .subscribe({
                next: (data) => {
                    if (data.success) {
                        this.username = data.username
                        this.email = data.email
                        this.points = data.points
                    } else {
                        this.popupLoader.showPopup(
                            'Erro',
                            data.message || 'Ocorreu um erro!'
                        )
                    }
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro',
                        'Não foi possível carregar os dados do perfil.'
                    )
                }
            })
    }

    editProfile() {
        this.isEditing = true
    }

    confirmEdit() {
        this.editForm.markAllAsTouched()
        if (!this.editForm.valid) return

        const params = {
            Id: this.authService.getUserInfo().id,
            Username: this.getUserName()?.value || '',
            Tag: this.getTag()?.value || ''
        }

        this.netApi
            .post<UserModel>('Profile', 'EditUserInfo', params)
            .subscribe({
                next: (data: UserModel) => {
                    if (data.success) {
                        this.username = data.username
                        this.email = data.email
                        this.points = data.points
                        this.popupLoader.showPopup(
                            'Alteração dos dados',
                            'Os dados foram alterados com sucesso.'
                        )
                        this.editTokenDetails()
                    } else {
                        this.popupLoader.showPopup(
                            'Alteração dos dados',
                            'Não foi possível editar os seus dados.'
                        )
                    }
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Alteração dos dados',
                        'Erro ao editar os seus dados.'
                    )
                }
            })

        this.isEditing = false
    }

    editTokenDetails() {
        this.netApi
            .get<LoginResponseModel>('Profile', 'GenerateToken')
            .subscribe({
                next: (data: LoginResponseModel) => {
                    if (data.success) {
                        this.authService.updateToken(data.user, data.token)
                        this.loadUserProfile()
                    } else {
                        this.popupLoader.showPopup(
                            'Alteração dos dados',
                            'Não.'
                        )
                    }
                },
                error: () => {
                    this.popupLoader.showPopup('Alteração dos dados', 'Erro.')
                }
            })
    }

    cancelEdit() {
        this.isEditing = false
        this.loadUserProfile()
    }

    loadUserFriends() {
        this.netApi
            .get<FriendsResponse>(
                'Profile',
                'GetFriends',
                undefined,
                this.userId.toString()
            )
            .subscribe({
                next: (data: FriendsResponse) => {
                    if (data.success) {
                        this.friendsList = data.friends
                    } else {
                        this.popupLoader.showPopup(
                            'Erro',
                            'Não foi possível carregar a lista de amigos.'
                        )
                    }
                },
                error: (err) => {
                    console.error('Erro ao buscar amigos:', err)
                    this.popupLoader.showPopup('Erro', 'Erro ao buscar amigos.')
                }
            })
    }

    /**
     * Search for users by username.
     */
    findUser() {
        this.userList = []
        this.searchUsername =
            (document.getElementById('input-add') as HTMLInputElement)?.value ||
            ''

        const params = {
            Id: this.id,
            FriendUsername: this.searchUsername
        }

        if (this.searchUsername.trim() !== '') {
            this.netApi
                .post<UserList>('Profile', 'UserList', params)
                .subscribe({
                    next: (data) => {
                        this.userList = data.usernames
                    },
                    error: () => {
                        this.popupLoader.showPopup(
                            'Erro',
                            'Erro ao buscar usuários.'
                        )
                    }
                })
        }
    }

    /**
     * Select a user to add as a friend.
     * @param username Selected username
     */
    selectUser(username: string) {
        this.selectedUser =
            this.selectedUser.toLowerCase() === username.toLowerCase()
                ? ''
                : username
    }

    /**
     * Add a friend to the user's list.
     */
    addFriend() {
        if (this.selectedUser.trim() === '') {
            this.popupLoader.showPopup(
                'Erro',
                'Digite um nome de utilizador válido.'
            )
            return
        }

        const params = {
            Id: this.id,
            FriendUsername: this.selectedUser
        }

        this.netApi
            .post<AddFriendResponse>('Profile', 'AddFriend', params)
            .subscribe({
                next: (response) => {
                    if (response.success && response.friendId) {
                        this.friendsList.push({
                            username: this.selectedUser,
                            id: response.friendId
                        })

                        this.selectedUser = ''
                        this.userList = []
                        this.showAddFriendModal = false

                        this.popupLoader.showPopup(
                            'Sucesso',
                            response.message || 'Amigo adicionado com sucesso.'
                        )
                    } else {
                        this.popupLoader.showPopup(
                            'Erro',
                            response.message ||
                                'Não foi possível adicionar o amigo.'
                        )
                    }
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro',
                        'Erro ao tentar adicionar o amigo.'
                    )
                }
            })
    }

    /**
     * Remove a friend from the user's list.
     */
    removeFriend(index: number | null) {
        if (index === null) {
            this.popupLoader.showPopup('Erro', 'Nenhum amigo selecionado.')
            return
        }

        const friendUsername = this.friendsList[index]?.username
        if (!friendUsername) return

        const params = {
            Id: this.id,
            FriendUsername: friendUsername
        }

        this.netApi
            .post<RemoveFriendResponse>('Profile', 'RemoveFriend', params)
            .subscribe({
                next: (response) => {
                    if (response.success) {
                        this.friendsList.splice(index, 1)
                        this.showRemoveFriendModal = false
                        this.selectedFriendIndex = null
                        this.popupLoader.showPopup(
                            'Sucesso',
                            response.message || 'Amigo removido com sucesso.'
                        )
                    } else {
                        this.popupLoader.showPopup(
                            'Erro',
                            response.message ||
                                'Não foi possível remover o amigo.'
                        )
                    }
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro',
                        'Erro ao tentar remover o amigo.'
                    )
                }
            })
    }

    viewProfile(id: number) {
        this.isEditing = false
        this.router.navigate(['/user-profile/' + id])
    }
}
