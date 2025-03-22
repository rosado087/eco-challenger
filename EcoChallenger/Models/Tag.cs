using System.ComponentModel.DataAnnotations;

public class Tag {
    public enum TagStyle {
        NORMAL,
        SOFT,
        OUTLINE,
        DASH
    }

    public int Id { get; set; }
    
    public string? Name { get; set; }

    [Required]
    public required string Color { get; set; }

    public string? Icon { get; set; }

    public TagStyle Style { get; set; } = TagStyle.NORMAL;
}