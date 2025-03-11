import { AuthUserInfo } from "./auth-user-info"

export interface LoginResponseModel {
    success: boolean
    message?: string
    token: string
    user: AuthUserInfo
}
