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
    public class TastingNotesControllerTests
    {
        public ITastingNoteRepository TastingNoteRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange
            TastingNoteRepo = MockRepository.GenerateMock<ITastingNoteRepository>();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllTastingNotesWithoutAdditionalDetails()
        {
            var mockedTastingNoteList = GetMockedTastingNoteList();

            // Arrange           
            TastingNoteRepo.Stub(repo => repo.GetAllTastingNotes())
                           .Return(mockedTastingNoteList);

            var tastingNotesController = new TastingNotesController(TastingNoteRepo);
            SetupControllerForTests(tastingNotesController);

            // Act
            var result = tastingNotesController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.TastingNote>>;

            // Assert
            TastingNoteRepo.AssertWasCalled(x => x.GetAllTastingNotes());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var tastingNoteList = result.Content as IEnumerable<API.TastingNote>;

            Assert.IsNotNull(tastingNoteList);
            Assert.AreEqual(tastingNoteList.Count(), mockedTastingNoteList.Count, "Returned list item count does not match");
            Assert.IsNull(tastingNoteList.First().Whisky, "Tasting Note Whisky is not null");
            Assert.IsNull(tastingNoteList.First().Event, "Tasting Note Event is not null");
            Assert.IsNull(tastingNoteList.First().Member, "Tasting Note Member is not null.");
        }

        [TestMethod]
        public void Get_ShouldFindTastingNoteWithAdditionalDetails()
        {
            var mockedTastingNoteList = GetMockedTastingNoteList();
            var tastingNoteId = 3;

            var mockedTastingNote = mockedTastingNoteList.First(tn => tn.TastingNoteId == tastingNoteId);
            var mockedWhisky = GetMockedWhisky(mockedTastingNote.WhiskyId);
            var mockedEvent = GetMockedEvent(mockedTastingNote.EventId);
            var mockedMember = GetMockedMember(mockedTastingNote.MemberId);

            // Arrange 
            TastingNoteRepo.Stub(repo => repo.GetTastingNote(tastingNoteId))
                           .Return(mockedTastingNote);

            var tastingNotesController = new TastingNotesController(TastingNoteRepo);
            SetupControllerForTests(tastingNotesController);

            // Act
            var result = tastingNotesController.Get(tastingNoteId) as OkNegotiatedContentResult<API.TastingNote>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var tastingNote = result.Content as API.TastingNote;
            Assert.IsNotNull(tastingNote);
            Assert.AreEqual(tastingNote.TastingNoteId, tastingNoteId);
            Assert.AreEqual(tastingNote.ImageUri, string.Format("{0}{1}/{2}/image", ConfigurationManager.AppSettings["BaseApiUri"], Resources.TastingNotes, tastingNoteId));  // Check ImageUri format is correct            
            Assert.Inconclusive("Additional details are not implemented");
            ////Assert.AreEqual(tastingNote.Whisky, mockedWhisky);   // Check that additional info is populated
            ////Assert.AreEqual(tastingNote.Event, mockedEvent);     // Check that additional info is populated
            ////Assert.AreEqual(tastingNote.Member, mockedMember);   // Check that additional info is populated
        }

        [TestMethod]
        public void Get_ShouldNotFindTastingNote()
        {
            var tastingNoteId = 5;

            // Arrange
            TastingNoteRepo.Stub(repo => repo.GetTastingNote(tastingNoteId))
                           .Throw(new NullReferenceException());

            var tastingNotesController = new TastingNotesController(TastingNoteRepo);

            // Act
            var result = tastingNotesController.Get(tastingNoteId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Post_ShouldReturnTastingNoteWithCorrectDetails()
        {
            var newTastingNote = GetMockedTastingNote(1, 2, 3, 4);

            // Arrange
            TastingNoteRepo.Stub(repo => repo.InsertTastingNote(newTastingNote.WhiskyId, newTastingNote.EventId, newTastingNote.MemberId, newTastingNote.Comment))
                           .Return(newTastingNote);

            var tastingNotesController = new TastingNotesController(TastingNoteRepo);
            SetupControllerForTests(tastingNotesController);

            // Act 
            var result = tastingNotesController.Post(new API.TastingNote { WhiskyId = newTastingNote.WhiskyId, EventId = newTastingNote.EventId, MemberId = newTastingNote.MemberId, Comment = newTastingNote.Comment }) as CreatedNegotiatedContentResult<API.TastingNote>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var tastingNote = result.Content as API.TastingNote;
            Assert.IsNotNull(tastingNote);
            Assert.AreEqual(tastingNote.TastingNoteId, newTastingNote.TastingNoteId);
            Assert.AreEqual(result.Location.ToString(), string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["BaseApiUri"], Resources.TastingNotes, newTastingNote.TastingNoteId));
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestForNullTastingNote()
        {
            // Arrange
            var tastingNotesController = new TastingNotesController(TastingNoteRepo);

            // Act 
            var result = tastingNotesController.Post(null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnOKRequestForTastingNoteUpdate()
        {
            var existingTastingNote = GetMockedTastingNote(1, 2, 3, 4);

            // Arrange
            TastingNoteRepo.Stub(repo => repo.UpdateTastingNote(existingTastingNote.TastingNoteId, existingTastingNote.WhiskyId, existingTastingNote.EventId, existingTastingNote.MemberId, existingTastingNote.Comment))
                           .Return(true);

            var tastingNotesController = new TastingNotesController(TastingNoteRepo);

            // Act 
            var result = tastingNotesController.Put(existingTastingNote.TastingNoteId, new API.TastingNote { TastingNoteId = existingTastingNote.TastingNoteId, WhiskyId = existingTastingNote.WhiskyId, EventId = existingTastingNote.EventId, MemberId = existingTastingNote.MemberId, Comment = existingTastingNote.Comment }) as OkResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestErrorMessageResultForDifferingTastingNoteId()
        {
            // Arrange
            var tastingNotesController = new TastingNotesController(TastingNoteRepo);

            // Act
            var result = tastingNotesController.Put(0, new API.TastingNote { TastingNoteId = 1, WhiskyId = 2, EventId = 3, MemberId = 4, Comment = "Comment" }) as BadRequestErrorMessageResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestForNullTastingNote()
        {
            // Arrange
            var tastingNotesController = new TastingNotesController(TastingNoteRepo);

            // Act 
            var result = tastingNotesController.Put(0, null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnNotFoundForInvalidTastingNoteId()
        {
            var existingTastingNote = GetMockedTastingNote(1, 2, 3, 4);

            // Arrange
            TastingNoteRepo.Stub(repo => repo.UpdateTastingNote(existingTastingNote.TastingNoteId, existingTastingNote.WhiskyId, existingTastingNote.EventId, existingTastingNote.MemberId, existingTastingNote.Comment))
                           .Return(false);

            var tastingNotesController = new TastingNotesController(TastingNoteRepo);

            // Act 
            var result = tastingNotesController.Put(existingTastingNote.TastingNoteId, new API.TastingNote { TastingNoteId = existingTastingNote.TastingNoteId, WhiskyId = existingTastingNote.WhiskyId, EventId = existingTastingNote.EventId, MemberId = existingTastingNote.MemberId, Comment = existingTastingNote.Comment }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        #region Private Methods

        private List<DAL.TastingNote> GetMockedTastingNoteList()
        {
            var tastingNotes = new List<DAL.TastingNote>();

            tastingNotes.Add(GetMockedTastingNote(1, 1, 2, 3));
            tastingNotes.Add(GetMockedTastingNote(2, 4, 5, 6));
            tastingNotes.Add(GetMockedTastingNote(3, 7, 8, 9));

            return tastingNotes;
        }

        private DAL.TastingNote GetMockedTastingNote(int tastingNoteId, int whiskyId, int eventId, int memberId)
        {
            return new DAL.TastingNote
            {
                TastingNoteId = tastingNoteId,
                WhiskyId = whiskyId,
                EventId = eventId,
                MemberId = memberId,
                Comment = string.Format("TastingNote {0}-{1}-{2}-{3}", tastingNoteId, whiskyId, eventId, memberId)
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

        private static void SetupControllerForTests(ApiController tastingNotesController)
        {
            tastingNotesController.Request = new HttpRequestMessage();
            tastingNotesController.Request.SetConfiguration(new HttpConfiguration());
            tastingNotesController.Request.RequestUri = new Uri(string.Format("{0}{1}", ConfigurationManager.AppSettings["BaseApiUri"], Resources.TastingNotes));
        }

        #endregion
    }
}
