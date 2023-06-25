using ApiApplication.Database.Entities;
using ApiApplication.Models;
using AutoMapper;

namespace ApiApplication
{
    [TestClass]
    public class AutoMapperProfileTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static IMapper _mapper;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [ClassInitialize]
        public static void ConfigurateAutoMapper(TestContext _)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperProfile>();
            });

            _mapper = new Mapper(config);
        }

        [TestMethod]
        public void ShouldMapShowtimeEntity()
        {
            var entity = CreateShowtimeEntity();
            var model = _mapper.Map<ShowtimeModel>(entity);

            AssertAreEqual(entity, model);
        }

        [TestMethod]
        public void ShouldMapShowtimeModel()
        {
            var model = CreateShowtimeModel();
            var entity = _mapper.Map<ShowtimeEntity>(model);

            AssertAreEqual(entity, model);
        }

        private ShowtimeEntity CreateShowtimeEntity()
        {
            return new ShowtimeEntity
            {
                Id = 567,
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(2),
                Schedule = new[] { "a", "b", "c" },
                AuditoriumId = 1,
                Movie = new MovieEntity
                {
                    ImdbId = "234",
                    ReleaseDate = DateTime.Now.AddMonths(-1),
                    Stars = "****",
                    Title = "Some title"
                }
            };
        }

        private ShowtimeModel CreateShowtimeModel()
        {
            return new ShowtimeModel
            {
                Id = 567,
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(2),
                Schedule = "a,b,c",
                AuditoriumId = 1,
                Movie = new MovieModel
                {
                    ImdbId = "234",
                    ReleaseDate = DateTime.Now.AddMonths(-1),
                    Starts = "****",
                    Title = "Some title"
                }
            };
        }

        private void AssertAreEqual(ShowtimeEntity entity, ShowtimeModel model)
        {
            if (entity == null && model == null)
                return;

            if (entity == null || model == null)
                Assert.Fail();

            Assert.AreEqual(entity.Id, model.Id);
            Assert.AreEqual(entity.StartDate, model.StartDate);
            Assert.AreEqual(entity.EndDate, model.EndDate);
            Assert.AreEqual(string.Join(",", entity.Schedule), model.Schedule);
            Assert.AreEqual(entity.AuditoriumId, model.AuditoriumId);

            AssertAreEqual(entity.Movie, model.Movie);
        }

        private void AssertAreEqual(MovieEntity entity, MovieModel model)
        {
            if (entity == null && model == null)
                return;

            if (entity == null || model == null)
                Assert.Fail();

            Assert.AreEqual(entity.ImdbId, model.ImdbId);
            Assert.AreEqual(entity.ReleaseDate, model.ReleaseDate);
            Assert.AreEqual(entity.Stars, model.Starts);
            Assert.AreEqual(entity.Title, model.Title);
        }
    }
}
