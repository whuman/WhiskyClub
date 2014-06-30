using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiskyClub.DataAccess.Models
{
    public class Note
    {
        public int NoteId { get; set; }

        public int WhiskyId { get; set; }

        public int EventId { get; set; }

        public int MemberId { get; set; }

        public string Comment { get; set; }
    }
}
