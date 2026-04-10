namespace RoyalHotel_RapidApi.Models
{
    public class HotelListViewModel
    {
        public string City { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public int AdultCount { get; set; }
        public List<HotelSearchResult> Hotels { get; set; } = new List<HotelSearchResult>();
    }
}

