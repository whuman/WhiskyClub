using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhiskyClub.WebAPI.Models;

namespace WhiskyClub.WebAPI.Tests.IntegrationTests
{
    [TestClass]
    public class MembersAPITests
    {
        private HttpClient _client;

        public HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient()
                                  {
                                      BaseAddress = new Uri(ConfigurationManager.AppSettings["BaseApiUri"]),
                                  };
                }

                return _client;
            }
        }

        [TestMethod]
        public void CRUD_Members()
        {
            // These tests update the database and should be run manually
            // Please ensure that your datastore is cleaned up afterwards
            Assert.Inconclusive("Test must be run manually.");

            ////var member = PostMember();
            ////GetMembers();
            ////GetMember(member);
            ////Put(member);
            ////Delete(member);
        }

        [TestCleanup]
        public void CleanUp()
        {
            Client.Dispose();
        }

        private Member PostMember()
        {
            // Arrange
            var postMember = new Member()
                             {
                                 Name = string.Format("Test Member POST {0:yyyyMMddHHmmss}", DateTime.Now)
                             };

            // Act 
            var response = Client.PostAsJsonAsync(Resources.Members, postMember).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "POST member not ok.");

            return response.Content.ReadAsAsync<Member>().Result;
        }

        private void GetMembers()
        {
            // Act 
            var list = Client.GetAsync(Resources.Members)
                             .Result.Content.ReadAsAsync<IEnumerable<Member>>()
                             .Result;

            // Assert
            Assert.IsTrue(list.Any(), "GET ALL members not OK.");
        }

        private Member GetMember(Member member)
        {
            // Act
            var response = Client.GetAsync(GetMemberRequestUri(member)).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Get member by ID not OK.");

            return response.Content.ReadAsAsync<Member>().Result;
        }

        private void Put(Member member)
        {
            // Arrange
            var putMember = new Member()
                                {
                                    MemberId = member.MemberId,
                                    Name = string.Format("Test Member PUT {0:yyyyMMddHHmmss}", DateTime.Now)
                                };

            // Act 
            var response = Client.PutAsJsonAsync(GetMemberRequestUri(member), putMember).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "PUT member not ok.");
            Assert.AreEqual(putMember.Name, GetMember(member).Name, "PUT member not updated.");
        }

        private void Delete(Member member)
        {
            // Act
            var response = Client.DeleteAsync(GetMemberRequestUri(member)).Result;
            var deletedStatus = Client.GetAsync(GetMemberRequestUri(member)).Result.StatusCode;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "DELETE Competition not OK.");
            Assert.AreEqual(HttpStatusCode.NotFound, deletedStatus, "DELETE Competition not deleted.");
        }

        private static string GetMemberRequestUri(Member member)
        {
            return string.Format("{0}/{1}", Resources.Members, member.MemberId);
        }
    }
}