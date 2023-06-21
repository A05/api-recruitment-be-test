namespace ApiApplication.Database.Entities
{
    public static class EntityExtensions
    {
        public static ShowtimeEntity Clone(this ShowtimeEntity obj)
        {
            if (obj == null)
                return null;

            return new ShowtimeEntity
            {
                Id = obj.Id,
                StartDate = obj.StartDate,
                EndDate = obj.EndDate,
                Schedule = obj.Schedule,
                AuditoriumId = obj.AuditoriumId,
                Movie = Clone(obj.Movie)
            };
        }

        public static MovieEntity Clone(this MovieEntity obj)
        {
            if (obj == null)
                return null;

            return new MovieEntity
            {
                Id = obj.Id,
                ImdbId = obj.ImdbId,
                ReleaseDate = obj.ReleaseDate,
                Stars = obj.Stars,
                Title = obj.Title,
                ShowtimeId = obj.ShowtimeId
            };
        }
    }
}
