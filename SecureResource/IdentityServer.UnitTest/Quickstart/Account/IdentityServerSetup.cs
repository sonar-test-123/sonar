using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;

namespace IdentityServer.UnitTest.Quickstart.Account
{
    public class IdentityServerSetup
    {
        private readonly string _apiName;
        private readonly string _apiSecret;
        private TestServer _identityServer;
        private const string TokenEndpoint = "http://localhost/connect/token";
        private HttpMessageHandler _handler;
        private IEnumerable<Client> _clients;
        private IEnumerable<TestUser> _users;
        private readonly IEnumerable<ApiResource> _apiResources;
        private const string ApiName = "api";
        private const string ApiSecret = "secret";
        private bool _loggingEnabled;

        public IdentityServerSetup(string apiName = null, string apiSecret = null)
        {
            _apiName = apiName ?? ApiName;
            _apiSecret = apiSecret ?? ApiSecret;
            _apiResources = InitApiResources(_apiName, _apiSecret);
            _clients = GetClients();
            _users = GetUsers();
        }

        public IdentityServerSetup AddLogging()
        {
            _loggingEnabled = true;
            return this;
        }

        public IdentityServerSetup SetClients(IEnumerable<Client> clients)
        {
            _clients = clients;
            return this;
        }

        public IdentityServerSetup SetUsers(IEnumerable<TestUser> users)
        {
            _users = users;
            return this;
        }

        public IdentityServerSetup SetScopeClaims(IList<string> userClaims)
        {
            var apiResource = _apiResources.First();
            apiResource.UserClaims = userClaims;
            return this;
        }

        public IdentityServerSetup Initialize()
        {
            InitializeIdentityServer();
            return this;
        }

        public IdentityServerAuthenticationOptions GetIdentityServerAuthenticationOptions(string authenticationScheme = "API")
        {
            var options = new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost",
                RequireHttpsMetadata = false,
                JwtBackChannelHandler = _identityServer.CreateHandler(),
                //IntrospectionBackChannelHandler = _identityServer.CreateHandler(),
                //IntrospectionDiscoveryHandler = _identityServer.CreateHandler(),
                //AutomaticAuthenticate = true,
                //AuthenticationScheme = authenticationScheme,
                ApiName = _apiName,
                ApiSecret = _apiSecret
            };
            return options;
        }

        public async Task<string> GetAccessTokenForUser(string userName, string password, string clientId = "client", string clientSecret = "secret")
        {
            var httpMessageInvoker = new HttpMessageInvoker(_handler);
            var tokenClientOption = new TokenClientOptions();
            tokenClientOption.ClientId = clientId;
            tokenClientOption.ClientSecret = clientSecret;
            tokenClientOption.Address = TokenEndpoint;
            var client = new TokenClient(httpMessageInvoker, tokenClientOption);

            var response = await client.RequestPasswordTokenAsync(userName, password, _apiName);
            return response.AccessToken;
        }

        public async Task SetAccessToken(HttpClient client, string username, string password, string clientId = "client", string clientSecret = "secret")
        {
            var token = await GetAccessTokenForUser(username, password, clientId, clientSecret);
            client.SetBearerToken(token);
        }

        private void InitializeIdentityServer()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(ConfigureIdentityServerServices)
                .Configure(ConfigureIdentityServerApp)
                .ConfigureLogging(ConfigureIdentityServerLogging);
            _identityServer = new TestServer(builder);
            _handler = _identityServer.CreateHandler();
        }

        private void ConfigureIdentityServerLogging(ILoggingBuilder obj)
        {
            if (!_loggingEnabled)
            {
                return;
            }
            obj.AddConsole();
            obj.AddDebug();
        }

        private void ConfigureIdentityServerApp(IApplicationBuilder app)
        {
            app.UseIdentityServer();
            //app.UseMvc();
        }

        private void ConfigureIdentityServerServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                //.AddTemporarySigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(_apiResources)
                .AddInMemoryClients(_clients)
                .AddTestUsers(_users.ToList());
            services.AddMvc();
        }

        private IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        private List<ApiResource> InitApiResources(string apiName, string apiSecret)
        {
            return new List<ApiResource>
            {
                new ApiResource(apiName, "My API") {ApiSecrets = {new Secret(apiSecret.Sha256())}}
            };
        }


        // Default client
        private IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        _apiName
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        // Default user
        private static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "user",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "User"),
                        new Claim("website", "https://user.com")
                    }
                }
            };
        }
    }
}
