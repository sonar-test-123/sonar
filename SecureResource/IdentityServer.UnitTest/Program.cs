using IdentityServer.UnitTest.Quickstart.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();