public class UserToken {
    public enum TokenType {
        AUTH,
        RECOVERY
    }

    public int Id { get; set; }
    public required User User { get; set; }
    public required string Token { get; set; }
    public required TokenType Type { get; set; }
}