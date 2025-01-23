export interface PopupButton {
    type: 'yes' | 'no' | 'cancel' | 'ok'
    text: string
    callback?: {() : void}
}
