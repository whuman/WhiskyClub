using System;
using System.Linq;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Controllers
{
    public class EventsController : ApiController
    {
        private IEventRepository EventRepository { get; }
        private IMemberRepository MemberRepository { get; }
        private IWhiskyRepository WhiskyRepository { get; }

        public EventsController() : this(new EventRepository(), new MemberRepository(), new WhiskyRepository()) { }

        public EventsController(IEventRepository eventRepository, IMemberRepository membersRepository, IWhiskyRepository whiskRepository)
        {
            if (eventRepository == null)
            {
                throw new ArgumentNullException(nameof(eventRepository));
            }

            if (membersRepository == null)
            {
                throw new ArgumentNullException(nameof(membersRepository));
            }

            if (whiskRepository == null)
            {
                throw new ArgumentNullException(nameof(whiskRepository));
            }

            EventRepository = eventRepository;
            MemberRepository = membersRepository;
            WhiskyRepository = whiskRepository;
        }

        // GET api/<controller>
        public IHttpActionResult GetAll()
        {
            // Do not return any additional event data (ie. Member info or Whiskies)
            var events = from e in EventRepository.GetAllEvents()
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
                var hostedEvent = EventRepository.GetEvent(id);
                var item = new Event
                               {
                                   EventId = hostedEvent.EventId,
                                   MemberId = hostedEvent.MemberId,
                                   Description = hostedEvent.Description,
                                   HostedDate = hostedEvent.HostedDate
                               };

                // Add additional data - Member info
                var member = MemberRepository.GetMember(hostedEvent.MemberId);
                item.Member = new Member
                                  {
                                      MemberId = member.MemberId,
                                      Name = member.Name
                                  };

                // Add additional data - list of Whiskies
                var whiskies = from whisky in WhiskyRepository.GetWhiskiesForEvent(hostedEvent.EventId)
                               select new Whisky
                                          {
                                              WhiskyId = whisky.WhiskyId,
                                              Name = whisky.Name,
                                              Brand = whisky.Brand,
                                              Age = whisky.Age,
                                              Country = whisky.Country,
                                              Region = whisky.Region,
                                              Description = whisky.Description,
                                              Price = whisky.Price,
                                              Volume = whisky.Volume
                                          };

                item.Whiskies = whiskies.ToList();

                return Ok(item);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        // GET custom routing from whiskies
        [Route("whiskies/{whiskyId}/events")]
        [HttpGet]
        public IHttpActionResult FindEventsForWhisky(int whiskyId)
        {
            var events = from e in EventRepository.GetEventsForWhisky(whiskyId)
                         select new Event
                                    {
                                        EventId = e.EventId,
                                        MemberId = e.MemberId,
                                        Description = e.Description,
                                        HostedDate = e.HostedDate
                                    };

            return Ok(events);
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

                //return Created(string.Format("{0}/{1}", Request.RequestUri, hostedEvent.EventId), hostedEvent);
                return Created($"{Request.RequestUri}/{hostedEvent.EventId}", hostedEvent);
            }
            else
            {
                return Conflict();
            }
        }

        // POST custom routing from whiskies
        [Route("whiskies/{whiskyId}/events/{eventId}")]
        [HttpPost]
        public IHttpActionResult AddEventToWhisky(int eventId, int whiskyId)
        {
            var status = WhiskyRepository.AddEventWhisky(eventId, whiskyId);

            if (status)
            {
                return Ok();
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

        // DELETE custom routing from whiskies
        [Route("whiskies/{whiskyId}/events/{eventId}")]
        [HttpDelete]
        public IHttpActionResult RemoveEventFromWhisky(int eventId, int whiskyId)
        {
            var status = WhiskyRepository.RemoveEventWhisky(eventId, whiskyId);
            if (status)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }        
    }
}