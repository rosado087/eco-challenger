using EcoChallenger.Models;
using System.ComponentModel.DataAnnotations;

public class User {
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string Username { get; set; }

    [EmailAddress]
    [Required]
    public required string Email { get; set; }

    public string? Password { get; set; }

    public string? GoogleToken { get; set; }

    public int Points { get; set; }

    public bool IsAdmin { get; set; } = false;
}