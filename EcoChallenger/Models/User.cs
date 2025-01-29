using System.ComponentModel.DataAnnotations;

public class User {
    public int Id { get; set; }
    
    [Required]
    public required string Username { get; set; }

    [EmailAddress]
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}