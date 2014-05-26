using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiskyClub.DataAccessLayer.Models;

namespace WhiskyClub.DataAccessLayer.Reposistories
{
    public interface IEventRepository : IDisposable
    {
        Event GetEvent(int eventId);

        IList<Event> GetAllEvents();
    }
}
