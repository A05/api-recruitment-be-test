using System;

namespace ApiApplication.Models
{
    public class ShowtimeModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Schedule { get; set; }
        public MovieModel Movie { get; set; }
        public int AuditoriumId { get; set; }
    }
}
