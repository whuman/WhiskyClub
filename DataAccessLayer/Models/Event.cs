using System;

namespace WhiskyClub.DataAccessLayer.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public int HostId { get; set; }

        public string Description { get; set; }

        public DateTime HostedDate { get; set; }
    }
}
