using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Controllers
{
    public class EventsController : ApiController
    {
        public IEventRepository EventRepository { get; set; }
        public IMemberRepository MemberRepository { get; set; }

        public EventsController() : this(new EventRepository(), new MemberRepository()) { }

        public EventsController(IEventRepository eventRepository, IMemberRepository membersRepository)
        {
            if (eventRepository == null)
            {
                throw new ArgumentNullException("eventRepository");
            }

            if (membersRepository == null)
            {
                throw new ArgumentNullException("membersRepository");
            }

            EventRepository = eventRepository;
            MemberRepository = membersRepository;
        }

        // GET api/<controller>
        public IHttpActionResult GetAll()
        {
            var events = from e in EventRepository.GetAllEvents()
                         //join h in MemberRepository.GetAllMembers() on e.MemberId equals h.MemberId
                         orderby e.HostedDate descending
                         select new Event
                                    {
                                        EventId = e.EventId,
                                        MemberId = e.MemberId,
                                        Description = e.Description,
                                        HostedDate = e.HostedDate
                                    };

            return Ok(events);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var eventModel = EventRepository.GetEvent(id);
                var item = new Event
                               {
                                   EventId = eventModel.EventId,
                                   MemberId = eventModel.MemberId,
                                   Description = eventModel.Description,
                                   HostedDate = eventModel.HostedDate
                               };

                var memberModel = MemberRepository.GetMember(eventModel.MemberId);
                item.Member = new Member
                                  {
                                      MemberId = memberModel.MemberId,
                                      Name = memberModel.Name
                                  };

                return Ok(item);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]Event hostedEvent)
        {
            if (hostedEvent == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newEvent = EventRepository.InsertEvent(hostedEvent.MemberId, hostedEvent.Description, hostedEvent.HostedDate);

            if (newEvent != null)
            {
                hostedEvent.EventId = newEvent.EventId;

                return Created<Event>(string.Format("{0}/{1}", Request.RequestUri, hostedEvent.EventId), hostedEvent);
            }
            else
            {
                return Conflict();
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(int id, [FromBody]Event hostedEvent)
        {
            if (hostedEvent == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hostedEvent.EventId)
            {
                return BadRequest("EventId does not match");
            }

            var status = EventRepository.UpdateEvent(id, hostedEvent.MemberId, hostedEvent.Description, hostedEvent.HostedDate);
            if (status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}