using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using API = WhiskyClub.WebAPI.Models;
using DAL = WhiskyClub.DataAccess.Models;
using WhiskyClub.DataAccess.Repositories;
using WhiskyClub.WebAPI.Controllers;

namespace WhiskyClub.WebAPI.Tests
{
    [TestClass]
    public class HostControllerTests
    {
        public IHostRepository HostRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange
            HostRepo = MockRepository.GenerateMock<IHostRepository>();
        }

        [TestMethod]
        public void GetAllHosts_ShouldReturnAllHosts()
        {
            var mockedHostList = GetMockedHostList();

            // Arrange           
            HostRepo.Stub(repo => repo.GetAllHosts())
                    .Return(mockedHostList);

            // Act
            var hostsController = new HostsController(HostRepo);
            var result = hostsController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Host>>;
            
            // Assert
            HostRepo.AssertWasCalled(x => x.GetAllHosts());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var hostList = result.Content as IEnumerable<API.Host>;

            Assert.IsNotNull(hostList);
            Assert.AreEqual(hostList.Count(), mockedHostList.Count, "Returned list item count does not match");
        }

        [TestMethod]
        public void GetHost_ShouldFindHost()
        {
            var mockedHostList = GetMockedHostList();
            var hostId = 3;

            // Arrange 
            HostRepo.Stub(repo => repo.GetHost(hostId))
                    .Return(mockedHostList.First(mh => mh.HostId == hostId));

            // Act
            var hostsController = new HostsController(HostRepo);
            var result = hostsController.Get(hostId) as OkNegotiatedContentResult<API.Host>;

            // Assert
            HostRepo.AssertWasCalled(x => x.GetHost(hostId));   // Not really useful as we don't care how the Repo gets the data
            HostRepo.AssertWasNotCalled(x => x.GetAllHosts());  // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var host = result.Content as API.Host;
            Assert.IsNotNull(host);
            Assert.AreEqual(host.HostId, hostId);
        }

        [TestMethod]
        public void GetHost_ShouldNotFindHost()
        {
            var hostId = 5;

            // Arrange
            HostRepo.Stub(repo => repo.GetHost(hostId))
                    .Throw(new NullReferenceException());

            // Act
            var hostsController = new HostsController(HostRepo);
            var result = hostsController.Get(hostId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        private List<DAL.Host> GetMockedHostList()
        {
            var hosts = new List<DAL.Host>();

            hosts.Add(GetMockedHost(3));
            hosts.Add(GetMockedHost(2));
            hosts.Add(GetMockedHost(1));

            return hosts;
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
