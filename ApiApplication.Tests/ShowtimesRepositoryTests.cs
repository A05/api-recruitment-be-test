using ApiApplication.Database;
using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics;

namespace ApiApplication.Tests
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
        public void ShouldAdd()
        {
            Debug.Assert(_context != null);

            var toAdd = CreateShowtimeEntity();

            var sut = new ShowtimesRepository(_context);
            var added = sut.Add(toAdd);

            Assert.AreNotSame(toAdd, added);
            Assert.AreNotSame(toAdd.Movie, added.Movie);

            AssertAreEqual(toAdd, added, withId: false);

            var fromDb = _context.Showtimes.Single(i => i.Id == added.Id);

            AssertAreEqual(added, fromDb, withId: true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionOnAddWhenShowtimeIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            sut.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionOnAddWhenMovieIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            sut.Add(new ShowtimeEntity { Movie = null });
        }

        [TestMethod]
        public void ShouldDelete()
        {
            Debug.Assert(_context != null);

            var obj1 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj1);
            var obj2 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj2);
            _context.SaveChanges();

            var sut = new ShowtimesRepository(_context);
            var deleted = sut.Delete(obj1.Id);

            Assert.AreEqual(obj1.Id, deleted.Id);
            Assert.AreEqual(1, _context.Showtimes.Count());

            Assert.AreEqual(obj1.Movie.Id, deleted.Movie.Id);
            Assert.AreEqual(1, _context.Movies.Count());

            Assert.AreEqual(obj2.Id, _context.Showtimes.First().Id);
            Assert.AreEqual(obj2.Movie.Id, _context.Movies.First().Id);
        }

        [TestMethod]
        public void ShouldGetByMovie()
        {
            Debug.Assert(_context != null);

            var obj1 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj1);
            var obj2 = CreateShowtimeEntity();
            _context.Showtimes.Add(obj2);
            _context.SaveChanges();

            var sut = new ShowtimesRepository(_context);
            var found = sut.GetByMovie(movie => movie.Title == obj1.Movie.Title);
            
            Assert.IsNotNull(found?.Movie);
            Assert.AreEqual(obj1.Id, found.Id);
            Assert.AreEqual(obj1.Movie.Title, found.Movie.Title);

            found = sut.GetByMovie(movie => movie.Title.Contains((_next - 1).ToString()));

            Assert.IsNotNull(found?.Movie);
            Assert.AreEqual(obj2.Id, found.Id);
            Assert.AreEqual(obj2.Movie.Title, found.Movie.Title);

            found = sut.GetByMovie(movie => movie.Title.Contains(_next.ToString()));

            Assert.IsNull(found);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionOnGetByMovieWhenFilterIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            sut.GetByMovie(null);
        }

        [TestMethod]
        public void ShouldGetCollection()
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
            var founds = sut.GetCollection(i => i.StartDate <= now && now <= i.EndDate);

            Assert.AreEqual(3, founds.Count());

            founds = sut.GetCollection(i => i.StartDate <= now.AddDays(-3) && now.AddDays(-3) <= i.EndDate);

            Assert.AreEqual(1, founds.Count());
            Assert.AreEqual(obj1.Id, founds.First().Id);

            founds = sut.GetCollection(i => i.StartDate <= now.AddDays(3) && now.AddDays(3) <= i.EndDate);

            Assert.AreEqual(1, founds.Count());
            Assert.AreEqual(obj2.Id, founds.First().Id);

            founds = sut.GetCollection(null);

            Assert.AreEqual(3, founds.Count());

            founds = sut.GetCollection();

            Assert.AreEqual(3, founds.Count());
        }

        [TestMethod]
        public void ShouldUpdate()
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
            var updated = sut.Update(obj2);

            Assert.IsNotNull(updated);
            Assert.AreNotSame(updated, obj2);

            AssertAreEqual(obj2, updated, withId: false);
            Assert.AreEqual(obj2.Id, updated.Id);
            Assert.AreEqual(obj1.Movie.Id, updated.Movie.Id);
            Assert.AreEqual(obj1.Movie.ShowtimeId, updated.Movie.ShowtimeId);
        }

        [TestMethod]
        public void ShouldUpdateWhenMovieIsNull()
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
            var updated = sut.Update(obj2);

            Assert.IsNotNull(updated);
            Assert.AreNotSame(updated, obj2);

            AssertAreEqual(obj2, updated, withId: true, withMovie: false);
            AssertAreEqual(obj1.Movie, updated.Movie, withId: true);
            Assert.AreEqual(obj1.Id, updated.Movie.ShowtimeId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionOnUpdateWhenEntityDoesNotExist()
        {
            var sut = new ShowtimesRepository(_context);
            sut.Update(new ShowtimeEntity { Id = 2548 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentNullExceptionOnUpdateWhenEntityIsNull()
        {
            var sut = new ShowtimesRepository(_context);
            sut.Update(null);
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