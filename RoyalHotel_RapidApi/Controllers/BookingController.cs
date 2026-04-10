using Microsoft.AspNetCore.Mvc;
using RoyalHotel_RapidApi.Models;
using System.Text.Json;

namespace RoyalHotel_RapidApi.Controllers
{
    public class BookingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BookingController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

      
        [HttpPost]
        public async Task<IActionResult> HotelList(string city, string checkIn, string checkOut, int adultCount)
        {    
            if (checkIn != null && checkIn.Contains(" "))
                checkIn = checkIn.Split(' ')[0];
            if (checkOut != null && checkOut.Contains(" "))
                checkOut = checkOut.Split(' ')[0];

            // Tarihleri session'a kaydet
            HttpContext.Session.SetString("checkIn", checkIn ?? "");
            HttpContext.Session.SetString("checkOut", checkOut ?? "");

            var apiKey = _configuration["RapidApi:Key"];
            var apiHost = _configuration["RapidApi:Host"];
            var client = _httpClientFactory.CreateClient();

            // 1-Destination ID bul. 
            string destId = string.Empty;
            string searchType = "CITY";

            var destRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={city}"),
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", apiHost }
                }
            };

            using (var destResponse = await client.SendAsync(destRequest))
            {
                destResponse.EnsureSuccessStatusCode();
                var destBody = await destResponse.Content.ReadAsStringAsync();

                using var destJson = JsonDocument.Parse(destBody);
                var root = destJson.RootElement;

                // API'dan gelen ilk sonucun dest_id'sini alıyoruz
                if (root.TryGetProperty("data", out var dataArray) && dataArray.GetArrayLength() > 0)
                {
                    var first = dataArray[0];
                    destId = first.GetProperty("dest_id").GetString() ?? string.Empty;
                    searchType = first.GetProperty("search_type").GetString() ?? "CITY";
                }
            }

            if (string.IsNullOrEmpty(destId))
            {
                ViewBag.Error = "Şehir bulunamadı. Lütfen farklı bir şehir adı deneyin.";
                return View(new HotelListViewModel());
            }

            //2-Otelleri ara 
            var hotelList = new List<HotelSearchResult>();

            var hotelRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                    $"?dest_id={destId}" +
                    $"&search_type={searchType}" +
                    $"&adults={adultCount}" +
                    $"&children_age=0,17" +
                    $"&room_qty=1" +
                    $"&page_number=1" +
                    $"&units=metric" +
                    $"&temperature_unit=c" +
                    $"&languagecode=tr" +
                    $"&currency_code=EUR" +
                    $"&arrival_date={checkIn}" +
                    $"&departure_date={checkOut}" +
                    $"&location=TR"
                ),
                Headers =
                {
                    { "x-rapidapi-key", apiKey },
                    { "x-rapidapi-host", apiHost }
                }
            };

            using (var hotelResponse = await client.SendAsync(hotelRequest))
            {
                hotelResponse.EnsureSuccessStatusCode();
                var hotelBody = await hotelResponse.Content.ReadAsStringAsync();

                using var hotelJson = JsonDocument.Parse(hotelBody);
                var root = hotelJson.RootElement;

                if (root.TryGetProperty("data", out var dataObj) &&
                    dataObj.TryGetProperty("hotels", out var hotelsArray))
                {
                    foreach (var hotel in hotelsArray.EnumerateArray())
                    {
                        var prop = hotel.GetProperty("property");

                        var h = new HotelSearchResult
                        {
                            hotel_id = prop.TryGetProperty("id", out var idVal) ? idVal.GetInt32() : 0,
                            name = prop.TryGetProperty("name", out var nameVal) ? nameVal.GetString() : "-",
                            address = prop.TryGetProperty("wishlistName", out var addrVal) ? addrVal.GetString() : "-",
                            review_score = prop.TryGetProperty("reviewScore", out var scoreVal) ? scoreVal.GetDouble() : 0,
                            review_score_word = prop.TryGetProperty("reviewScoreWord", out var wordVal) ? wordVal.GetString() : "-",
                            review_nr = prop.TryGetProperty("reviewCount", out var nrVal) ? nrVal.GetInt32() : 0,
                            main_photo_url = prop.TryGetProperty("photoUrls", out var photos) && photos.GetArrayLength() > 0
                                ? photos[0].GetString() : "/images/no-hotel.jpg",
                            country_trans = prop.TryGetProperty("countryCode", out var cc) ? cc.GetString() : "-",
                        };

                        // Fiyat bilgisi
                        if (hotel.TryGetProperty("priceBreakdown", out var price) &&
                            price.TryGetProperty("grossPrice", out var gross) &&
                            gross.TryGetProperty("value", out var priceVal))
                        {
                            h.min_total_price = priceVal.GetDouble();
                        }

                        hotelList.Add(h);
                    }
                }
            }

            var viewModel = new HotelListViewModel
            {
                City = city,
                CheckIn = checkIn,
                CheckOut = checkOut,
                AdultCount = adultCount,
                Hotels = hotelList
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> HotelDetail(int id)
        {
       
            var apiKey = _configuration["RapidApi:Key"];
            var apiHost = _configuration["RapidApi:Host"];
            var client = _httpClientFactory.CreateClient();

            // Session'dan tarihleri al
            var checkIn = HttpContext.Session.GetString("checkIn") ?? DateTime.Now.ToString("yyyy-MM-dd");
            var checkOut = HttpContext.Session.GetString("checkOut") ?? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(
                    $"https://booking-com15.p.rapidapi.com/api/v1/hotels/getHotelDetails" +
                    $"?hotel_id={id}" +
                    $"&arrival_date={checkIn}" +
                    $"&departure_date={checkOut}" +
                    $"&adults=1" +
                    $"&children_age=1,17" +
                    $"&room_qty=1" +
                    $"&units=metric" +
                    $"&temperature_unit=c" +
                    $"&languagecode=tr" +
                    $"&currency_code=EUR"
                ),
                Headers =
    {
        { "x-rapidapi-key",  apiKey  },
        { "x-rapidapi-host", apiHost }
    }
            };

            var detail = new HotelDetailViewModel();

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
               

                using var json = JsonDocument.Parse(body);
                var root = json.RootElement;
                if (root.TryGetProperty("data", out var data))
                {
                    detail.HotelId = id;
                    detail.Name = data.TryGetProperty("hotel_name", out var n) ? n.GetString() : "-";
                    detail.Address = data.TryGetProperty("address", out var a) ? a.GetString() : "-";
                    detail.City = data.TryGetProperty("city", out var c) ? c.GetString() : "-";
                    detail.CountryCode = data.TryGetProperty("countrycode", out var cc) ? cc.GetString() : "-";
                    detail.Latitude = data.TryGetProperty("latitude", out var lat) ? lat.GetDouble() : 0;
                    detail.Longitude = data.TryGetProperty("longitude", out var lon) ? lon.GetDouble() : 0;
                    detail.CurrencyCode = "EUR";

                    // Puan — rawData altında
                    if (data.TryGetProperty("rawData", out var raw))
                    {
                        detail.ReviewScore = raw.TryGetProperty("reviewScore", out var rs) ? rs.GetDouble() : 0;
                        detail.ReviewScoreWord = raw.TryGetProperty("reviewScoreWord", out var rsw) ? rsw.GetString() : "";
                        detail.ReviewCount = raw.TryGetProperty("reviewCount", out var rc) ? rc.GetInt32() : 0;
                    }

                    // Fiyat
                    if (data.TryGetProperty("composite_price_breakdown", out var pb) &&
                        pb.TryGetProperty("gross_amount_per_night", out var gp) &&
                        gp.TryGetProperty("value", out var pv))
                    {
                        detail.Price = pv.GetDouble();
                    }

                    // Olanaklar — facilities_block direkt data altında
                    if (data.TryGetProperty("facilities_block", out var fb) &&
                        fb.TryGetProperty("facilities", out var facs))
                    {
                        foreach (var fac in facs.EnumerateArray().Take(10))
                        {
                            if (fac.TryGetProperty("name", out var fname))
                                detail.Facilities.Add(fname.GetString());
                        }
                    }

                    // Fotoğraflar — rooms.{roomId}.photos altında
                    if (data.TryGetProperty("rooms", out var rooms))
                    {
                        foreach (var room in rooms.EnumerateObject())
                        {
                            if (room.Value.TryGetProperty("photos", out var photos))
                            {
                                foreach (var photo in photos.EnumerateArray().Take(8))
                                {
                                    if (photo.TryGetProperty("url_max1280", out var url))
                                        detail.PhotoUrls.Add(url.GetString());
                                    else if (photo.TryGetProperty("url_max750", out var url2))
                                        detail.PhotoUrls.Add(url2.GetString());
                                }
                                break; // İlk oda yeterli
                            }
                        }
                    }

                    // Açıklama — rooms.{roomId}.description altında
                    if (data.TryGetProperty("rooms", out var roomsDesc))
                    {
                        foreach (var room in roomsDesc.EnumerateObject())
                        {
                            if (room.Value.TryGetProperty("description", out var desc))
                            {
                                detail.Description = desc.GetString();
                                break;
                            }
                        }
                    }
                }
            }

            return View(detail);
        }
    }
}