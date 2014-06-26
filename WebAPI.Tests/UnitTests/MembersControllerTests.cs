using System;
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
        public void GetAll_ShouldReturnAllMembers()
        {
            var mockedMemberList = GetMockedMemberList();

            // Arrange           
            MemberRepo.Stub(repo => repo.GetAllMembers())
                      .Return(mockedMemberList);

            var membersController = new MembersController(MemberRepo);

            // Act
            var result = membersController.GetAll() as OkNegotiatedContentResult<IEnumerable<API.Member>>;

            // Assert
            MemberRepo.AssertWasCalled(x => x.GetAllMembers());   // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var memberList = result.Content as IEnumerable<API.Member>;

            Assert.IsNotNull(memberList);
            Assert.AreEqual(memberList.Count(), mockedMemberList.Count, "Returned list item count does not match");
        }

        [TestMethod]
        public void Get_ShouldFindMember()
        {
            var mockedMemberList = GetMockedMemberList();
            var memberId = 3;

            // Arrange 
            MemberRepo.Stub(repo => repo.GetMember(memberId))
                      .Return(mockedMemberList.First(mh => mh.MemberId == memberId));

            var membersController = new MembersController(MemberRepo);

            // Act
            var result = membersController.Get(memberId) as OkNegotiatedContentResult<API.Member>;

            // Assert
            MemberRepo.AssertWasCalled(x => x.GetMember(memberId));   // Not really useful as we don't care how the Repo gets the data
            MemberRepo.AssertWasNotCalled(x => x.GetAllMembers());  // Not really useful as we don't care how the Repo gets the data

            Assert.IsNotNull(result, "Result was not of the correct type.");

            var member = result.Content as API.Member;
            Assert.IsNotNull(member);
            Assert.AreEqual(member.MemberId, memberId);
        }

        [TestMethod]
        public void Get_ShouldNotFindMember()
        {
            var memberId = 5;

            // Arrange
            MemberRepo.Stub(repo => repo.GetMember(memberId))
                      .Throw(new NullReferenceException());

            var membersController = new MembersController(MemberRepo);

            // Act
            var result = membersController.Get(memberId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Post_ShouldReturnMemberWithCorrectDetails()
        {
            var newMember = GetMockedMember(1);

            // Arrange
            MemberRepo.Stub(repo => repo.InsertMember(newMember.Name))
                      .Return(newMember);

            var membersController = new MembersController(MemberRepo);
            SetupControllerForTests(membersController);

            // Act 
            var result = membersController.Post(new API.Member { Name = newMember.Name }) as CreatedNegotiatedContentResult<API.Member>;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");

            var member = result.Content as API.Member;
            Assert.IsNotNull(member);
            Assert.AreEqual(member.MemberId, newMember.MemberId);
            Assert.AreEqual(result.Location.ToString(), string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["BaseApiUri"], Resources.Members, newMember.MemberId));
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestForNullMember()
        {
            // Arrange
            var membersController = new MembersController(MemberRepo);

            // Act 
            var result = membersController.Post(null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnOKRequestForMemberUpdate()
        {
            var existingMember = GetMockedMember(1);

            // Arrange
            MemberRepo.Stub(repo => repo.UpdateMember(existingMember.MemberId, existingMember.Name))
                      .Return(true);

            var membersController = new MembersController(MemberRepo);

            // Act 
            var result = membersController.Put(existingMember.MemberId, new API.Member { MemberId = existingMember.MemberId, Name = existingMember.Name }) as OkResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestErrorMessageResultForDifferingMemberId()
        {
            // Arrange
            var membersController = new MembersController(MemberRepo);

            // Act
            var result = membersController.Put(0, new API.Member { MemberId = 1, Name = "Member Name" }) as BadRequestErrorMessageResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnBadRequestForNullMember()
        {
            // Arrange
            var membersController = new MembersController(MemberRepo);

            // Act 
            var result = membersController.Put(0, null) as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        [TestMethod]
        public void Put_ShouldReturnNotFoundForInvalidMemberId()
        {
            var existingMember = GetMockedMember(1);

            // Arrange
            MemberRepo.Stub(repo => repo.UpdateMember(existingMember.MemberId, existingMember.Name))
                      .Return(false);

            var membersController = new MembersController(MemberRepo);

            // Act 
            var result = membersController.Put(existingMember.MemberId, new API.Member { MemberId = existingMember.MemberId, Name = existingMember.Name }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result, "Result was not of the correct type.");
        }

        #region Private Methods

        private List<DAL.Member> GetMockedMemberList()
        {
            var members = new List<DAL.Member>();

            members.Add(GetMockedMember(3));
            members.Add(GetMockedMember(2));
            members.Add(GetMockedMember(1));

            return members;
        }

        private DAL.Member GetMockedMember(int id)
        {
            return new DAL.Member
                       {
                           MemberId = id,
                           Name = string.Format("Member {0}", id)
                       };
        }

        private static void SetupControllerForTests(ApiController membersController)
        {
            membersController.Request = new HttpRequestMessage();
            membersController.Request.SetConfiguration(new HttpConfiguration());
            membersController.Request.RequestUri = new Uri(string.Format("{0}{1}", ConfigurationManager.AppSettings["BaseApiUri"], Resources.Members));
        }

        #endregion
    }
}
