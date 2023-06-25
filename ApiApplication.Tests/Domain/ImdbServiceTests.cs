using Moq;

namespace ApiApplication.Domain
{
    [TestClass]
    [TestCategory("Integration")]
    public class ImdbServiceTests
    {
        [TestMethod]
        public async Task ShouldFind()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            var sut = new ImdbService("k_5v2j0109", httpClientFactory);

            var (movie, description) = await sut.FindAsync("tt0411008");

            Assert.IsNotNull(movie);
            Assert.IsTrue(string.IsNullOrEmpty(description));
            Assert.AreEqual("tt0411008", movie.ImdbId);            
            Assert.AreEqual("Lost", movie.Title);
            Assert.AreEqual("Jorge Garcia, Josh Holloway, Yunjin Kim", movie.Stars);
            Assert.AreEqual(new DateTime(2004, 9, 22), movie.ReleaseDate);
        }

        [TestMethod]
        public async Task ShouldReturnNullOnFindWhenMovieDoesNotExist()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            var sut = new ImdbService("k_5v2j0109", httpClientFactory);

            var (movie, description) = await sut.FindAsync("doesnotexist");

            Assert.IsNull(movie);
            Assert.IsFalse(string.IsNullOrEmpty(description));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionOnConstructWhenApiKeyIsNull()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            new ImdbService(null, httpClientFactory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionOnConstructWhenFactoryIsNull()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            new ImdbService("somekey", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ShouldThrowArgumentExceptionOnFindWhenImdbIdIsNull()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            var sut = new ImdbService("k_5v2j0109", httpClientFactory);

            await sut.FindAsync(null);
        }
    }
}
