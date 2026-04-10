using RoyalHotel_RapidApi.Models;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace RoyalHotel_RapidApi.Services
{
    public class ClaudeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ClaudeUrl = "https://api.anthropic.com/v1/messages";

       
        public ClaudeService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;   
            _apiKey = configuration["ApiSettings:ClaudeApiKey"];
        }

        public async Task<string> GetChefRecommendationAsync()
        {
            var requestBody = new
            {
                model = "claude-3-haiku-20240307",
                max_tokens = 200,
                messages = new[]
                {
                    new { role = "user", content = "Sen lüks bir otelin baş şefisin. Aydın ve Ege bölgesine özgü yerel bir yemek öner. " +
                         "Yanına mutlaka yakışacak yerel bir alkolsüz içecek (şerbet, ayran vb.) tavsiyesi ekle. " +
                         "Format şu olsun: 'Yemek Adı: Kısa açıklama. Yanına [İçecek] harika bir eşlikçidir.' " +
                         "Sadece bu cümleyi dön." }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, ClaudeUrl);
            request.Headers.Add("x-api-key", _apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();
        }

        public async Task<List<LocationViewModel>> GetTravelTipsAsync(string city)
        {
            var requestBody = new
            {
                model = "claude-3-haiku-20240307",
                max_tokens = 600,
                messages = new[]
                {
                    new {
                        role = "user",
                        content = $"{city} şehri için en popüler 3 turistik yeri şu JSON formatında öner: [{{ \"Name\": \"yer adı\", \"Description\": \"kısa açıklama\" }}]. " +
                                  "ÖNEMLİ: Açıklama kısmının sonuna mutlaka 'Merkeze yaklaşık [X] km uzaklıktadır.' bilgisini ekle. " +
                                  "Sadece JSON döndür, başka hiçbir metin yazma."
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, ClaudeUrl);
            request.Headers.Add("x-api-key", _apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var jsonRaw = doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();

            return JsonSerializer.Deserialize<List<LocationViewModel>>(jsonRaw);
        }
    }
}