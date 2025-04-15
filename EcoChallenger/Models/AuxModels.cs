using EcoChallenger.Models;

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
    public bool success { get; set; }
    public string? token { get; set; }
    public LoginResponseUserModel? user { get; set; }


    public class LoginResponseUserModel {
        public int id { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public bool isAdmin { get; set; }
    }
}

public class ProfileEditModel{
    public int? Id { get; set; }
    public string Username  {get; set; }

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

public class TagCRUDModel {
    public string? Name { get; set; }

    public required int Price { get; set; }

    public required string BackgroundColor { get; set; }

    public required string TextColor { get; set; }

    public Tag.TagStyle Style { get; set; } = Tag.TagStyle.NORMAL;

    public IFormFile? Icon { get; set; }
}

 public class ResponseModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class TagUsersTestModel
{

    public int UserId { get; set;}

    public string TagName { get; set;}

    public bool SelectedTag { get; set;}
}

public class ChallengeModel
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int Points { get; set; }

    public string Type { get; set; }
    public int MaxProgress { get; set; } = 1;
    public int UserId {get; set;}

    public void Validate(){
        if(string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(Type) || Points == 0)
            throw new ArgumentException("Argumentos inválidos");

    }
}

public class SelectedTags {
    public required List<int> tagIds { get; set; }
}

public class ChallengesResponse
{
    public bool Success { get; set; }
    public string? Message {get; set; }
    public List<DailyChallengeResponse> DailyChallenges { get; set; } = [];
    public List<WeeklyChallengeResponse> WeeklyChallenges { get; set; } = [];
    
}

public class DailyChallengeResponse
{
    public Challenge Challenge { get; set; } = null!;
    public bool WasConcluded { get; set; }
}

public class WeeklyChallengeResponse
{
    public Challenge Challenge { get; set; } = null!;
    public int Progress { get; set; }
    public bool WasConcluded { get; set; }
}

public class TopInfoTagResponse
{
    public List<InfoTagTest> Tags { get; set; } = [];
}

public class InfoTagTest
{
    public string TagName { get; set; }
    public int Count { get; set; }
}

public class TopInfoChallengeResponse
{
    public List<InfoChallengeTest> Challenges { get; set; } = [];
}

public class InfoChallengeTest
{
    public string ChallengeName { get; set; }
    public int Count { get; set; }
}

public class TopInfoUserResponse
{
    public List<InfoUserTest> Users { get; set; } = [];
}

public class InfoUserTest
{
    public string UserName { get; set; }
    public int Points { get; set; }
}

public class TopInfoFriendResponse
{
    public List<InfoFriendTest> Users { get; set; } = [];
}

public class InfoFriendTest
{
    public string UserName { get; set; }
    public int Points { get; set; }
}

public class TopInfoMonthlyResponse
{
    public List<InfoMonthlyTest> Logins { get; set; } = [];
}

public class InfoMonthlyTest
{
    public string Date { get; set; }
    public int Counts { get; set; }
}