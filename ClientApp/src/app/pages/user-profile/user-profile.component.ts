import { Component, inject, OnInit } from '@angular/core'
import { provideIcons } from '@ng-icons/core'
import { PopupLoaderService } from '../../services/popup-loader/popup-loader.service'
import { NetApiService } from '../../services/net-api/net-api.service'
import { Router } from '@angular/router'
import { AuthService } from '../../services/auth/auth.service'
import { FormsModule } from '@angular/forms'
import { NgFor, NgIf } from '@angular/common'
import {
    heroUser,
    heroLockClosed,
    heroPencil
} from '@ng-icons/heroicons/outline'

export interface UserList {
    usernames: string[]
}

export interface UserProfileResponse {
    success: boolean
    username: string
    email: string
    points: string
    tag: string
    message: string
}

export interface FriendsResponse {
    success: boolean
    friends: { username: string; email: string }[]
}

@Component({
    selector: 'app-user-profile',
    standalone: true,
    imports: [NgFor, NgIf, FormsModule],
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
    authService = inject(AuthService)

    username: string = ''
    email: string = ''
    points: string = ''
    tag: string = ''
    friendsList: { username: string; email: string }[] = [] // Updated to match API response

    availableTags: string[] = ['Eco-Warrior', 'Nature Lover', 'Green Guru']

    userList: string[] = []
    showAddFriendModal: boolean = false
    showRemoveFriendModal: boolean = false
    selectedFriendIndex: number | null = null
    searchUsername: string = ''
    selectedUser: string = ''

    ngOnInit(): void {
        this.username = this.authService.getUserInfo().username
    }

    /**
     * Load user profile details from API.
     */
    loadUserProfile() {
        this.netApi
            .get<FriendsResponse>('Profile', 'GetFriends', this.username)
            .subscribe({
                next: (data: FriendsResponse) => {
                    if (data.success) {
                        this.friendsList = data.friends
                        console.log(
                            'Lista de amigos carregada:',
                            this.friendsList
                        ) // Debugging
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
     * Load the list of friends for the logged-in user.
     */
    loadFriendsList() {
        if (!this.username) {
            this.popupLoader.showPopup(
                'Erro',
                'Nome de utilizador não encontrado.'
            )
            return
        }

        this.netApi
            .get<FriendsResponse>('Profile', 'GetFriends', this.username)
            .subscribe({
                next: (data: FriendsResponse) => {
                    if (data.success) {
                        this.friendsList = data.friends
                        console.log(
                            'Lista de amigos carregada:',
                            this.friendsList
                        ) // Debugging
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
                        email: ''
                    }) // Email isn't provided by the API, so left empty
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

    /**
     * View the profile of a selected friend.
     * @param username Friend's username
     */
    viewProfile(username: string) {
        this.router.navigate(['/friend-profile', username])
    }

    /**
     * Salva a tag selecionada no perfil do usuário
     */
    saveTag() {
        this.netApi
            .post('Profile', 'UpdateUserTag', {
                email: this.email,
                tag: this.tag
            })
            .subscribe({
                next: () => {
                    this.popupLoader.showPopup(
                        'Sucesso',
                        'Tag atualizada com sucesso.'
                    )
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro',
                        'Erro ao atualizar a tag.'
                    )
                }
            })
    }

    /**
     * Atualiza o nome de utilizador no perfil do usuário.
     */
    updateUsername() {
        if (!this.username || this.username.trim() === '') {
            this.popupLoader.showPopup(
                'Erro',
                'O nome de utilizador não pode estar vazio.'
            )
            return
        }

        this.netApi
            .post('Profile', 'UpdateUsername', {
                email: this.email,
                username: this.username
            })
            .subscribe({
                next: () => {
                    this.popupLoader.showPopup(
                        'Sucesso',
                        'Nome de utilizador atualizado com sucesso.'
                    )
                },
                error: () => {
                    this.popupLoader.showPopup(
                        'Erro',
                        'Não foi possível atualizar o nome de utilizador.'
                    )
                }
            })
    }
}
