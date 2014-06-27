using System;
using System.Collections.Generic;
using WhiskyClub.DataAccess.Models;

namespace WhiskyClub.DataAccess.Repositories
{
    public interface IEventRepository : IDisposable
    {
        Event GetEvent(int eventId);

        List<Event> GetAllEvents();

        List<Event> GetEventsForWhisky(int whiskyId);

        Event InsertEvent(int memberId, string description, DateTime hostedDate);

        bool UpdateEvent(int eventId, int memberId, string description, DateTime hostedDate);
    }
}
