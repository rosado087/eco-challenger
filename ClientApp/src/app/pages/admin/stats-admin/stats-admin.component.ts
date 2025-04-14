import { Component } from '@angular/core';
import { TopPurchasedTagsComponent } from "../../../components/charts/top-purchased-tags/top-purchased-tags.component";
import { TopCompletedChallengesComponent } from "../../../components/charts/top-completed-challenges/top-completed-challenges.component";
import { TopUserMostPointsComponent } from "../../../components/charts/top-users-most-points/top-users-most-points.component";
import { MonthlyActiveUsersComponent } from "../../../components/charts/monthly-active-users/monthly-active-users.component";

@Component({
  selector: 'app-stats-admin',
  imports: [TopPurchasedTagsComponent, TopCompletedChallengesComponent, TopUserMostPointsComponent, MonthlyActiveUsersComponent],
  templateUrl: './stats-admin.component.html',
  styleUrl: './stats-admin.component.css'
})
export class StatsAdminComponent {

}
