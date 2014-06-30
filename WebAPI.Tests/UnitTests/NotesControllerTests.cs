using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Controllers;
using API = WhiskyClub.WebAPI.Models;
using DAL = WhiskyClub.DataAccess.Models;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Configuration;

namespace WhiskyClub.WebAPI.Tests.UnitTests
{
    [TestClass]
    public class NotesControllerTests
    {
        public INoteRepository NoteRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange
            NoteRepo = MockRepository.GenerateMock<INoteRepository>();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllNotesWithoutAdditionalDetails()
        {
            var mockedNoteList = GetMockedNoteList();

            // Arrange           
            NoteRepo.Stub(repo => repo.GetAllNotes())
                           .Return(mockedNoteList);

            var notesController = new NotesController(NoteRepo);
            SetupControllerForTests(notesController);

            // Act
            var result = notesController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Note>>;

            // Assert
            NoteRepo.AssertWasCalled(x => x.GetAllNotes());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var noteList = result.Content as IEnumerable<API.Note>;

            Assert.IsNotNull(noteList);
            Assert.AreEqual(noteList.Count(), mockedNoteList.Count, "Returned list item count does not match");
            Assert.IsNull(noteList.First().Whisky, "Note Whisky is not null");
            Assert.IsNull(noteList.First().Event, "Note Event is not null");
            Assert.IsNull(noteList.First().Member, "Note Member is not null.");
        }

        [TestMethod]
        public void Get_ShouldFindNoteWithAdditionalDetails()
        {
            var mockedNoteList = GetMockedNoteList();
            var noteId = 3;

            var mockedNote = mockedNoteList.First(tn => tn.NoteId == noteId);
            var mockedWhisky = GetMockedWhisky(mockedNote.WhiskyId);
            var mockedEvent = GetMockedEvent(mockedNote.EventId);
            var mockedMember = GetMockedMember(mockedNote.MemberId);

            // Arrange 
            NoteRepo.Stub(repo => repo.GetNote(noteId))
                           .Return(mockedNote);

            var notesController = new NotesController(NoteRepo);
            SetupControllerForTests(notesController);

            // Act
            var result = notesController.Get(noteId) as OkNegotiatedContentResult<API.Note>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var note = result.Content as API.Note;
            Assert.IsNotNull(note);
            Assert.AreEqual(note.NoteId, noteId);
            Assert.AreEqual(note.ImageUri, string.Format("{0}{1}/{2}/image", ConfigurationManager.AppSettings["BaseApiUri"], Resources.Notes, noteId));  // Check ImageUri format is correct            
            Assert.Inconclusive("Additional details are not implemented");
            ////Assert.AreEqual(note.Whisky, mockedWhisky);   // Check that additional info is populated
            ////Assert.AreEqual(note.Event, mockedEvent);     // Check that additional info is populated
            ////Assert.AreEqual(note.Member, mockedMember);   // Check that additional info is populated
        }

