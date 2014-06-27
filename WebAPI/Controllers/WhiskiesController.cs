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
    public class WhiskiesController : ApiController
    {
        public IWhiskyRepository WhiskyRepository { get; set; }
        public IEventRepository EventRepository { get; set; }

        public WhiskiesController() : this(new WhiskyRepository(), new EventRepository()) { }

        public WhiskiesController(IWhiskyRepository whiskyRepository, IEventRepository eventRepository)
        {
            if (whiskyRepository == null)
            {
                throw new ArgumentNullException("whiskyRepository");
            }

            if (eventRepository == null)
            {
                throw new ArgumentNullException("eventRepository");
            }

            WhiskyRepository = whiskyRepository;
            EventRepository = eventRepository;
        }

        // GET api/<controller>
        public IHttpActionResult GetAll()
        {
            var whiskies = from whisky in WhiskyRepository.GetAllWhiskies()
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
                               Volume = whisky.Volume,
                               ImageUri = string.Format("{0}/{1}/image", Request.RequestUri, whisky.WhiskyId)
                           };

            return Ok(whiskies);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var whisky = WhiskyRepository.GetWhisky(id);
                var item = new Whisky
                {
                    WhiskyId = whisky.WhiskyId,
                    Name = whisky.Name,
                    Brand = whisky.Brand,
                    Age = whisky.Age,
                    Country = whisky.Country,
                    Region = whisky.Region,
                    Description = whisky.Description,
                    Price = whisky.Price,
                    Volume = whisky.Volume,
                    ImageUri = string.Format("{0}/{1}/image", Request.RequestUri, whisky.WhiskyId)
                };

                // Add additional data - list of Events
                var events = from e in EventRepository.GetEventsForWhisky(item.WhiskyId)
                             select new Event
                             {
                                 EventId = e.EventId,
                                 MemberId = e.MemberId,
                                 Description = e.Description,
                                 HostedDate = e.HostedDate
                             };

                item.Events = events.ToList();

                // TODO : Add additional data - list of TastingNotes

                return Ok(item);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        // GET custom routing from events
        [Route("events/{eventId}/whiskies")]
        [HttpGet]
        public IHttpActionResult FindWhiskiesForEvent(int eventId)
        {
            var whiskies = from whisky in WhiskyRepository.GetWhiskiesForEvent(eventId)
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
                               Volume = whisky.Volume,
                               ImageUri = string.Format("{0}/{1}/image", Request.RequestUri, whisky.WhiskyId)
                           };

            return Ok(whiskies);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]Whisky whisky)
        {
            if (whisky == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newWhisky = WhiskyRepository.InsertWhisky(whisky.Name, whisky.Brand, whisky.Age, whisky.Country, whisky.Region, whisky.Description, whisky.Price, whisky.Volume);

            if (newWhisky != null)
            {
                whisky.WhiskyId = newWhisky.WhiskyId;

                return Created<Whisky>(string.Format("{0}/{1}", Request.RequestUri, whisky.WhiskyId), whisky);
            }
            else
            {
                return Conflict();
            }
        }

        // POST custom routing from events
        [Route("events/{eventId}/whiskies/{whiskyId}")]
        [HttpPost]
        public IHttpActionResult AddWhiskyToEvent(int eventId, int whiskyId)
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
        public IHttpActionResult Put(int id, [FromBody]Whisky whisky)
        {
            if (whisky == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != whisky.WhiskyId)
            {
                return BadRequest("WhiskyId does not match");
            }

            var status = WhiskyRepository.UpdateWhisky(id, whisky.Name, whisky.Brand, whisky.Age, whisky.Country, whisky.Region, whisky.Description, whisky.Price, whisky.Volume);
            if (status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            var status = WhiskyRepository.DeleteWhisky(id);
            if (status)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        // DELETE custom routing from events
        [Route("events/{eventId}/whiskies/{whiskyId}")]
        [HttpDelete]
        public IHttpActionResult RemoveWhiskyFromEvent(int eventId, int whiskyId)
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