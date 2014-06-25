using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiskyClub.DataAccess.Models
{
    public class Whisky
    {
        public int WhiskyId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public int Age { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public int? Volume { get; set; }
    }
}