        [TestMethod]
        public void Get_ShouldNotFindNote()
        {
            var noteId = 5;

            // Arrange
            NoteRepo.Stub(repo => repo.GetNote(noteId))
                           .Throw(new NullReferenceException());

            var notesController = new NotesController(NoteRepo);

            // Act
            var result = notesController.Get(noteId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Post_ShouldReturnNoteWithCorrectDetails()
        {
            var newNote = GetMockedNote(1, 2, 3, 4);

            // Arrange
            NoteRepo.Stub(repo => repo.InsertNote(newNote.WhiskyId, newNote.EventId, newNote.MemberId, newNote.Comment))
                           .Return(newNote);

            var notesController = new NotesController(NoteRepo);
            SetupControllerForTests(notesController);

            // Act 
            var result = notesController.Post(new API.Note { WhiskyId = newNote.WhiskyId, EventId = newNote.EventId, MemberId = newNote.MemberId, Comment = newNote.Comment }) as CreatedNegotiatedContentResult<API.Note>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var note = result.Content as API.Note;
            Assert.IsNotNull(note);
            Assert.AreEqual(note.NoteId, newNote.NoteId);
            Assert.AreEqual(result.Location.ToString(), string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["BaseApiUri"], Resources.Notes, newNote.NoteId));
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestForNullNote()
        {
            // Arrange
            var notesController = new NotesController(NoteRepo);

            // Act 
            var result = notesController.Post(null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnOKRequestForNoteUpdate()
        {
            var existingNote = GetMockedNote(1, 2, 3, 4);

            // Arrange
            NoteRepo.Stub(repo => repo.UpdateNote(existingNote.NoteId, existingNote.WhiskyId, existingNote.EventId, existingNote.MemberId, existingNote.Comment))
                           .Return(true);

            var notesController = new NotesController(NoteRepo);

            // Act 
            var result = notesController.Put(existingNote.NoteId, new API.Note { NoteId = existingNote.NoteId, WhiskyId = existingNote.WhiskyId, EventId = existingNote.EventId, MemberId = existingNote.MemberId, Comment = existingNote.Comment }) as OkResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestErrorMessageResultForDifferingNoteId()
        {
            // Arrange
            var notesController = new NotesController(NoteRepo);

            // Act
            var result = notesController.Put(0, new API.Note { NoteId = 1, WhiskyId = 2, EventId = 3, MemberId = 4, Comment = "Comment" }) as BadRequestErrorMessageResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestForNullNote()
        {
            // Arrange
            var notesController = new NotesController(NoteRepo);

            // Act 
            var result = notesController.Put(0, null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnNotFoundForInvalidNoteId()
        {
            var existingNote = GetMockedNote(1, 2, 3, 4);

            // Arrange
            NoteRepo.Stub(repo => repo.UpdateNote(existingNote.NoteId, existingNote.WhiskyId, existingNote.EventId, existingNote.MemberId, existingNote.Comment))
                           .Return(false);

            var notesController = new NotesController(NoteRepo);

            // Act 
            var result = notesController.Put(existingNote.NoteId, new API.Note { NoteId = existingNote.NoteId, WhiskyId = existingNote.WhiskyId, EventId = existingNote.EventId, MemberId = existingNote.MemberId, Comment = existingNote.Comment }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        #region Private Methods

        private List<DAL.Note> GetMockedNoteList()
        {
            var notes = new List<DAL.Note>();

            notes.Add(GetMockedNote(1, 1, 2, 3));
            notes.Add(GetMockedNote(2, 4, 5, 6));
            notes.Add(GetMockedNote(3, 7, 8, 9));

            return notes;
        }

        private DAL.Note GetMockedNote(int noteId, int whiskyId, int eventId, int memberId)
        {
            return new DAL.Note
            {
                NoteId = noteId,
                WhiskyId = whiskyId,
                EventId = eventId,
                MemberId = memberId,
                Comment = string.Format("Note {0}-{1}-{2}-{3}", noteId, whiskyId, eventId, memberId)
            };
        }

        private DAL.Whisky GetMockedWhisky(int id)
        {
            return new DAL.Whisky
            {
                WhiskyId = id,
                Name = string.Format("Whisky {0}", id)
            };
        }

        private DAL.Event GetMockedEvent(int id)
        {
            return new DAL.Event
            {
                EventId = id,
                Description = string.Format("Event {0}", id)
            };
        }

        private DAL.Member GetMockedMember(int id)
        {
            return new DAL.Member
            {
                MemberId = id,
                Name = string.Format("Member {0}", id)
            };
        }

        private static void SetupControllerForTests(ApiController notesController)
        {
            notesController.Request = new HttpRequestMessage();
            notesController.Request.SetConfiguration(new HttpConfiguration());
            notesController.Request.RequestUri = new Uri(string.Format("{0}{1}", ConfigurationManager.AppSettings["BaseApiUri"], Resources.Notes));
        }

        #endregion
    }
}
