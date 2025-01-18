import { ApplicationRef, ComponentRef, Directive, EmbeddedViewRef, ViewContainerRef } from '@angular/core';
import { PopupButton } from '../models/popup-button.type';
import { PopupComponent } from '../components/popup/popup.component';

@Directive({
  selector: '[appPopupLoader]'
})
export class PopupLoaderDirective {
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
  
      this.appRef.attachView(this.componentRef.hostView);
  
      const domElem = (this.componentRef.hostView as EmbeddedViewRef<any>)
        .rootNodes[0] as HTMLElement;
  
      document.body.appendChild(domElem);
    }
  
    public hidePopup(): void {
      if(!this.componentRef) return;
  
      this.appRef.detachView(this.componentRef.hostView);
      this.componentRef.destroy();
    }
}
