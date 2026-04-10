namespace RoyalHotel_RapidApi.Models
{
    public class WeatherViewModel
    {
        public List<Weather>? weather { get; set; }
        public Main? main { get; set; }
        public string? name { get; set; } 
    }

    public class Weather
    {
        public string? main { get; set; } 
        public string? description { get; set; } 
        public string? icon { get; set; } 
    }

    public class Main
    {
        public double temprature { get; set; } 
        public int humidity { get; set; }
    }
}