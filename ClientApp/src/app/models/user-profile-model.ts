export interface UserList {
    usernames: string[]
}

export interface UserProfileResponse {
    success: boolean
    username: string
    email: string
    points: string
    tag: string
    message: string
}

export interface FriendsResponse {
    success: boolean
    friends: { username: string; id: number }[]
}