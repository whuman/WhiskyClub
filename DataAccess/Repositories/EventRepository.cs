using System.Collections.Generic;
using System.Linq;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Repositories
{
    public class EventRepository : EntityFrameworkRepositoryBase, IEventRepository
    {
        public Models.Event GetEvent(int eventId)
        {
            var entity = GetOne<Event, int>(eventId);

            return new Models.Event
                       {
                           EventId = entity.EventId,
                           MemberId = entity.MemberId,
                           Description = entity.Description,
                           HostedDate = entity.HostedDate
                       };
        }

        public List<Models.Event> GetAllEvents()
        {
            var eventItems = from e in GetAll<Event>()
                             select new Models.Event
                                        {
                                            EventId = e.EventId,
                                            MemberId = e.MemberId,
                                            Description = e.Description,
                                            HostedDate = e.HostedDate
                                        };

            return eventItems.ToList();
        }
    }
}
