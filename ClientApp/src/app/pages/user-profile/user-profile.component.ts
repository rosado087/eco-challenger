import { Component, inject, OnInit } from '@angular/core';
import { NgIcon } from '@ng-icons/core';
import { PopupLoaderService } from '../../services/popup-loader.service';
import { NetApiService } from '../../services/net-api.service';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';


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

  // Dados do usuário
  username: string = "Joaquim Dias";
  email: string = "ddd@gmail.com";
  points: number = 350;
  completedChallenges: number = 5;
  tags: string = "Nenhuma tag equipada";

  // Lista de amigos
  friendsList: { username: string }[] = [
    { username: "Amigo1" },
    { username: "Amigo2" },
    { username: "Amigo3" },
    { username: "Amigo4" },
    { username: "Amigo5" }
  ];

  // Controle de modais
  showAddFriendModal: boolean = false;
  showRemoveFriendModal: boolean = false;
  selectedFriendIndex: number | null = null;
  searchUsername: string = "";

  ngOnInit(): void {
    this.loadUserProfile();

    this.friendsList = [
      { username: "Amigo1" },
      { username: "Amigo2" },
      { username: "Amigo3" },
      { username: "Amigo4" },
      { username: "Amigo5" },
      { username: "Amigo6" },
      { username: "Amigo7" },
      { username: "Amigo8" }
    ];

    console.log("🚀 Amigos carregados:", this.friendsList); // <--- Testar se os amigos existem
  }



  /**
   * Carregar informações do perfil do user
   */
  loadUserProfile() {
    // Simulação de requisição à API para buscar dados do user
    this.netApi.get<any>('Profile', 'GetUserProfile').subscribe({
      next: (data) => {
        this.username = data.username;
        this.email = data.email;
        this.points = data.points;
        this.completedChallenges = data.completedChallenges;
        this.tags = data.tags || "Nenhuma tag equipada";
        this.friendsList = data.friends || [];
      },
      error: () => {
        this.popupLoader.showPopup('Erro', 'Não foi possível carregar os dados do perfil.');
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

    this.netApi.post<any>('Profile', 'UpdateUsername', { username: this.username }).subscribe({
      next: () => {
        this.popupLoader.showPopup('Sucesso', 'Nome de utilizador atualizado com sucesso.');
      },
      error: () => {
        this.popupLoader.showPopup('Erro', 'Não foi possível atualizar o nome de utilizador.');
      }
    });
  }

  /**
   * Abrir o perfil de um amigo
   */
  viewProfile(username: string) {
    this.router.navigate(['/friend-profile', username]);
  }

  /**
   * Adicionar um amigo à lista
   */
  addFriend() {
    if (this.searchUsername.trim() === "") {
      this.popupLoader.showPopup('Erro', 'Digite um nome de utilizador válido.');
      return;
    }

    this.netApi.post<any>('Friends', 'AddFriend', { username: this.searchUsername }).subscribe({
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
      this.popupLoader.showPopup('Erro', 'Nenhum amigo selecionado.');
      return;
    }

    const friendUsername = this.friendsList[index]?.username;
    if (!friendUsername) return;

    this.netApi.post<any>('Friends', 'RemoveFriend', { username: friendUsername }).subscribe({
      next: () => {
        this.friendsList = [...this.friendsList.slice(0, index), ...this.friendsList.slice(index + 1)];
        this.showRemoveFriendModal = false;
        this.selectedFriendIndex = null; // Reset após remoção
        this.popupLoader.showPopup('Sucesso', 'Amigo removido com sucesso.');
      },
      error: () => {
        this.popupLoader.showPopup('Erro', 'Não foi possível remover o amigo.');
      }
    });
  }

}
