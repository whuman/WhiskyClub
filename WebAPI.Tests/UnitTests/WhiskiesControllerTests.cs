﻿using System;
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
    public class WhiskiesControllerTests
    {
        private IWhiskyRepository WhiskyRepo { get; set; }
        private IEventRepository EventRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange
            WhiskyRepo = MockRepository.GenerateMock<IWhiskyRepository>();
            EventRepo = MockRepository.GenerateMock<IEventRepository>();
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllWhiskies()
        {
            var mockedWhiskyList = GetMockedWhiskyList();

            // Arrange           
            WhiskyRepo.Stub(repo => repo.GetAllWhiskies())
                      .Return(mockedWhiskyList);

            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);
            SetupControllerForTests(whiskiesController);

            // Act
            var result = whiskiesController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Whisky>>;

            // Assert
            WhiskyRepo.AssertWasCalled(x => x.GetAllWhiskies());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var whiskyList = result.Content;

            Assert.IsNotNull(whiskyList);
            Assert.AreEqual(whiskyList.Count(), mockedWhiskyList.Count, "Returned list item count does not match");
        }

        [TestMethod]
        public void Get_ShouldFindWhiskyWithAdditionalDetails()
        {
            var mockedWhiskyList = GetMockedWhiskyList();
            var mockedEventList = GetMockedEventList();
            var whiskyId = 3;

            // Arrange 
            WhiskyRepo.Stub(repo => repo.GetWhisky(whiskyId))
                      .Return(mockedWhiskyList.First(mh => mh.WhiskyId == whiskyId));
            EventRepo.Stub(repo => repo.GetEventsForWhisky(whiskyId))
                     .Return(mockedEventList);    // Should be three items

            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);
            SetupControllerForTests(whiskiesController);

            // Act
            var result = whiskiesController.Get(whiskyId) as OkNegotiatedContentResult<API.Whisky>;

            // Assert
            WhiskyRepo.AssertWasCalled(x => x.GetWhisky(whiskyId));   // Not really useful as we don't care how the Repo gets the data
            WhiskyRepo.AssertWasNotCalled(x => x.GetAllWhiskies());  // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var whisky = result.Content;
            Assert.IsNotNull(whisky);
            Assert.AreEqual(whisky.WhiskyId, whiskyId);
            Assert.AreEqual(whisky.ImageUri,
                $"{ConfigurationManager.AppSettings["BaseApiUri"]}{Resources.Whiskies}/{whiskyId}/image");  // Check ImageUri format is correct
            Assert.IsNotNull(whisky.Events);    // Check populated events
            Assert.AreEqual(whisky.Events.Count, mockedEventList.Count);
        }

        [TestMethod]
        public void Get_ShouldNotFindWhisky()
        {
            var whiskyId = 5;

            // Arrange
            WhiskyRepo.Stub(repo => repo.GetWhisky(whiskyId))
                      .Throw(new NullReferenceException());

            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);

            // Act
            var result = whiskiesController.Get(whiskyId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Post_ShouldReturnWhiskyWithCorrectDetails()
        {
            var newWhisky = GetMockedWhisky(1);

            // Arrange
            WhiskyRepo.Stub(repo => repo.InsertWhisky(newWhisky.Name, newWhisky.Brand, newWhisky.Age, newWhisky.Country, newWhisky.Region, newWhisky.Description, newWhisky.Price, newWhisky.Volume))
                      .Return(newWhisky);

            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);
            SetupControllerForTests(whiskiesController);

            // Act 
            var result = whiskiesController.Post(new API.Whisky { Name = newWhisky.Name }) as CreatedNegotiatedContentResult<API.Whisky>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var whisky = result.Content as API.Whisky;
            Assert.IsNotNull(whisky);
            Assert.AreEqual(whisky.WhiskyId, newWhisky.WhiskyId);
            Assert.AreEqual(result.Location.ToString(),
                $"{ConfigurationManager.AppSettings["BaseApiUri"]}{Resources.Whiskies}/{newWhisky.WhiskyId}");
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestForNullWhisky()
        {
            // Arrange
            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);

            // Act 
            var result = whiskiesController.Post(null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnOKRequestForWhiskyUpdate()
        {
            var existingWhisky = GetMockedWhisky(1);

            // Arrange
            WhiskyRepo.Stub(repo => repo.UpdateWhisky(existingWhisky.WhiskyId, existingWhisky.Name, existingWhisky.Brand, existingWhisky.Age, existingWhisky.Country, existingWhisky.Region, existingWhisky.Description, existingWhisky.Price, existingWhisky.Volume))
                      .Return(true);

            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);

            // Act 
            var result = whiskiesController.Put(existingWhisky.WhiskyId, new API.Whisky { WhiskyId = existingWhisky.WhiskyId, Name = existingWhisky.Name }) as OkResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestErrorMessageResultForDifferingWhiskyId()
        {
            // Arrange
            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);

            // Act
            var result = whiskiesController.Put(0, new API.Whisky { WhiskyId = 1, Name = "Whisky Name" }) as BadRequestErrorMessageResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestForNullWhisky()
        {
            // Arrange
            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);

            // Act 
            var result = whiskiesController.Put(0, null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnNotFoundForInvalidWhiskyId()
        {
            var existingWhisky = GetMockedWhisky(1);

            // Arrange
            WhiskyRepo.Stub(repo => repo.UpdateWhisky(existingWhisky.WhiskyId, existingWhisky.Name, existingWhisky.Brand, existingWhisky.Age, existingWhisky.Country, existingWhisky.Region, existingWhisky.Description, existingWhisky.Price, existingWhisky.Volume))
                      .Return(false);

            var whiskiesController = new WhiskiesController(WhiskyRepo, EventRepo);

            // Act 
            var result = whiskiesController.Put(existingWhisky.WhiskyId, new API.Whisky { WhiskyId = existingWhisky.WhiskyId, Name = existingWhisky.Name }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        #region Private Methods

        private List<DAL.Whisky> GetMockedWhiskyList()
        {
            var whiskies = new List<DAL.Whisky> { GetMockedWhisky(3), GetMockedWhisky(2), GetMockedWhisky(1) };

            return whiskies;
        }

        private List<DAL.Event> GetMockedEventList()
        {
            var events = new List<DAL.Event> { GetMockedEvent(4), GetMockedEvent(5), GetMockedEvent(6) };

            return events;
        }

        private DAL.Whisky GetMockedWhisky(int id)
        {
            return new DAL.Whisky
            {
                WhiskyId = id,
                Name = $"Whisky {id}"
            };
        }

        private DAL.Event GetMockedEvent(int id)
        {
            return new DAL.Event
            {
                EventId = id,
                Description = $"Event {id}"
            };
        }

        private static void SetupControllerForTests(ApiController whiskiesController)
        {
            whiskiesController.Request = new HttpRequestMessage();
            whiskiesController.Request.SetConfiguration(new HttpConfiguration());
            whiskiesController.Request.RequestUri = new Uri(
                $"{ConfigurationManager.AppSettings["BaseApiUri"]}{Resources.Whiskies}");
        }

        #endregion        
    }
}
