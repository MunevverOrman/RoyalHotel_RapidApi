using Newtonsoft.Json;

namespace RoyalHotel_RapidApi.Models
{
    public class CryptoViewModel
    {
        public CryptoCoin? bitcoin { get; set; }
        public CryptoCoin? ethereum { get; set; }
        public CryptoCoin? binancecoin { get; set; }
    }

    public class CryptoCoin
    {
        public double usd { get; set; }
    }
}