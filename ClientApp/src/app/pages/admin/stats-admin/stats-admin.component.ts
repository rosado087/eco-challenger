import { Component } from '@angular/core';
import { TopPurchasedTagsComponent } from "../../../components/charts/top-purchased-tags/top-purchased-tags.component";

@Component({
  selector: 'app-stats-admin',
  imports: [TopPurchasedTagsComponent],
  templateUrl: './stats-admin.component.html',
  styleUrl: './stats-admin.component.css'
})
export class StatsAdminComponent {

}
