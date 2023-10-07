using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Newtonsoft.Json.Linq;

namespace IdentityServer.UnitTest.Quickstart.Account
{
    public class AccountControllerUnitTest : IntegrationTestBase
    {
        private readonly Mock<TestUserStore> _mockUsers;
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IAuthenticationSchemeProvider> _mockSchemeProvider;
        private readonly Mock<IEventService> _mockEvents;
        private readonly AccountController _accountController;
        TestServer _server;

        public AccountControllerUnitTest()
        {
            _mockUsers = new Mock<TestUserStore>(lsTestUser());
            _mockInteraction = new Mock<IIdentityServerInteractionService>();
            _mockClientStore = new Mock<IClientStore>();
            _mockSchemeProvider = new Mock<IAuthenticationSchemeProvider>();
            _mockEvents = new Mock<IEventService>();
            _accountController = new AccountController(_mockInteraction.Object, _mockClientStore.Object,
                _mockSchemeProvider.Object, _mockEvents.Object, _mockUsers.Object);
        }

        [Fact]
        public async Task GetLoginSuccess()
        {

            var user = new TestUser() { Username = "JohnDoe", SubjectId = "1" };
            AuthorizationRequest authorization = new AuthorizationRequest();
            authorization.Client = new Client();
            authorization.IdP = "test";
            authorization.LoginHint = user.Username;

            var scheme = new AuthenticationScheme(
                "Test",
                "Test",
                typeof(MockAuthenticationHandler)
            );

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.SubjectId),
                new Claim("name", user.Username),
            };
            var identity = new ClaimsIdentity(claims, "JohnDoe");
            var claimsPrincipal = new ClaimsPrincipal(identity);
;
            _mockInteraction.Setup(x => x.GetAuthorizationContextAsync(It.IsAny<string>())).Returns(Task.FromResult(authorization));
            _mockSchemeProvider.Setup(x => x.GetSchemeAsync(It.IsAny<string>())).ReturnsAsync(scheme);
            
            var result = await _accountController.Login("/Account");
            result.Should().NotBeNull();
            Assert.False(result.Equals(authorization));

        }

        private List<TestUser> lsTestUser()
        {
            return new List<TestUser>()
            {
                new TestUser()
                {
                    Username = "JohnDoe",
                    SubjectId = "1"
                },
                new TestUser()
                {
                    Username = "test2",
                    SubjectId = "2"
                }
            };
        }
    }
}
