using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics;

namespace ApiApplication.Database
{
    [TestClass]
    [TestCategory("Integration")]
    public class ShowtimesRepositoryTests
    {
        private int _next;
        private CinemaDbContext? _context;

        [TestInitialize]
        public void CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CinemaDbContext>();

            optionsBuilder
                .UseInMemoryDatabase("CinemaDb")
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var options = optionsBuilder.Options;

            _context = new CinemaDbContext(options);

            _context.Database.EnsureDeleted();
        }

        [TestCleanup]
        public void DisposeContext()
        {
            _context?.Dispose();
        }

        [TestMethod]
        public async Task ShouldAdd()
        {
            Debug.Assert(_context != null);

            var toAdd = CreateShowtimeEntity();

            var sut = new ShowtimesRepository(_context);
            var added = await sut.AddAsync(toAdd);

            Assert.AreNotSame(toAdd, added);
            Assert.AreNotSame(toAdd.Movie, added.Movie);

            AssertAreEqual(toAdd, added, withId: false);

            var fromDb = _context.Showtimes.Single(i => i.Id == added.Id);

            AssertAreEqual(added, fromDb, withId: true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ShouldThrowArgumentNullExceptionOnAddWhenShowtimeIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            await sut.AddAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ShouldThrowArgumentExceptionOnAddWhenMovieIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            await sut.AddAsync(new ShowtimeEntity { Movie = null });
        }

        [TestMethod]
        public async Task ShouldDelete()
        {
            Debug.Assert(_context != null);

            var obj1 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj1);
            var obj2 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj2);
            _context.SaveChanges();

            var sut = new ShowtimesRepository(_context);
            var deleted = await sut.DeleteAsync(obj1.Id);

            Assert.AreEqual(obj1.Id, deleted.Id);
            Assert.AreEqual(1, _context.Showtimes.Count());

            Assert.AreEqual(obj1.Movie.Id, deleted.Movie.Id);
            Assert.AreEqual(1, _context.Movies.Count());

