using System.ComponentModel.DataAnnotations;

namespace WhiskyClub.WebAPI.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}