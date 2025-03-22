import { Component } from '@angular/core';
import { StoreTagComponent } from "../../components/store-tag/store-tag.component";
import { TagComponent } from "../../components/tag/tag.component";

@Component({
  selector: 'app-store',
  imports: [StoreTagComponent, TagComponent],
  templateUrl: './store.component.html',
  styleUrl: './store.component.css'
})
export class StoreComponent {

}
