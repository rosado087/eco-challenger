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
import { FriendsResponse, UserList } from '../../models/user-profile-model'

@Component({
    selector: 'app-user-profile',
    standalone: true,
    imports: [
        NgIcon,
        NgFor,
        NgIf,
        ReactiveFormsModule,
        ButtonComponent,
        FormsModule
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
    tag: string = ''
    friendsList: { username: string; id: number }[] = []
    isEditing: boolean = false
    availableTags: string[] = []
    userList: string[] = []
    showAddFriendModal: boolean = false
    showRemoveFriendModal: boolean = false
    selectedFriendIndex: number | null = null
    searchUsername: string = ''
    selectedUser: string = ''

    editForm = new FormGroup({
        editUserName: new FormControl('', [Validators.required]),
        tag: new FormControl('')
    })

    getUserName() {
        return this.editForm.get('editUserName')
    }

    getTag() {
        return this.editForm.get('tag')
    }

    /*ngOnInit(): void {
        this.userId = Number(this.route.snapshot.paramMap.get('id'))
        this.id = this.authService.getUserInfo().id
        this.loadUserProfile(this.userId)
        this.loadUserFriends(this.userId)
    }*/

    ngOnInit(): void {
        this.route.paramMap.subscribe(params => {
            this.userId = Number(params.get('id'));
            this.id = this.authService.getUserInfo().id;
            this.loadUserProfile(this.userId);
            this.loadUserFriends(this.userId);
        });
    }
    

    /**
     * Load user profile details from API.
     */
    loadUserProfile(id: number) {
        this.netApi
            .get<UserModel>('Profile', 'GetUserInfo', id.toString())
            .subscribe({
                next: (data) => {
                    if (data.success) {
                        this.username = data.username
                        this.email = data.email
                        this.points = data.points
                        this.tag = data.tag
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

        this.netApi
            .get<UserModel>('Profile', 'GetTags', this.id.toString())
            .subscribe({
                next: (data) => {
                    if (data.success) {
                        this.availableTags = data.list
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
                        'Não foi possível carregar as tags do perfil.'
                    )
                }
            })
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
                        this.tag = data.tag
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
                        this.loadUserProfile(this.userId)
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
        this.loadUserProfile(this.userId)
    }

    loadUserFriends(id: number) {
        this.netApi
            .get<FriendsResponse>('Profile', 'GetFriends', id.toString())
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
            (document.getElementById('InputAdd') as HTMLInputElement)?.value ||
            ''

        if (this.searchUsername.trim() !== '') {
            this.netApi
                .post<UserList>('Profile', 'UserList', [
                    this.username,
                    this.searchUsername
                ])
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

        this.netApi
            .post('Profile', 'AddFriend', [this.username, this.selectedUser])
            .subscribe({
                next: () => {
                    this.friendsList.push({
                        username: this.selectedUser,
                        id: this.getFriendId(this.username)
                    }) 
                    this.selectedUser = ''
                    this.userList = []
                    this.showAddFriendModal = false
                    this.popupLoader.showPopup(
                        'Sucesso',
                        'Amigo adicionado com sucesso.'
                    )
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro',
                        'Não foi possível adicionar o amigo.'
                    )
                }
            })
    }

    getFriendId(username: string): number{
        this.netApi
            .get<UserModel>('Profile', 'GetUserInfo', username)
            .subscribe({
                next: (data) => {
                    if (data.success) {
                        this.friendId = data.id
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
                        'Não foi possível encontrar o id do amigo.'
                    )
                }
            })
        return this.friendId
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

        this.netApi
            .post('Friends', 'RemoveFriend', { username: friendUsername })
            .subscribe({
                next: () => {
                    this.friendsList.splice(index, 1)
                    this.showRemoveFriendModal = false
                    this.selectedFriendIndex = null
                    this.popupLoader.showPopup(
                        'Sucesso',
                        'Amigo removido com sucesso.'
                    )
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro',
                        'Não foi possível remover o amigo.'
                    )
                }
            })
    }

    viewProfile(id: number) {
        this.router.navigate(['/user-profile/' + id,])
    }
}
