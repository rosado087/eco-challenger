using System.ComponentModel.DataAnnotations;

public class TagUsers {
    public int Id { get; set; }
    [Required]
    public User User { get; set;}

    [Required]
    public Tag Tag { get; set;}

    [Required]
    public bool SelectedTag { get; set;}
}