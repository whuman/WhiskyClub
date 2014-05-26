using System.Collections.Generic;
using System.Linq;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Reposistories
{
    public class EventRepository : EntityFrameworkRepositoryBase, IEventRepository
    {
        public Models.Event GetEvent(int eventId)
        {
            var entity = GetOne<Event, int>(eventId);

            return new Models.Event
                       {
                           EventId = entity.EventId,
                           HostId = entity.HostId,
                           Description = entity.Description,
                           HostedDate = entity.HostedDate
                       };
        }

        public IList<Models.Event> GetAllEvents()
        {
            var eventItems = from e in GetAll<Event>()
                             select new Models.Event
                                        {
                                            EventId = e.EventId,
                                            HostId = e.HostId,
                                            Description = e.Description,
                                            HostedDate = e.HostedDate
                                        };

            return eventItems.ToList();
        }
    }
}
