using System;
using System.Collections.Generic;

namespace WhiskyClub.WebAPI.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public int MemberId { get; set; }

        public string Description { get; set; }

        public DateTime HostedDate { get; set; }

        public Member Member { get; set; }

        public List<Whisky> Whiskies { get; set; }
    }
}