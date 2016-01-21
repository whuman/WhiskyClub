﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Controllers;
using API = WhiskyClub.WebAPI.Models;
using DAL = WhiskyClub.DataAccess.Models;

namespace WhiskyClub.WebAPI.Tests.UnitTests
{
    [TestClass]
    public class EventsControllerTests
    {
        private IEventRepository EventRepo { get; set; }
        private IMemberRepository MemberRepo { get; set; }
        private IWhiskyRepository WhiskyRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange  
            EventRepo = MockRepository.GenerateMock<IEventRepository>();
            MemberRepo = MockRepository.GenerateMock<IMemberRepository>();
            WhiskyRepo = MockRepository.GenerateMock<IWhiskyRepository>();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllEventsWithoutAdditionalDetails()
        {
            var mockedEventList = GetMockedEventList();

            // Arrange           
            EventRepo.Stub(repo => repo.GetAllEvents())
                     .Return(mockedEventList);

            // Act
            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);
            var result = eventsController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Event>>;

            // Assert
            EventRepo.AssertWasCalled(x => x.GetAllEvents());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var eventList = result.Content as IList<API.Event>;

            Assert.IsNotNull(eventList);
            Assert.AreEqual(eventList.Count, mockedEventList.Count, "Returned list item count does not match");
            Assert.IsNull(eventList.First().Member, "Event Member is not null.");
            Assert.IsNull(eventList.First().Whiskies, "Event Whiskies list is not null");
        }

        [TestMethod]
        public void Get_ShouldFindEventWithAdditionalDetails()
        {
            var mockedEventList = GetMockedEventList();
            var mockedMemberList = GetMockedMemberList();
            var mockedWhiskyList = GetMockedWhiskyList();
            var eventId = 3;
            var memberId = mockedEventList.First(me => me.EventId == eventId).MemberId; // Fetch the MemberId from the mockedEventList setup

            // Arrange
            EventRepo.Stub(repo => repo.GetEvent(eventId))
                     .Return(mockedEventList.First(me => me.EventId == eventId));

            MemberRepo.Stub(repo => repo.GetMember(memberId))
                      .Return(mockedMemberList.First(mh => mh.MemberId == memberId));

            WhiskyRepo.Stub(repo => repo.GetWhiskiesForEvent(eventId))
                      .Return(mockedWhiskyList);

            // Act
            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);
            var result = eventsController.Get(eventId) as OkNegotiatedContentResult<API.Event>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var eventItem = result.Content;

            Assert.IsNotNull(eventItem);
            Assert.AreEqual(eventItem.EventId, eventId);
            Assert.IsNotNull(eventItem.Member, "Event Member is null.");
            Assert.AreEqual(eventItem.Member.MemberId, memberId);
            Assert.IsNotNull(eventItem.Whiskies, "Event Whiskies is null.");
            Assert.AreEqual(eventItem.Whiskies.Count, mockedWhiskyList.Count);
        }

        [TestMethod]
        public void Get_ShouldNotFindEvent()
        {
            var eventId = 5;

            // Arrange
            EventRepo.Stub(repo => repo.GetEvent(eventId))
                     .Throw(new NullReferenceException());

            // Act
            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);
            var result = eventsController.Get(eventId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Post_ShouldReturnEventWithCorrectDetails()
        {
            var newEvent = GetMockedEvent(1, 1);

            // Arrange
            EventRepo.Stub(repo => repo.InsertEvent(newEvent.MemberId, newEvent.Description, newEvent.HostedDate))
                     .Return(newEvent);

            var hostedEventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);
            SetupControllerForTests(hostedEventsController);

            // Act 
            var result = hostedEventsController.Post(new API.Event { MemberId = newEvent.MemberId, Description = newEvent.Description, HostedDate = newEvent.HostedDate }) as CreatedNegotiatedContentResult<API.Event>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var hostedEvent = result.Content;
            Assert.IsNotNull(hostedEvent);
            Assert.AreEqual(hostedEvent.EventId, newEvent.EventId); // Test EventId
            Assert.AreEqual(result.Location.ToString(),
                $"{ConfigurationManager.AppSettings["BaseApiUri"]}{Resources.Events}/{newEvent.EventId}");   // Test Location
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestForNullEvent()
        {
            // Arrange
            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);

            // Act 
            var result = eventsController.Post(null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnOKRequestForEventUpdate()
        {
            var existingEvent = GetMockedEvent(1, 1);

            // Arrange
            EventRepo.Stub(repo => repo.UpdateEvent(existingEvent.EventId, existingEvent.MemberId, existingEvent.Description, existingEvent.HostedDate))
                     .Return(true);

            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);

            // Act 
            var result = eventsController.Put(existingEvent.EventId, new API.Event { EventId = existingEvent.EventId, MemberId = existingEvent.MemberId, Description = existingEvent.Description, HostedDate = existingEvent.HostedDate }) as OkResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestErrorMessageResultForDifferingEventId()
        {
            // Arrange
            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);

            // Act
            var result = eventsController.Put(0, new API.Event { EventId = 1 }) as BadRequestErrorMessageResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestForNullEvent()
        {
            // Arrange
            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);

            // Act 
            var result = eventsController.Put(1, null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnNotFoundForInvalidEventId()
        {
            var existingEvent = GetMockedEvent(1, 1);

            // Arrange
            EventRepo.Stub(repo => repo.UpdateEvent(existingEvent.EventId, existingEvent.MemberId, existingEvent.Description, existingEvent.HostedDate))
                     .Return(false);

            var eventsController = new EventsController(EventRepo, MemberRepo, WhiskyRepo);

            // Act 
            var result = eventsController.Put(existingEvent.EventId, new API.Event { EventId = existingEvent.EventId, MemberId = existingEvent.MemberId, Description = existingEvent.Description, HostedDate = existingEvent.HostedDate }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        #region Private Methods

        private List<DAL.Event> GetMockedEventList()
        {
            var events = new List<DAL.Event> { GetMockedEvent(3, 1), GetMockedEvent(2, 2), GetMockedEvent(1, 3) };

            return events;
        }

        private List<DAL.Member> GetMockedMemberList()
        {
            var members = new List<DAL.Member> { GetMockedMember(3), GetMockedMember(2), GetMockedMember(1) };

            return members;
        }

        private List<DAL.Whisky> GetMockedWhiskyList()
        {
            var whiskies = new List<DAL.Whisky> { GetMockedWhisky(3), GetMockedWhisky(2), GetMockedWhisky(1) };

            return whiskies;
        }

        private DAL.Event GetMockedEvent(int eventId, int memberId)
        {
            return new DAL.Event
            {
                EventId = eventId,
                MemberId = memberId,
                Description = $"Event {eventId}",
                HostedDate = DateTime.Now
            };
        }

        private DAL.Member GetMockedMember(int id)
        {
            return new DAL.Member
            {
                MemberId = id,
                Name = $"Member {id}"
            };
        }

        private DAL.Whisky GetMockedWhisky(int id)
        {
            return new DAL.Whisky
            {
                WhiskyId = id,
                Name = $"Whisky {id}"
                // Don't really need to populate the rest of the fields here
            };
        }

        private static void SetupControllerForTests(ApiController eventsController)
        {
            eventsController.Request = new HttpRequestMessage();
            eventsController.Request.SetConfiguration(new HttpConfiguration());
            eventsController.Request.RequestUri = new Uri(
                $"{ConfigurationManager.AppSettings["BaseApiUri"]}{Resources.Events}");
        }

        #endregion

    }
}
