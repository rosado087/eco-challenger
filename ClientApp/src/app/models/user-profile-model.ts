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

export interface AddFriendRequest {
    username: string;
    friendUsername: string;
}

export interface AddFriendResponse {
    success: boolean;
    message?: string;
    friendId?: number;
}

export interface RemoveFriendResponse 
{
    success: boolean;
    message?: string;
}