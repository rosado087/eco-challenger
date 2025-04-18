export type TagStyle = 'normal' | 'soft' | 'outline' | 'dash'

export interface Tag {
    id: number
    name?: string
    price: number
    backgroundColor: string
    textColor: string
    icon?: string
    style: TagStyle
    owned?: boolean
    isBeingUsed?: boolean
}
