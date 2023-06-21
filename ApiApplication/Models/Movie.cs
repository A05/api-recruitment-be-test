using System;

namespace ApiApplication.Models
{
    public class Movie
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
