namespace RoyalHotel_RapidApi.Models
{
    public class HotelDetailViewModel
    {
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public double ReviewScore { get; set; }
        public string ReviewScoreWord { get; set; }
        public int ReviewCount { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Price { get; set; }
        public string CurrencyCode { get; set; }
        public List<string> PhotoUrls { get; set; } = new List<string>();
        public List<string> Facilities { get; set; } = new List<string>();
    }
}
