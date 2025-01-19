public class UserToken {
    public enum TokenType {
        AUTH,
        RECOVERY
    }

    public int Id { get; set; }
    public required User User { get; set; }
    public required string Token { get; set; }
    public required TokenType Type { get; set; }
    public required DateTime CreationDate { get; set; }
    public required TimeSpan Duration { get; set; }
}