            Assert.AreEqual(obj2.Id, _context.Showtimes.First().Id);
            Assert.AreEqual(obj2.Movie.Id, _context.Movies.First().Id);
        }

        [TestMethod]
        public async Task ShouldGetByMovie()
        {
            Debug.Assert(_context != null);

            var obj1 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj1);
            var obj2 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj2);
            _context.SaveChanges();

            var sut = new ShowtimesRepository(_context);
            var found = await sut.GetByMovieAsync(movie => movie.Title == obj1.Movie.Title);
            
            Assert.IsNotNull(found?.Movie);
            Assert.AreEqual(obj1.Id, found.Id);
            Assert.AreEqual(obj1.Movie.Title, found.Movie.Title);

            found = await sut.GetByMovieAsync(movie => movie.Title.Contains((_next - 1).ToString()));

            Assert.IsNotNull(found?.Movie);
            Assert.AreEqual(obj2.Id, found.Id);
            Assert.AreEqual(obj2.Movie.Title, found.Movie.Title);

            found = await sut.GetByMovieAsync(movie => movie.Title.Contains(_next.ToString()));

            Assert.IsNull(found);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ShouldThrowArgumentNullExceptionOnGetByMovieWhenFilterIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            await sut.GetByMovieAsync(null);
        }

        [TestMethod]
        public async Task ShouldGetCollection()
        {
            Debug.Assert(_context != null);

            var now = DateTime.Now;

            var obj1 = CreateShowtimeEntity();
            obj1.StartDate = now.AddDays(-4);
            _context.Showtimes.Add(obj1);
            var obj2 = CreateShowtimeEntity();
            obj2.EndDate = now.AddDays(4);
            _context.Showtimes.Add(obj2);
            var obj3 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj3);
            _context.SaveChanges();

            var sut = new ShowtimesRepository(_context);
            var founds = await sut.GetCollectionAsync(i => i.StartDate <= now && now <= i.EndDate);

            Assert.AreEqual(3, founds.Count());

            founds = await sut.GetCollectionAsync(i => i.StartDate <= now.AddDays(-3) && now.AddDays(-3) <= i.EndDate);

            Assert.AreEqual(1, founds.Count());
            Assert.AreEqual(obj1.Id, founds.First().Id);

            founds = await sut.GetCollectionAsync(i => i.StartDate <= now.AddDays(3) && now.AddDays(3) <= i.EndDate);

            Assert.AreEqual(1, founds.Count());
            Assert.AreEqual(obj2.Id, founds.First().Id);

            founds = await sut.GetCollectionAsync(null);

            Assert.AreEqual(3, founds.Count());

            founds = await sut.GetCollectionAsync();

            Assert.AreEqual(3, founds.Count());
        }

        [TestMethod]
        public async Task ShouldUpdate()
        {
            Debug.Assert(_context != null);

            var obj1 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj1);            
            _context.SaveChanges();

            var obj2 = new ShowtimeEntity
            {
                Id = obj1.Id,
                StartDate = DateTime.Now.AddDays(-6),
                EndDate = DateTime.Now.AddDays(8),
                Schedule = new[] { "g", "h", "j" },
                AuditoriumId = 3,
                Movie = new MovieEntity
                {
                    ImdbId = $"567_{_next}",
                    ReleaseDate = DateTime.Now.AddMonths(-5),
                    Stars = "****+++",
                    Title = $"Upated title {_next++}"
                }
            };

            var sut = new ShowtimesRepository(_context);
            var updated = await sut.UpdateAsync(obj2);

            Assert.IsNotNull(updated);
            Assert.AreNotSame(updated, obj2);

            AssertAreEqual(obj2, updated, withId: false);
            Assert.AreEqual(obj2.Id, updated.Id);
            Assert.AreEqual(obj1.Movie.Id, updated.Movie.Id);
            Assert.AreEqual(obj1.Movie.ShowtimeId, updated.Movie.ShowtimeId);
        }

        [TestMethod]
        public async Task ShouldUpdateWhenMovieIsNull()
        {
            Debug.Assert(_context != null);

            var obj1 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj1);
            _context.SaveChanges();

            var obj2 = new ShowtimeEntity
            {
                Id = obj1.Id,
                StartDate = DateTime.Now.AddDays(-6),
                EndDate = DateTime.Now.AddDays(8),
                Schedule = new[] { "g", "h", "j" },
                AuditoriumId = 3,
                Movie = null
            };

            var sut = new ShowtimesRepository(_context);
            var updated = await sut.UpdateAsync(obj2);

            Assert.IsNotNull(updated);
            Assert.AreNotSame(updated, obj2);

            AssertAreEqual(obj2, updated, withId: true, withMovie: false);
            AssertAreEqual(obj1.Movie, updated.Movie, withId: true);
            Assert.AreEqual(obj1.Id, updated.Movie.ShowtimeId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ShouldThrowArgumentExceptionOnUpdateWhenEntityDoesNotExist()
        {
            var sut = new ShowtimesRepository(_context);
            await sut.UpdateAsync(new ShowtimeEntity { Id = 2548 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ShouldThrowArgumentNullExceptionOnUpdateWhenEntityIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            await sut.UpdateAsync(null);
        }

        private ShowtimeEntity CreateShowtimeEntity()
        {
            return new ShowtimeEntity
            {
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(2),
                Schedule = new [] { "a", "b", "c" },
                AuditoriumId = 1,
                Movie = new MovieEntity
                {
                    ImdbId = $"234_{_next}",
                    ReleaseDate = DateTime.Now.AddMonths(-1),
                    Stars = "****",
                    Title = $"Some title {_next++}"
                }
            };
        }

        private static void AssertAreEqual(ShowtimeEntity exp, ShowtimeEntity act, bool withId, bool withMovie = true)
        {
            Assert.IsNotNull(exp);
            Assert.IsNotNull(act);

            Assert.IsTrue(act.Id != 0);
            if (withId)
                Assert.AreEqual(exp.Id, act.Id);

            Assert.AreEqual(exp.StartDate, act.StartDate);
            Assert.AreEqual(exp.EndDate, act.EndDate);
            Assert.AreEqual(exp.AuditoriumId, act.AuditoriumId);
            Assert.AreEqual(exp.Schedule.Count(), act.Schedule.Count());
            foreach (var s in exp.Schedule)
                Assert.IsTrue(act.Schedule.Contains(s));

            if (withMovie)
            {
                AssertAreEqual(exp.Movie, act.Movie, withId);
                Assert.AreEqual(act.Id, act.Movie.ShowtimeId);
            }
        }

        private static void AssertAreEqual(MovieEntity exp, MovieEntity act, bool withId)
        {
            Assert.IsNotNull(exp);
            Assert.IsNotNull(act);

            Assert.IsTrue(act.Id != 0);
            if (withId)
            {
                Assert.AreEqual(exp.Id, act.Id);
                Assert.AreEqual(exp.ShowtimeId, act.ShowtimeId);
            }
                        
            Assert.AreEqual(exp.ImdbId, act.ImdbId);
            Assert.AreEqual(exp.ReleaseDate, act.ReleaseDate);
            Assert.AreEqual(exp.Stars, act.Stars);
            Assert.AreEqual(exp.Title, act.Title);
        }
    }
}