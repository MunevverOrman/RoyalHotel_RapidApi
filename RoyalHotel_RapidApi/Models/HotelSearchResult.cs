namespace RoyalHotel_RapidApi.Models
{
    public class HotelSearchResult
    {
        public int hotel_id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string country_trans { get; set; }
        public double review_score { get; set; }
        public string review_score_word { get; set; }
        public int review_nr { get; set; }
        public string main_photo_url { get; set; }
        public double min_total_price { get; set; }
        public string currency_code { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
