using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace EcoChallenger.Utils
{
    public static class TokenManager {

        private static JwtSettings? _jwtSettings;

        /// <summary>
        /// Initialize JWT config on project startup
        /// </summary>
        /// <param name="jwtSettings">JWT Settings object</param>
        public static void Initialize(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

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
        /// stores it in the UserToken Model object
        /// </summary>
        /// <param name="user">User record to assign the token to</param>
        /// <returns>The created UserToken record</returns>
        public static UserToken CreateRecoveryUserToken(User user) {
            string newToken = Guid.NewGuid().ToString();
            var type = UserToken.TokenType.RECOVERY;
            
            return new UserToken {
                User = user,
                Token = newToken,
                Type = type,
                CreationDate = DateTime.UtcNow,
                Duration = TimeSpan.FromHours(4) // Token lasts 4 hours
            };
        }

        /// <summary>
        /// Creates a JWT valid token
        /// </summary>
        /// <param name="user">User record to build the token with</param>
        /// <returns>The created JWT token</returns>
        public static string GenerateJWT(User user) {
            if(_jwtSettings == null)
                throw new InvalidOperationException("JWT settings are missing in the configuration.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim> {
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Sub, user.Email),
                new (JwtRegisteredClaimNames.Email, user.Email),
                new ("userid", user.Id.ToString()),
                new ("username", user.Username),
                new ("isAdmin", user.IsAdmin.ToString(), ClaimValueTypes.Boolean),
                new (ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(new TimeSpan(8, 0, 0)),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);

            if(jwt == null) throw new SecurityTokenException("Failed to create JWT token.");
            
            return jwt;
        }
    }
}