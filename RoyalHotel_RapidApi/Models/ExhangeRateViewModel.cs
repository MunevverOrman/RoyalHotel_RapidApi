namespace RoyalHotel_RapidApi.Models
{
    // Ana kapsayıcı sınıf
    public class ExhangeRateViewModel
    {
        public ConvertResult convert_result { get; set; }
    }

    // Sadece kur oranını tutan sınıf
    public class ConvertResult
    {
        public float rate { get; set; }
    }
}