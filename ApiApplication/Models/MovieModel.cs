using System;

namespace ApiApplication.Models
{
    public class MovieModel
    {
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Starts { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
