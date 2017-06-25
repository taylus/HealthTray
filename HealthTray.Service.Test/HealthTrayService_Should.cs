using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthTray.Service.Test
{
    [TestClass]
    public class HealthTrayService_Should
    {
        [TestMethod]
        public async Task Return_Checks()
        {
            //arrange - set up a successful response w/ check data in the expected JSON format
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"checks\": [ { \"last_ping\": \"2017-01-04T13:24:39.903464+00:00\", \"ping_url\": \"https://hchk.io/662ebe36-ecab-48db-afe3-e20029cb71e6\", \"next_ping\": \"2017-01-04T14:24:39.903464+00:00\", \"grace\": 900, \"name\": \"Api test 1\", \"n_pings\": 1, \"tags\": \"foo\", \"pause_url\": \"https://healthchecks.io/api/v1/checks/662ebe36-ecab-48db-afe3-e20029cb71e6/pause\", \"timeout\": 3600, \"status\": \"up\", \"update_url\": \"https://healthchecks.io/api/v1/checks/662ebe36-ecab-48db-afe3-e20029cb71e6\" }, { \"last_ping\": null, \"ping_url\": \"https://hchk.io/9d17c61f-5c4f-4cab-b517-11e6b2679ced\", \"next_ping\": null, \"grace\": 3600, \"name\": \"Api test 2\", \"n_pings\": 0, \"tags\": \"bar baz\", \"pause_url\": \"https://healthchecks.io/api/v1/checks/9d17c61f-5c4f-4cab-b517-11e6b2679ced/pause\", \"tz\": \"UTC\", \"schedule\": \"0/10 * * * *\", \"status\": \"new\", \"update_url\": \"https://healthchecks.io/api/v1/checks/9d17c61f-5c4f-4cab-b517-11e6b2679ced\" } ] }")
            });

            //act
            var service = new HealthTrayService(new HttpClient(stubHandler));
            var checks = await service.GetChecks();

            //assert
            Assert.AreEqual(2, checks.Count);
        }

        [TestMethod, ExpectedException(typeof(HttpRequestException))]
        public async Task Throw_Exception_On_Unsuccessful_Response()
        {
            //arrange - set up a successful response w/ check data in the expected JSON format
            var stubHandler = new StubHttpClientHandler();
            stubHandler.EnqueueResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            //act
            var service = new HealthTrayService(new HttpClient(stubHandler));
            await service.GetChecks();
        }
    }
}
