using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhiskyClub.WebAPI.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime HostedDate { get; set; }

        public Member Member { get; set; }

        public List<Whisky> Whiskies { get; set; }
    }
}