using System;
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

        public List<Models.Event> GetEventsForWhisky(int whiskyId)
        {
            var items = from ew in GetAll<EventWhisky>()
                        where ew.WhiskyId == whiskyId
                        select new Models.Event
                        {
                            EventId = ew.Event.EventId,
                            MemberId = ew.Event.MemberId,
                            Description = ew.Event.Description,
                            HostedDate = ew.Event.HostedDate
                        };

            return items.ToList();
        }
        
        public Models.Event InsertEvent(int memberId, string description, System.DateTime hostedDate)
        {
            try
            {
                var hostedEvent = new Event();
                hostedEvent.MemberId = memberId;
                hostedEvent.Description = description;
                hostedEvent.HostedDate = hostedDate;
                hostedEvent.InsertedDate = DateTime.Now;
                hostedEvent.UpdatedDate = DateTime.Now;

                Insert(hostedEvent);

                CommitChanges();

                return new Models.Event
                {
                    EventId = hostedEvent.EventId,
                    MemberId = hostedEvent.MemberId,
                    Description = hostedEvent.Description,
                    HostedDate = hostedEvent.HostedDate
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateEvent(int eventId, int memberId, string description, System.DateTime hostedDate)
        {
            try
            {
                var hostedEvent = GetOne<Event, int>(eventId);
                hostedEvent.MemberId = memberId;
                hostedEvent.Description = description;
                hostedEvent.HostedDate = hostedDate;
                hostedEvent.UpdatedDate = DateTime.Now;

                Update(hostedEvent);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }        
    }
}
