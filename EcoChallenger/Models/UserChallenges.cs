using System.ComponentModel.DataAnnotations;

namespace EcoChallenger.Models
{
    public class UserChallenges
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Challenge Challenge {  get; set; }

        [Required]
        public User User { get; set; }

        public int Progress { get; set; }

        [Required]
        public bool WasConcluded { get; set; }

    }
}
