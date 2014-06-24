using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Controllers;
using API = WhiskyClub.WebAPI.Models;
using DAL = WhiskyClub.DataAccess.Models;

namespace WhiskyClub.WebAPI.Tests
{
    [TestClass]
    public class EventsControllerTests
    {
        public IEventRepository EventRepo { get; set; }
        public IMemberRepository MemberRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange  
            EventRepo = MockRepository.GenerateMock<IEventRepository>();
            MemberRepo = MockRepository.GenerateMock<IMemberRepository>();
        }

        [TestMethod]
        public void GetAllEvents_ShouldReturnAllEventsWithoutMemberDetails()
        {
            var mockedEventList = GetMockedEventList();

            // Arrange           
            EventRepo.Stub(repo => repo.GetAllEvents())
                     .Return(mockedEventList);

            // Act
            var eventsController = new EventsController(EventRepo, MemberRepo);
            var result = eventsController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Event>>;

            // Assert
            EventRepo.AssertWasCalled(x => x.GetAllEvents());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var eventList = result.Content as IEnumerable<API.Event>;

            Assert.IsNotNull(eventList);
            Assert.AreEqual(eventList.Count(), mockedEventList.Count, "Returned list item count does not match");
            Assert.IsNull(eventList.First().Member, "Event Member is not null.");
        }

        [TestMethod]
        public void GetEvent_ShouldFindEventWithMemberDetails()
        {
            var mockedEventList = GetMockedEventList();
            var mockedMemberList = GetMockedMemberList();
            var eventId = 3;
            var hostId = mockedEventList.First(me => me.EventId == eventId).MemberId; // Fetch the MemberId from the mockedEventList setup

            // Arrange
            EventRepo.Stub(repo => repo.GetEvent(eventId))
                     .Return(mockedEventList.First(me => me.EventId == eventId));

            MemberRepo.Stub(repo => repo.GetMember(hostId))
                    .Return(mockedMemberList.First(mh => mh.MemberId == hostId));

            // Act
            var eventsController = new EventsController(EventRepo, MemberRepo);
            var result = eventsController.Get(eventId) as OkNegotiatedContentResult<API.Event>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var eventItem = result.Content as API.Event;

            Assert.IsNotNull(eventItem);
            Assert.AreEqual(eventItem.EventId, eventId);
            Assert.IsNotNull(eventItem.Member, "Event Member is null.");
            Assert.AreEqual(eventItem.Member.MemberId, hostId);
        }

        [TestMethod]
        public void GetEvent_ShouldNotFindEvent()
        {
            var eventId = 5;

            // Arrange
            EventRepo.Stub(repo => repo.GetEvent(eventId))
                     .Throw(new NullReferenceException());

            // Act
            var eventsController = new EventsController(EventRepo, MemberRepo);
            var result = eventsController.Get(eventId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        private List<DAL.Event> GetMockedEventList()
        {
            var events = new List<DAL.Event>();

            events.Add(GetMockedEvent(3, 1));
            events.Add(GetMockedEvent(2, 2));
            events.Add(GetMockedEvent(1, 3));

            return events;
        }

        private List<DAL.Member> GetMockedMemberList()
        {
            var hosts = new List<DAL.Member>();

            hosts.Add(GetMockedMember(3));
            hosts.Add(GetMockedMember(2));
            hosts.Add(GetMockedMember(1));

            return hosts;
        }

        private DAL.Event GetMockedEvent(int eventId, int hostId)
        {
            return new DAL.Event
                       {
                           EventId = eventId,
                           Description = string.Format("Event {0}", eventId),
                           HostedDate = DateTime.Now,
                           MemberId = hostId
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

    }
}
