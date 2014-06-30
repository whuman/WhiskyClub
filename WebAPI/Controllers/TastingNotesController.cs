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
    public class TastingNotesController : ApiController
    {
        public ITastingNoteRepository TastingNoteRepository { get; set; }

        public TastingNotesController() : this(new TastingNoteRepository()) { }

        public TastingNotesController(ITastingNoteRepository tastingNoteRepository)
        {
            if (tastingNoteRepository == null)
            {
                throw new ArgumentNullException("tastingNoteRepository");
            }

            TastingNoteRepository = tastingNoteRepository;
        }

        // GET api/<controller>
        public IHttpActionResult GetAll()
        {
            var tastingNotes = from tastingNote in TastingNoteRepository.GetAllTastingNotes()
                               select new TastingNote
                               {
                                   TastingNoteId = tastingNote.TastingNoteId,
                                   WhiskyId = tastingNote.WhiskyId,
                                   EventId = tastingNote.EventId,
                                   MemberId = tastingNote.MemberId,
                                   Comment = tastingNote.Comment,
                                   ImageUri = string.Format("{0}/{1}/image", Request.RequestUri, tastingNote.TastingNoteId)
                               };

            return Ok(tastingNotes);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var tastingNote = TastingNoteRepository.GetTastingNote(id);
                var item = new TastingNote
                {
                    TastingNoteId = tastingNote.TastingNoteId,
                    WhiskyId = tastingNote.WhiskyId,
                    EventId = tastingNote.EventId,
                    MemberId = tastingNote.MemberId,
                    Comment = tastingNote.Comment,
                    ImageUri = string.Format("{0}/{1}/image", Request.RequestUri, tastingNote.TastingNoteId)
                };

                // TODO Add additional data - Whisky info
                item.Whisky = null;

                // TODO Add additional data - Event info
                item.Event = null;

                // TODO Add additional data - Member info
                item.Member = null;

                return Ok(item);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET custom routing for image
        [Route("tastingnotes/{tastingNoteId}/image")]
        [HttpGet]
        public IHttpActionResult FindImageForTastingNote(int tastingNoteId)
        {
            var tastingNoteImage = TastingNoteRepository.GetTastingNoteImage(tastingNoteId);

            if (tastingNoteImage != null || tastingNoteImage.Length > 0)
            {
                return Ok(tastingNoteImage);
            }
            else
            {
                return NotFound();
            }
        }

        // GET custom routing from whiskies
        [Route("whiskies/{whiskyId}/tastingnotes")]
        [HttpGet]
        public IHttpActionResult FindTastingNotesForEvent(int whiskyId)
        {
            var tastingNotes = from tastingNote in TastingNoteRepository.GetTastingNotesForWhisky(whiskyId)
                               select new TastingNote
                               {
                                   TastingNoteId = tastingNote.TastingNoteId,
                                   WhiskyId = tastingNote.WhiskyId,
                                   EventId = tastingNote.EventId,
                                   MemberId = tastingNote.MemberId,
                                   Comment = tastingNote.Comment,
                                   ImageUri = string.Format("{0}/{1}/image", Request.RequestUri, tastingNote.TastingNoteId)
                               };

            return Ok(tastingNotes);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]TastingNote tastingNote)
        {
            if (tastingNote == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newTastingNote = TastingNoteRepository.InsertTastingNote(tastingNote.WhiskyId, tastingNote.EventId, tastingNote.MemberId, tastingNote.Comment);

            if (newTastingNote != null)
            {
                tastingNote.TastingNoteId = newTastingNote.TastingNoteId;

                return Created<TastingNote>(string.Format("{0}/{1}", Request.RequestUri, tastingNote.TastingNoteId), tastingNote);
            }
            else
            {
                return Conflict();
            }
        }

        // POST custom routing for image
        [Route("tastingnotes/{tastingNoteId}/image")]
        [HttpPost]
        public IHttpActionResult Post(int tastingNoteId)
        {
            byte[] imageArray = Request.Content.ReadAsByteArrayAsync().Result;

            var status = TastingNoteRepository.UpdateTastingNoteImage(tastingNoteId, imageArray);
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
        public IHttpActionResult Put(int id, [FromBody]TastingNote tastingNote)
        {
            if (tastingNote == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tastingNote.TastingNoteId)
            {
                return BadRequest("TastingNoteId does not match");
            }

            var status = TastingNoteRepository.UpdateTastingNote(id, tastingNote.WhiskyId, tastingNote.EventId, tastingNote.MemberId, tastingNote.Comment);
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
            var status = TastingNoteRepository.DeleteTastingNote(id);
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