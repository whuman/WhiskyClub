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
    public class WhiskiesAPITests
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
        public void CRUD_Whiskies()
        {
            var whisky = PostWhisky();
            GetWhiskies();
            GetWhisky(whisky);
            Put(whisky);
            Delete(whisky);
        }

        [TestCleanup]
        public void CleanUp()
        {
            Client.Dispose();
        }

        private Whisky PostWhisky()
        {
            // Arrange
            var postWhisky = new Whisky()
            {
                Name = string.Format("Test Whisky POST {0:yyyyMMddHHmmss}", DateTime.Now),
                Brand = "Test Brand",
                Age = 1,
                Country = "Test Country",
                Region = "Test Region", 
                Description = "Test Description"
            };

            // Act 
            var response = Client.PostAsJsonAsync(Resources.Whiskies, postWhisky).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "POST whisky not ok.");

            return response.Content.ReadAsAsync<Whisky>().Result;
        }

        private void GetWhiskies()
        {
            // Act 
            var list = Client.GetAsync(Resources.Whiskies)
                             .Result.Content.ReadAsAsync<IEnumerable<Whisky>>()
                             .Result;

            // Assert
            Assert.IsTrue(list.Any(), "GET ALL whiskies not OK.");
        }

        private Whisky GetWhisky(Whisky whisky)
        {
            // Act
            var response = Client.GetAsync(GetWhiskyRequestUri(whisky)).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Get whisky by ID not OK.");

            return response.Content.ReadAsAsync<Whisky>().Result;
        }

        private void Put(Whisky whisky)
        {
            // Arrange
            var putWhisky = new Whisky()
            {
                WhiskyId = whisky.WhiskyId,
                Name = string.Format("Test Whisky PUT {0:yyyyMMddHHmmss}", DateTime.Now),
                Brand = "Test Brand",
                Age = 1,
                Country = "Test Country",
                Region = "Test Region",
                Description = "Test Description"
            };

            // Act 
            var response = Client.PutAsJsonAsync(GetWhiskyRequestUri(whisky), putWhisky).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "PUT whisky not ok.");
            Assert.AreEqual(putWhisky.Name, GetWhisky(whisky).Name, "PUT whisky not updated.");
        }

        private void Delete(Whisky whisky)
        {
            // Act
            var response = Client.DeleteAsync(GetWhiskyRequestUri(whisky)).Result;
            var deletedStatus = Client.GetAsync(GetWhiskyRequestUri(whisky)).Result.StatusCode;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "DELETE Competition not OK.");
            Assert.AreEqual(HttpStatusCode.NotFound, deletedStatus, "DELETE Competition not deleted.");
        }

        private static string GetWhiskyRequestUri(Whisky whisky)
        {
            return string.Format("{0}/{1}", Resources.Whiskies, whisky.WhiskyId);
        }
    }
}
