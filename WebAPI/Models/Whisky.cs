using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhiskyClub.WebAPI.Models
{
    public class Whisky
    {
        public int WhiskyId { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal? Price { get; set; }

        public int? Volume { get; set; }

        public List<Event> Events { get; set; }
    }
}