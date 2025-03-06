import { Component, inject, OnInit } from '@angular/core';
import { NgIcon } from '@ng-icons/core';
import { PopupLoaderService } from '../../services/popup-loader.service';
import { NetApiService } from '../../services/net-api.service';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { UserModel } from '../../models/user-model';


export interface UserList {
  usernames: string[];
}

export interface Result {
  success: boolean;
}

interface FriendsResponse {
  success: boolean;
  friends: string[];
  message?: string;
}

interface TagsResponse {
  success: boolean;
  list: string[];
  message?: string;
}

interface ApiResponse {
  success: boolean;
  message?: string;
  error?: string;
}



@Component({
  selector: 'app-user-profile',
  imports: [NgIcon, FormsModule],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css'],
  providers: [

    PopupLoaderService
  ]
})
export class UserProfileComponent implements OnInit {

  netApi = inject(NetApiService);
  popupLoader = inject(PopupLoaderService);
  router = inject(Router);
  authService = inject(AuthService);

  username: string = "";
  email: string = "";
  points: string = "";
  tag: string = "";

  // Estado para controlar a edição do nome de utilizador
  editingUsername: boolean = false;

  // Lista de amigos
  friendsList: { username: string }[] = [];

  userList : string [] = [];

  // Tags
  availableTags: string[] = [];
  selectedTag: string = "";

    // Controle de modais
    showAddFriendModal: boolean = false
    showRemoveFriendModal: boolean = false
    selectedFriendIndex: number | null = null
    searchUsername: string = ''
    selectedUser: string = ''

  ngOnInit(): void {
    this.loadUserProfile();

    //this.getTags();

    // Aguarda username ser carregado antes de buscar amigos
  setTimeout(() => {
    this.loadFollowingList();
  }, 1000);


  }



  /**
   * Carregar informações do perfil do usuário
   */
  loadUserProfile() {
    const email = this.authService.getUserEmail;

    this.netApi.get<UserModel>('Profile', 'GetUserInfo', email)
      .subscribe({
        next: (data) => {
          if (data.success) {
            this.username = data.username;
            this.email = data.email;
            this.points = data.points;
            this.tag = data.tag;
            this.getTags();
          } else {
            this.popupLoader.showPopup('Erro', data.message || 'Ocorreu um erro!');
          }
        },
        error: () => {
          this.popupLoader.showPopup('Erro', 'Não foi possível carregar os dados do perfil.');
        }
      });
  }

  /**
 * Carregar a lista de amigos do usuário logado.
 */
loadFollowingList() {
  if (!this.username) return;

  this.netApi.get<FriendsResponse>('Profile', `GetFriends?username=${this.username}`)
    .subscribe({
      next: (data) => {
        if (data.success) {
          this.friendsList = data.friends.map((friend: string) => ({ username: friend }));
          console.log("Amigos carregados:", this.friendsList);
        } else {
          this.popupLoader.showPopup('Erro', data.message || 'Erro ao carregar amigos.');
        }
      },
      error: () => {
        this.popupLoader.showPopup('Erro', 'Erro ao tentar obter amigos.');
      }
    });
}




      getTags() {
        this.netApi.get<TagsResponse>('Profile', `GetTags/${this.email}`)
          .subscribe({
            next: (data) => {
              console.log("Resposta da API:", data); // <-- Depuração no console
              if (data.success) {
                this.availableTags = data.list; // <-- Ajustado para pegar `data.list`
                this.selectedTag = this.availableTags[0] || ""; // Seleciona a primeira tag
              } else {
                console.error("Erro da API:", data.message);
                this.popupLoader.showPopup('Erro', data.message || 'Não foi possível carregar as tags.');
              }
            },
            error: (err) => {
              console.error("Erro na requisição:", err);
              this.popupLoader.showPopup('Erro', 'Erro ao carregar tags.');
            }
          });
      }





  /**
   * Atualizar nome de utilizador na API
   */
  updateUsername() {
    if (this.username.trim() === "") {
      this.popupLoader.showPopup('Erro', 'O nome de utilizador não pode estar vazio.');
      return;
    }

    this.netApi.post<ApiResponse>('Profile', 'UpdateUsername', { username: this.username })
      .subscribe({
        next: () => {
          this.popupLoader.showPopup('Sucesso', 'Nome de utilizador atualizado com sucesso.');
          this.editingUsername = false; // Oculta o campo de edição após salvar
        },
        error: () => {
          this.popupLoader.showPopup('Erro', 'Não foi possível atualizar o nome de utilizador.');
        }
      });
  }

  /**
   * Salvar a tag selecionada na API
   */
  saveTag() {
    this.netApi.post<ApiResponse>('Profile', 'UpdateTag', { email: this.email, tag: this.selectedTag })
      .subscribe({
        next: () => {
          this.popupLoader.showPopup('Sucesso', 'Tag atualizada com sucesso.');
          this.tag = this.selectedTag;
        },
        error: () => {
          this.popupLoader.showPopup('Erro', 'Não foi possível atualizar a tag.');
        }
      });
  }

    /**
     * Abrir o perfil de um amigo
     */
    viewProfile(username: string) {
        this.router.navigate(['/friend-profile', username])
    }

  /**
   * Buscar usuários pelo nome
   */
  findUser() {
    this.userList = [];
    this.searchUsername = (document.getElementById("InputAdd") as HTMLInputElement).value || "";
    if (this.searchUsername.trim() !== ""){
      this.netApi.post<UserList>('Profile', 'UserList', [this.username, this.searchUsername]).subscribe({

        next: (data) => {
          this.userList = this.userList.concat(data.usernames)
        },
        error: () => {
          this.popupLoader.showPopup(
            'Erro',
            'Isto é um problema.'

          )
        }
      });
    }

  }

  /**
   * Guarda o nome de utilizador para adicionar como amigo.
   */
  selectUser(username: string) {
    this.selectedUser = this.selectedUser.toLowerCase() === username.toLowerCase() ? "" : username;
  }

    /**
     * Adicionar um amigo à lista
     */
    addFriend() {
        if (this.searchUsername.trim() === '') {
            this.popupLoader.showPopup(
                'Erro',
                'Digite um nome de utilizador válido.'
            )
            return
        }

    this.netApi.post<ApiResponse>('Friends', 'AddFriend', { username: this.searchUsername }).subscribe({
      next: () => {
        this.friendsList.push({ username: this.searchUsername });
        this.searchUsername = "";
        this.showAddFriendModal = false;
        this.popupLoader.showPopup('Sucesso', 'Amigo adicionado com sucesso.');
      },
      error: () => {
        this.popupLoader.showPopup('Erro', 'Não foi possível adicionar o amigo.');
      }
    });
  }

  /**
   * Remover um amigo da lista
   */
  removeFriend(index: number | null) {
    if (index === null) {
      this.popupLoader.showPopup('Erro', 'Nenhum usuário selecionado.');
      return;
    }

        const friendUsername = this.friendsList[index]?.username
        if (!friendUsername) return

    this.netApi.post<ApiResponse>('Profile', 'UnfollowUser', [this.username, friendUsername])
      .subscribe({
        next: (data) => {
          if (data.success) {
            this.friendsList.splice(index, 1);
            this.showRemoveFriendModal = false;
            this.selectedFriendIndex = null;
            this.popupLoader.showPopup('Sucesso', 'Você parou de seguir este usuário.');
          } else {
            this.popupLoader.showPopup('Erro', data.error || 'Erro ao parar de seguir.');
          }
        },
        error: () => {
          this.popupLoader.showPopup('Erro', 'Erro ao tentar parar de seguir.');
        }
      });
  }
}


//Test for push
