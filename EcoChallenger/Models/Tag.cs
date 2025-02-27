using System.ComponentModel.DataAnnotations;

public class Tag {
    public int Id { get; set; }
    
    [Required]
    public required string Name { get; set; }
}