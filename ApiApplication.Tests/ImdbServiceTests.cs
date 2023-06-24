using ApiApplication.Services;
using Moq;

namespace ApiApplication.Tests
{
    [TestClass]
    public class ImdbServiceTests
    {
        [TestMethod]
        public void ShouldFind()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            var sut = new ImdbService("k_5v2j0109", httpClientFactory);

            var movie = sut.Find("tt0411008", out var description);

            Assert.IsNotNull(movie);
            Assert.IsTrue(string.IsNullOrEmpty(description));
            Assert.AreEqual("tt0411008", movie.ImdbId);            
            Assert.AreEqual("Lost", movie.Title);
            Assert.AreEqual("Jorge Garcia, Josh Holloway, Yunjin Kim", movie.Stars);
            Assert.AreEqual(new DateTime(2004, 9, 22), movie.ReleaseDate);
        }

        [TestMethod]
        public void ShouldReturnNullOnFindWhenMovieDoesNotExist()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            var sut = new ImdbService("k_5v2j0109", httpClientFactory);

            var movie = sut.Find("doesnotexist", out var description);

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
        public void ShouldThrowArgumentExceptionOnFindWhenImdbIdIsNull()
        {
            var httpClientFactory = Mock.Of<IHttpClientFactory>(o =>
                o.CreateClient(It.IsAny<string>()) == new HttpClient());

            var sut = new ImdbService("k_5v2j0109", httpClientFactory);

            sut.Find(null, out var _);
        }
    }
}
