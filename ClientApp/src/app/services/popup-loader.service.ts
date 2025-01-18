import { ApplicationRef, ComponentRef, Directive, EmbeddedViewRef, Injectable, ViewContainerRef } from '@angular/core';
import { PopupButton } from '../models/popup-button.type';
import { PopupComponent } from '../components/popup/popup.component';

@Injectable({
  providedIn: 'root'
})
export class PopupLoaderService {
  private componentRef: ComponentRef<any> | undefined;

  constructor(
    private viewContainerRef: ViewContainerRef,
    private appRef: ApplicationRef
  ) {}

  public showPopup(message: string, buttons?: PopupButton[]): void {
    this.componentRef = this.viewContainerRef.createComponent(PopupComponent)

    // We need to inject the props into the component
    // and this way of doing it works, although its a bit nasty
    this.componentRef.instance.message = message;
    this.componentRef.instance.buttons = buttons

    const domElem = (this.componentRef.hostView as EmbeddedViewRef<any>)
      .rootNodes[0] as HTMLElement;

    // Add the popup to the top of the DOM inside the designated
    // container. In case it doesn't find it (it should never happen)
    // it will just add it to wherever
    const container = document.getElementById('popup-container');
    if(!container) document.body.appendChild(domElem);
    else container.appendChild(domElem);    
  }

  public hidePopup(): void {
    if(!this.componentRef) return;

    this.componentRef.destroy();
  }
}

