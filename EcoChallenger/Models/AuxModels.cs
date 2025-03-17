public class SendRecoveryEmailModel {
    public string Email { get; set; }
}

public class SetNewPasswordModel {
    public string Password { get; set; }
    public string Token { get; set; }
}

public class LoginRequestModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponseModel
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public string Username { get; set; }
    public string Email { get; set; } 
}

public class ProfileEditModel{
    public int Id { get; set; }
    public string Username  {get; set; }
    public string Tag {get; set;}

    public void Validate(){
        if(string.IsNullOrEmpty(Username))
            throw new ArgumentException("Username é inválido");

    }
}

public class ProfileFriendModel 
{
    public int Id {get; set; }
    public string FriendUsername {get; set; }
}
public class GAuthModel {
    public string GoogleToken { get; set; }
    public string Email { get; set; }
}