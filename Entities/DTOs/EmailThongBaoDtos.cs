using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    public class CreateEmailThongBaoDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class UpdateEmailThongBaoDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class EmailThongBaoDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}
