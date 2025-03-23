import { Component, inject, input, output } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms'
import { ButtonComponent } from "../../../components/button/button.component";
import { NgIcon, provideIcons } from '@ng-icons/core'
import {
  heroPencil,
  heroTrash,
  heroMagnifyingGlass
} from '@ng-icons/heroicons/outline'
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service';

@Component({
  selector: 'app-tag-form-modal',
  imports: [NgIcon, ButtonComponent, ReactiveFormsModule],
  providers: [PopupLoaderService, provideIcons({ heroPencil, heroTrash, heroMagnifyingGlass }),],
  templateUrl: './tag-form-modal.component.html',
  styleUrl: './tag-form-modal.component.css'
})
export class TagFormModalComponent {
  popupLoader = inject(PopupLoaderService)
  open = input<boolean>(false)
  tagId = input<number>(0)
  isEditMode = input<boolean>(false)
  closeClick = output()

  tagForm = new FormGroup({
    name: new FormControl('', [
      this.nameOrIconRequired()
    ]),
    color: new FormControl('', [
      Validators.required
    ]),
    icon: new FormControl('', [
      this.nameOrIconRequired()
    ]),
    style: new FormControl('normal', [
      Validators.required
    ]),
    price: new FormControl(0, [
      Validators.required
    ]),
  },
  { validators: this.nameOrIconRequired() })

  nameOrIconRequired(): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {
      const missing = !group.get('name')?.value && group.get('icon')?.value

      return missing ? { nameOrIconRequired: true } : null
    }
  }
  
  submitRecord() {

  }
}
