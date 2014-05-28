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
        public IHostRepository HostRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange  
            EventRepo = MockRepository.GenerateMock<IEventRepository>();
            HostRepo = MockRepository.GenerateMock<IHostRepository>();
        }

        [TestMethod]
        public void GetAllEvents_ShouldReturnAllEventsWithoutHostDetails()
        {
            var mockedEventList = GetMockedEventList();

            // Arrange           
            EventRepo.Stub(repo => repo.GetAllEvents())
                     .Return(mockedEventList);

            // Act
            var eventsController = new EventsController(EventRepo, HostRepo);
            var result = eventsController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Event>>;

            // Assert
            EventRepo.AssertWasCalled(x => x.GetAllEvents());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var eventList = result.Content as IEnumerable<API.Event>;

            Assert.IsNotNull(eventList);
            Assert.AreEqual(eventList.Count(), mockedEventList.Count, "Returned list item count does not match");
            Assert.IsNull(eventList.First().Host, "Event Host is not null.");
        }

        [TestMethod]
        public void GetEvent_ShouldFindEventWithHostDetails()
        {
            var mockedEventList = GetMockedEventList();
            var mockedHostList = GetMockedHostList();
            var eventId = 3;
            var hostId = mockedEventList.First(me => me.EventId == eventId).HostId; // Fetch the HostId from the mockedEventList setup

            // Arrange
            EventRepo.Stub(repo => repo.GetEvent(eventId))
                     .Return(mockedEventList.First(me => me.EventId == eventId));

            HostRepo.Stub(repo => repo.GetHost(hostId))
                    .Return(mockedHostList.First(mh => mh.HostId == hostId));

            // Act
            var eventsController = new EventsController(EventRepo, HostRepo);
            var result = eventsController.Get(eventId) as OkNegotiatedContentResult<API.Event>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var eventItem = result.Content as API.Event;

            Assert.IsNotNull(eventItem);
            Assert.AreEqual(eventItem.EventId, eventId);
            Assert.IsNotNull(eventItem.Host, "Event Host is null.");
            Assert.AreEqual(eventItem.Host.HostId, hostId);
        }

        [TestMethod]
        public void GetEvent_ShouldNotFindEvent()
        {
            var eventId = 5;

            // Arrange
            EventRepo.Stub(repo => repo.GetEvent(eventId))
                     .Throw(new NullReferenceException());

            // Act
            var eventsController = new EventsController(EventRepo, HostRepo);
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

        private List<DAL.Host> GetMockedHostList()
        {
            var hosts = new List<DAL.Host>();

            hosts.Add(GetMockedHost(3));
            hosts.Add(GetMockedHost(2));
            hosts.Add(GetMockedHost(1));

            return hosts;
        }

        private DAL.Event GetMockedEvent(int eventId, int hostId)
        {
            return new DAL.Event
                       {
                           EventId = eventId,
                           Description = string.Format("Event {0}", eventId),
                           HostedDate = DateTime.Now,
                           HostId = hostId
                       };
        }

        private DAL.Host GetMockedHost(int id)
        {
            return new DAL.Host
                       {
                           HostId = id,
                           Name = string.Format("Host {0}", id)
                       };
        }

    }
}
