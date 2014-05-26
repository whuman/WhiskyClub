using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiskyClub.DataAccess.Models;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface IEventRepository : IDisposable
    {
        Event GetEvent(int eventId);

        List<Event> GetAllEvents();
    }
}
