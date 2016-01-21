using System;
using System.Linq;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Controllers
{
    public class WhiskiesController : ApiController
    {
        private IWhiskyRepository WhiskyRepository { get; }
        private IEventRepository EventRepository { get; }

        public WhiskiesController() : this(new WhiskyRepository(), new EventRepository()) { }

        public WhiskiesController(IWhiskyRepository whiskyRepository, IEventRepository eventRepository)
        {
            if (whiskyRepository == null)
            {
                throw new ArgumentNullException(nameof(whiskyRepository));
            }

            if (eventRepository == null)
            {
                throw new ArgumentNullException(nameof(eventRepository));
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
                               ImageUri = $"{Request.RequestUri}/{whisky.WhiskyId}/image"
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
                    ImageUri = $"{Request.RequestUri}/{whisky.WhiskyId}/image"
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

                // TODO : Add additional data - list of Notes

                return Ok(item);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET custom routing for image
        [Route("whiskies/{whiskyId}/image")]
        [HttpGet]
        public IHttpActionResult FindImageForWhisky(int whiskyId)
        {
            // Not really sure this will work as it should be trying to serialize the byte array...
            // Goping to trust in Web API magic here and see what happens.
            var whiskyImage = WhiskyRepository.GetWhiskyImage(whiskyId);

            if (whiskyImage != null && whiskyImage.Length > 0)
            {
                return Ok(whiskyImage);
            }
            else
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

            var newWhisky = WhiskyRepository.InsertWhisky(
                whisky.Name, whisky.Brand, whisky.Age, whisky.Country, whisky.Region, whisky.Description, whisky.Price, whisky.Volume);

            if (newWhisky != null)
            {
                whisky.WhiskyId = newWhisky.WhiskyId;

                return Created($"{Request.RequestUri}/{whisky.WhiskyId}", whisky);
            }
            else
            {
                return Conflict();
            }
        }

        // POST custom routing for image
        [Route("whiskies/{whiskyId}/image")]
        [HttpPost]
        public IHttpActionResult Post(int whiskyId)
        {
            byte[] imageArray = Request.Content.ReadAsByteArrayAsync().Result;

            var status = WhiskyRepository.UpdateWhiskyImage(whiskyId, imageArray);
            if (status)
            {
                return Ok();
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