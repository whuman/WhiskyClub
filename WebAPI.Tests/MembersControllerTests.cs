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
    public class MembersControllerTests
    {
        public IMemberRepository MemberRepo { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            // Arrange
            MemberRepo = MockRepository.GenerateMock<IMemberRepository>();
        }

        [TestMethod]
        public void GetAllMembers_ShouldReturnAllMembers()
        {
            var mockedMemberList = GetMockedMemberList();

            // Arrange           
            MemberRepo.Stub(repo => repo.GetAllMembers())
                    .Return(mockedMemberList);

            // Act
            var hostsController = new MembersController(MemberRepo);
            var result = hostsController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Member>>;
            
            // Assert
            MemberRepo.AssertWasCalled(x => x.GetAllMembers());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var hostList = result.Content as IEnumerable<API.Member>;

            Assert.IsNotNull(hostList);
            Assert.AreEqual(hostList.Count(), mockedMemberList.Count, "Returned list item count does not match");
        }

        [TestMethod]
        public void GetMember_ShouldFindMember()
        {
            var mockedMemberList = GetMockedMemberList();
            var hostId = 3;

            // Arrange 
            MemberRepo.Stub(repo => repo.GetMember(hostId))
                    .Return(mockedMemberList.First(mh => mh.MemberId == hostId));

            // Act
            var hostsController = new MembersController(MemberRepo);
            var result = hostsController.Get(hostId) as OkNegotiatedContentResult<API.Member>;

            // Assert
            MemberRepo.AssertWasCalled(x => x.GetMember(hostId));   // Not really useful as we don't care how the Repo gets the data
            MemberRepo.AssertWasNotCalled(x => x.GetAllMembers());  // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var host = result.Content as API.Member;
            Assert.IsNotNull(host);
            Assert.AreEqual(host.MemberId, hostId);
        }

        [TestMethod]
        public void GetMember_ShouldNotFindMember()
        {
            var hostId = 5;

            // Arrange
            MemberRepo.Stub(repo => repo.GetMember(hostId))
                    .Throw(new NullReferenceException());

            // Act
            var hostsController = new MembersController(MemberRepo);
            var result = hostsController.Get(hostId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        private List<DAL.Member> GetMockedMemberList()
        {
            var hosts = new List<DAL.Member>();

            hosts.Add(GetMockedMember(3));
            hosts.Add(GetMockedMember(2));
            hosts.Add(GetMockedMember(1));

            return hosts;
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
