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

        public WhiskiesController() : this(new WhiskyRepository()) { }

        public WhiskiesController(IWhiskyRepository whiskyRepository)
        {
            if (whiskyRepository == null)
            {
                throw new ArgumentNullException("whiskyRepository");
            }

            WhiskyRepository = whiskyRepository;
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
                               Volume = whisky.Volume                               
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
                    Volume = whisky.Volume 
                };

                // TODO : Load Event details for this whisky

                return Ok(item);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
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
    }
}