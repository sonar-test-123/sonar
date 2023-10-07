using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace IdentityServer.UnitTest.Quickstart.Account
{
    public class AccountControllerUnitTest
    {
        private readonly Mock<TestUserStore> _mockUsers;
        private readonly Mock<IIdentityServerInteractionService> _mockInteraction;
        private readonly Mock<IClientStore> _mockClientStore;
        private readonly Mock<IAuthenticationSchemeProvider> _mockSchemeProvider;
        private readonly Mock<IEventService> _mockEvents;
        private readonly AccountController _accountController;

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
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.SubjectId),
                new Claim("name", user.Username),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockPrincipal = new Mock<IPrincipal>();
            mockPrincipal.Setup(x => x.Identity).Returns(identity);
            mockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);

            //_mockInteraction.Setup(x => x.GetAuthorizationContextAsync(It.IsAny<string>())).Returns()

            _accountController.Login("test");


        }

        private List<TestUser> lsTestUser()
        {
            return new List<TestUser>()
            {
                new TestUser()
                {
                    Username = "test1",
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
