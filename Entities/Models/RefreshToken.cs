using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public bool IsRevoked { get; set; } = false;

        [Required]
        public DateTime Expires { get; set; }

        public User User { get; set; }
    }
}
