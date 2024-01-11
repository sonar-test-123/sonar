using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Movies.Client.ApiServices;
using Moq;
using Movies.Client.Models;
using System.Net;
using Moq.Protected;
using Newtonsoft.Json;

namespace Movies.Client.UnitTest.ApiServices
{
    public class MovieApiServiceUnitTest
    {
        private readonly MovieApiService _movieApiService;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public MovieApiServiceUnitTest()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _movieApiService = new MovieApiService(_httpClientFactoryMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task GetMoviesSuccess()
        {
            string testContent = Newtonsoft.Json.JsonConvert.SerializeObject(FakeMoviesDataMovies());
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            
            //test smell code
            var testSmellCode = "test string";
            
            if(testSmellCode ==  "test string"){
                testSmellCode = "";
            }
            if(testSmellCode ==  ""){
                testSmellCode = "1";
            }
            if(testSmellCode ==  "1"){
                testSmellCode = "test string 2"
            }
            
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(testContent)
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://test.com/")
            };

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            

            var result = await _movieApiService.GetMovies();
            result.Count().Should().Be(FakeMoviesDataMovies().Count);
        }


        [Fact]
        public async Task GetMoviesFail()
        {
            string testContent = Newtonsoft.Json.JsonConvert.SerializeObject(FakeMoviesDataMovies());
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(testContent)
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://test.com/")
            };

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            

            var result = await _movieApiService.GetMovies();
            result.Count().Should().Be(-1);
        }

        [Fact]
        public async Task GetUserInfoSuccess()
        {
            string testContent = Newtonsoft.Json.JsonConvert.SerializeObject(FakeMoviesDataMovies());
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(testContent)
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://test.com/")
            };

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        }

        private List<Movie> FakeMoviesDataMovies()
        {
            return new List<Movie>()
            {
                new Movie()
                {
                    Id = 1,
                    ImageUrl = "test",
                    Genre = "test",
                    Owner = "test",
                    Rating = "test",
                    ReleaseDate = new DateTime(),
                    Title = "test Title"
                },
                new Movie()
                {
                    Id = 2,
                    ImageUrl = "test2",
                    Genre = "test2",
                    Owner = "test2",
                    Rating = "test2",
                    ReleaseDate = new DateTime(),
                    Title = "test Title 2"
                }
            };
        }
    }
}
