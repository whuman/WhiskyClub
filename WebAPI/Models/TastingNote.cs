using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WhiskyClub.WebAPI.Models
{
    public class TastingNote
    {
        public int TastingNoteId { get; set; }

        [Required]
        public int WhiskyId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public string Comment { get; set; }

        public string ImageUri { get; set; }

        public Whisky Whisky { get; set; }

        public Event Event { get; set; }

        public Member Member { get; set; }
    }
}