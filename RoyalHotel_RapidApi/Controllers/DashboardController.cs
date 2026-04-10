using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RoyalHotel_RapidApi.Models;
using RoyalHotel_RapidApi.Services;

namespace RoyalHotel_RapidApi.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ClaudeService _claudeService;

       
        public DashboardController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ClaudeService claudeService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _claudeService = claudeService;
        }

        public async Task<IActionResult> Index()
        {
          
            var rapidApiKey = _configuration["ApiSettings:RapidApiKey"];
            var exchangeHost = _configuration["ApiSettings:ExchangeHost"];
            var weatherHost = _configuration["ApiSettings:WeatherHost"];

            var client = _httpClientFactory.CreateClient();

            // --- 1. DÖVİZ (USD & EUR) ---
            try
            {
                // Dolar
                var responseUsd = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://{exchangeHost}/convert?base=USD&target=TRY"),
                    Headers = { { "x-rapidapi-key", rapidApiKey }, { "x-rapidapi-host", exchangeHost } }
                });
                var bodyUsd = await responseUsd.Content.ReadAsStringAsync();
                var dataUsd = JsonConvert.DeserializeObject<ExhangeRateViewModel>(bodyUsd);
                ViewBag.usd = dataUsd?.convert_result?.rate.ToString("0.00") ?? "44.65";

                // Euro
                var responseEur = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://{exchangeHost}/convert?base=EUR&target=TRY"),
                    Headers = { { "x-rapidapi-key", rapidApiKey }, { "x-rapidapi-host", exchangeHost } }
                });
                var bodyEur = await responseEur.Content.ReadAsStringAsync();
                var dataEur = JsonConvert.DeserializeObject<ExhangeRateViewModel>(bodyEur);
                ViewBag.eur = dataEur?.convert_result?.rate.ToString("0.00") ?? "52.21";
            }
            catch { ViewBag.usd = "44.65"; ViewBag.eur = "52.21"; }


            // --- 4. KRİPTO PARA FİYATLARI ---
            try
            {
                var cryptoRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://coingecko.p.rapidapi.com/simple/price?ids=bitcoin,ethereum,binancecoin&vs_currencies=usd"),
                    Headers =
        {
            { "x-rapidapi-key", rapidApiKey },
            { "x-rapidapi-host", "coingecko.p.rapidapi.com" }
        }
                };

                var cryptoResponse = await client.SendAsync(cryptoRequest);
                var cryptoBody = await cryptoResponse.Content.ReadAsStringAsync();
                var cryptoData = JsonConvert.DeserializeObject<CryptoViewModel>(cryptoBody);

                ViewBag.btc = cryptoData?.bitcoin?.usd.ToString("N2") ?? "67,000.00";
                ViewBag.eth = cryptoData?.ethereum?.usd.ToString("N2") ?? "3,500.00";
                ViewBag.bnb = cryptoData?.binancecoin?.usd.ToString("N2") ?? "580.00";
            }
            catch
            {
                ViewBag.btc = "67,000.00";
                ViewBag.eth = "3,500.00";
                ViewBag.bnb = "580.00";
            }

            // --- 2. HAVA DURUMU ---
            try
            {
                var requestWeather = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://{weatherHost}/api/weather/current?place=Aydın&units=metric&lang=tr"),
                    Headers = {
                        { "x-rapidapi-key", rapidApiKey },
                        { "x-rapidapi-host", weatherHost }
                    }
                };
                var responseWeather = await client.SendAsync(requestWeather);
                var bodyWeather = await responseWeather.Content.ReadAsStringAsync();
                var weather = JsonConvert.DeserializeObject<WeatherViewModel>(bodyWeather);

                if (weather != null && weather.main != null)
                {
                    ViewBag.city = weather.name ?? "Aydın";
                    double rawTemp = weather.main.temprature;
                    ViewBag.temp = rawTemp > 100 ? Math.Round(rawTemp - 273.15, 0) : Math.Round(rawTemp, 0);
                    ViewBag.status = weather.weather?[0]?.description ?? "Güneşli";
                }
                else
                {
                    ViewBag.temp = "18";
                    ViewBag.status = "Parçalı Bulutlu";
                }
            }
            catch
            {
                ViewBag.temp = "18";
                ViewBag.status = "Veri Alınamadı";
            }

            // 3. YAKIT --(rapid api'den bir kanal bulunması halinde revize edilcek)
            ViewBag.benzin = "44.20"; ViewBag.motorin = "42.50"; ViewBag.lpg = "22.15";

      
            ViewBag.chefRecommend = await _claudeService.GetChefRecommendationAsync();
            var tips = await _claudeService.GetTravelTipsAsync("Aydın");

            return View(tips);
        }
    }
}