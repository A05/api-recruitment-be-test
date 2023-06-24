using ApiApplication.Database.Entities;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json.Nodes;

namespace ApiApplication.Services
{
    public class ImdbService : IImdbService
    {
        private readonly string _apiKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImdbService(string apiKey, IHttpClientFactory httpClientFactory)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("IMDB API key must be specified.");

            _apiKey = apiKey;

            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public MovieEntity Find(string imdbId, out string description)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
                throw new ArgumentException("IMDB ID must be specified.");

            var httpClient = _httpClientFactory.CreateClient();
            
            httpClient.BaseAddress = new Uri($"https://imdb-api.com/en/API/Title/{_apiKey}/");

            var jsonString = httpClient.GetStringAsync(imdbId).Result;

            var movieNode = JsonNode.Parse(jsonString);

            MovieEntity entity;

            if (IsThereAnyError(movieNode, out var errorMessage))
            {
                description = $"Failed to find the movie with IMDB ID {imdbId}. {errorMessage}.";
                entity = null;            
            }
            else
            {
                description = string.Empty;
                entity = new MovieEntity
                {
                    ImdbId = imdbId,
                    ReleaseDate = GetReleaseDate(movieNode),
                    Stars = GetStars(movieNode),
                    Title = GetTitle(movieNode)
                };
            }

            return entity;
        }

        private bool IsThereAnyError(JsonNode movieNode, out string errorMessage)
        {
            const string ERROR_MESSAGE = "errorMessage";

            errorMessage = (string)movieNode[ERROR_MESSAGE];

            var isThereAnyError = !string.IsNullOrEmpty(errorMessage);

            if (!errorMessage.EndsWith('.'))
                errorMessage += ".";

            return isThereAnyError;
        }

        private DateTime GetReleaseDate(JsonNode movieNode)
        {
            const string RELEASE_DATE = "releaseDate";

            var dateAsString = (string)movieNode[RELEASE_DATE];
            var date = DateTime.ParseExact(dateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            return date;
        }

        private string GetStars(JsonNode movieNode)
        {
            const string STARS = "stars";

            var stars = (string)movieNode[STARS];

            return stars;
        }

        private string GetTitle(JsonNode movieNode)
        {
            const string TITLE = "title";

            var title = (string)movieNode[TITLE];

            return title;
        }
    }
}
