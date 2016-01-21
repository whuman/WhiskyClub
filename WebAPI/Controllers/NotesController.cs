using System;
using System.Linq;
using System.Web.Http;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Controllers
{
    public class NotesController : ApiController
    {
        private INoteRepository NoteRepository { get; }

        public NotesController() : this(new NoteRepository()) { }

        public NotesController(INoteRepository noteRepository)
        {
            if (noteRepository == null)
            {
                throw new ArgumentNullException(nameof(noteRepository));
            }

            NoteRepository = noteRepository;
        }

        // GET api/<controller>
        public IHttpActionResult GetAll()
        {
            var notes = from note in NoteRepository.GetAllNotes()
                               select new Note
                               {
                                   NoteId = note.NoteId,
                                   WhiskyId = note.WhiskyId,
                                   EventId = note.EventId,
                                   MemberId = note.MemberId,
                                   Comment = note.Comment,
                                   ImageUri = $"{Request.RequestUri}/{note.NoteId}/image"
                               };

            return Ok(notes);
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            try
            {
                var note = NoteRepository.GetNote(id);
                var item = new Note
                {
                    NoteId = note.NoteId,
                    WhiskyId = note.WhiskyId,
                    EventId = note.EventId,
                    MemberId = note.MemberId,
                    Comment = note.Comment,
                    ImageUri = $"{Request.RequestUri}/{note.NoteId}/image",
                    Whisky = null,  // TODO Add additional data - Whisky info
                    Event = null,   // TODO Add additional data - Event info
                    Member = null   // TODO Add additional data - Member info
                };
                return Ok(item);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET custom routing for image
        [Route("notes/{noteId}/image")]
        [HttpGet]
        public IHttpActionResult FindImageForNote(int noteId)
        {
            var noteImage = NoteRepository.GetNoteImage(noteId);

            if (noteImage != null && noteImage.Length > 0)
            {
                return Ok(noteImage);
            }
            else
            {
                return NotFound();
            }
        }

        // GET custom routing from whiskies
        [Route("whiskies/{whiskyId}/notes")]
        [HttpGet]
        public IHttpActionResult FindNotesForEvent(int whiskyId)
        {
            var notes = from note in NoteRepository.GetNotesForWhisky(whiskyId)
                               select new Note
                               {
                                   NoteId = note.NoteId,
                                   WhiskyId = note.WhiskyId,
                                   EventId = note.EventId,
                                   MemberId = note.MemberId,
                                   Comment = note.Comment,
                                   ImageUri = $"{Request.RequestUri}/{note.NoteId}/image"
                               };

            return Ok(notes);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody]Note note)
        {
            if (note == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newNote = NoteRepository.InsertNote(note.WhiskyId, note.EventId, note.MemberId, note.Comment);

            if (newNote != null)
            {
                note.NoteId = newNote.NoteId;

                return Created($"{Request.RequestUri}/{note.NoteId}", note);
            }
            else
            {
                return Conflict();
            }
        }

        // POST custom routing for image
        [Route("notes/{noteId}/image")]
        [HttpPost]
        public IHttpActionResult Post(int noteId)
        {
            byte[] imageArray = Request.Content.ReadAsByteArrayAsync().Result;

            var status = NoteRepository.UpdateNoteImage(noteId, imageArray);
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
        public IHttpActionResult Put(int id, [FromBody]Note note)
        {
            if (note == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != note.NoteId)
            {
                return BadRequest("NoteId does not match");
            }

            var status = NoteRepository.UpdateNote(id, note.WhiskyId, note.EventId, note.MemberId, note.Comment);
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
            var status = NoteRepository.DeleteNote(id);
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