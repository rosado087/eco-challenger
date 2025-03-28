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
    public required int Price { get; set; }

    [Required]
    public required string BackgroundColor { get; set; }

    [Required]
    public required string TextColor { get; set; }

    public string? Icon { get; set; }

    public TagStyle Style { get; set; } = TagStyle.NORMAL;
}