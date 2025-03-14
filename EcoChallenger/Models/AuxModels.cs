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
        public string Username  {get; set;}
        public string Tag {get; set;}
        public string Email {get; set;}
    }
