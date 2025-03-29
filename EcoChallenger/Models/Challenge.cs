using System.ComponentModel.DataAnnotations;

namespace EcoChallenger.Models
{
    public class Challenge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public string Type { get; set; }
        public int MaxProgress { get; set; }
    }
}
