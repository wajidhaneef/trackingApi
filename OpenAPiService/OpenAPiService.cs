using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace trackingApi.OpenAPiService1
{
    
    public class OpenAPiService
    {
        private readonly HttpClient _httpClient;

        public OpenAPiService()
        {
            _httpClient = new HttpClient();
        }
        // this method fetch data from open api : currency api
        public async Task<string> GetOpenCurrency(string to, string from, string amount)
        {
            
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://currency-converter-by-api-ninjas.p.rapidapi.com/v1/convertcurrency" + "?have=" + from + "&want=" + to + "&amount=" + amount),
                Headers =
                {
                    { "X-RapidAPI-Key", "dd867142ecmsh9ebc1c43e426732p1f49b9jsnf9935f66addb" },
                    { "X-RapidAPI-Host", "currency-converter-by-api-ninjas.p.rapidapi.com" },
                },
            };
            var body = "";
            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                body = await response.Content.ReadAsStringAsync();
            }
            return body;
        }
        // this method fetch data from open api : News api
        public async Task<string> GetOpenNews(string title)
        {
            //Api keys
            //https://newsapi.org/v2/everything?q=
            //11b0760a15074aeba99c3be836328cac
            //4bd64eea2a7f4590843512926f67dde2
            // {"Authorization", "Bearer 4bd64eea2a7f4590843512926f67dde2" }
            // { "X-Api-Key", "11b0760a15074aeba99c3be836328cac" }
            //Another api 
            // https://newsdata.io/api/1/news?apikey=pub_2241410d88c2afec96b134fececf4df9a330f&q=pizza
            var encodedTitle = HttpUtility.UrlEncode(title);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://newsdata.io/api/1/news?apikey=pub_2241410d88c2afec96b134fececf4df9a330f&q=" + encodedTitle),
                Headers =
                {
                    {"X-Api-Key", "4bd64eea2a7f4590843512926f67dde2" },
                    
                },
               
            };
            var body = "";
            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                body = await response.Content.ReadAsStringAsync();
            }
            return body;
        }
        // this method fetch data from open api : Weather api
        public async Task<string> GetOpenWeather(string city)
        {
            // string apiKey = "91f41afbd0f27fe7be4259ca75c8b844";
            //string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=91f41afbd0f27fe7be4259ca75c8b844"),
                
            };
            var body = "";
            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                body = await response.Content.ReadAsStringAsync();
            }
            return body;
        }

        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
