namespace DuploCloudApi.Models
{
    public class Forecast
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }

        public required string Latitude { get; set; }

        public required string Longitude { get; set; }

        public string? ForecastJsonData { get; set; }   

    }
}
