public static class PasswordGenerator {
    
    public static string GeneratePasswordHash(string password) {
        return password;
    }

    public static bool ComparePasswordWithHash(string password, string hashedpassword) {
        if(GeneratePasswordHash(password) == hashedpassword) return true;

        return false;
    }
}