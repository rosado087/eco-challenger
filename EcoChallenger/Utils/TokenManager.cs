using Microsoft.EntityFrameworkCore;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public static class TokenManager {

    /// <summary>
    /// Gets a UserToken record from the DB that matches a given token string
    /// </summary>
    /// <param name="ctx">Database context</param>
    /// <param name="token">Token string to search filter the records</param>
    /// <returns>Corresponding UserToken record, or null if nothing was found</returns>
    public static UserToken? GetValidTokenRecord(AppDbContext ctx, string token) {
        var userTokens = ctx.UserTokens
            .Where(ut => ut.Type == UserToken.TokenType.RECOVERY && ut.Token == token)
            .Include(ut => ut.User) //Make sure it loads the user record
            .ToList();

        if(userTokens == null || userTokens.Count() == 0) return null;

        // This is not the best way, I'm not checking if there is more than
        // one record with the same token, but its unlikely it will ever happen
        var userTkn = userTokens.First();

        // No records found, which means its invalid
        if (userTkn == null) return null;

        // The token has expired
        DateTime expirationDate = userTkn.CreationDate + userTkn.Duration;
        if(DateTime.UtcNow >= expirationDate) return null;

        return userTkn;
    }

    /// <summary>
    /// Creates a UserToken string, which consists of a GUID and
    /// stores it in the DB
    /// </summary>
    /// <param name="user">User record to assign the token to</param>
    /// <returns>The created UserToken record</returns>
    public static UserToken CreateUserToken(User user) {
        string newToken = Guid.NewGuid().ToString();

        return new UserToken {
            User = user,
            Token = newToken,
            Type = UserToken.TokenType.RECOVERY,
            CreationDate = DateTime.Now,
            Duration = TimeSpan.FromHours(4) // Token lasts 4 hours
        };
    }

    public static string GenerateToken(User user, IConfiguration configuration)
{
    // Validate the Jwt:Key configuration
    if (string.IsNullOrEmpty(configuration["Jwt:Key"]))
    {
        throw new ArgumentNullException(nameof(configuration), "Jwt:Key configuration is missing.");
    }

    var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

}