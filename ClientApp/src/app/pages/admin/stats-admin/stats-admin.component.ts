import { Component } from '@angular/core';
import { TopPurchasedTagsComponent } from "../../../components/charts/top-purchased-tags/top-purchased-tags.component";
import { TopCompletedChallengesComponent } from "../../../components/charts/top-completed-challenges/top-completed-challenges.component";

@Component({
  selector: 'app-stats-admin',
  imports: [TopPurchasedTagsComponent, TopCompletedChallengesComponent],
  templateUrl: './stats-admin.component.html',
  styleUrl: './stats-admin.component.css'
})
export class StatsAdminComponent {

}
