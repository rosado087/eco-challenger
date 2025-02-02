using System.Security.Cryptography;
using System.Text;

public static class PasswordGenerator {
    
    /// <summary>
    /// Converts a plain text password into a SHA256 hash
    /// </summary>
    /// <param name="password">Password in plain text</param>
    /// <returns>Corresponding SHA256</returns>
    public static string GeneratePasswordHash(string password) {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // C# returns a byte array so we need to convert this
            // into a hexadecimal string to be able to store it in the DB
            StringBuilder builder = new StringBuilder();
            foreach (byte byteValue in bytes)
            {
                builder.Append(byteValue.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// Compares a plain text password against its hashed variante to
    /// see if they are the same
    /// </summary>
    /// <param name="password">Password in plain text</param>
    /// <param name="hashedpassword">Password in hashed form</param>
    /// <returns>True if they are the same, otherwise false</returns>
    public static bool ComparePasswordWithHash(string password, string hashedpassword) {
        if(GeneratePasswordHash(password) == hashedpassword) return true;

        return false;
    }

            public static bool ValidatePassword(string password, string hashedpassword)
        {
            return GeneratePasswordHash(password) == hashedpassword;
        }

}