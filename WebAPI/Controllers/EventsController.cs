using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Controllers
{
    public class EventsController : ApiController
    {
        private IEventRepository _eventRepo;
        private IHostRepository _hostRepo;

        public EventsController()
        {
            _eventRepo = new EventRepository();
            _hostRepo = new HostRepository();
        }

        // GET api/<controller>
        public IEnumerable<Event> Get()
        {
            var events = from e in _eventRepo.GetAllEvents()
                         select new Event
                                    {
                                        EventId = e.EventId,
                                        Description = e.Description,
                                        HostedDate = e.HostedDate
                                    };

            return events.ToList();
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var dalEvent = _eventRepo.GetEvent(id);
                var item = new Event
                               {
                                   EventId = dalEvent.EventId,
                                   Description = dalEvent.Description,
                                   HostedDate = dalEvent.HostedDate
                               };

                var dalHost = _hostRepo.GetHost(dalEvent.HostId);
                item.Host = new Host
                                {
                                    HostId = dalHost.HostId,
                                    Name = dalHost.Name
                                };

                return Ok(item);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        ////// POST api/<controller>
        ////public void Post([FromBody]string value)
        ////{
        ////}

        ////// PUT api/<controller>/5
        ////public void Put(int id, [FromBody]string value)
        ////{
        ////}

        ////// DELETE api/<controller>/5
        ////public void Delete(int id)
        ////{
        ////}
    }
}