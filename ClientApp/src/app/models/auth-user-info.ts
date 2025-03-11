export class AuthUserInfo {
    public id: number
    public username: string
    public email: string
    public isAdmin: boolean

    public constructor(
        id: number,
        username: string,
        email: string,
        isAdmin: boolean = false
    ) {
        this.id = id
        this.username = username
        this.email = email
        this.isAdmin = isAdmin
    }
}
