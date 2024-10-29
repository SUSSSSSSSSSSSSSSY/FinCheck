using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCheck
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CurrencyService
    {
        private readonly string _apiKey = "3b4d28121548ce879b636bd5ef014bf8";
        private readonly string _baseUrl = "https://api.exchangeratesapi.io/v1/latest";

        public async Task<decimal?> GetExchangeRateToUAH(string baseCurrency)
        {
            if (string.IsNullOrEmpty(baseCurrency))
                throw new ArgumentException("Base currency must be provided.");

            string apiUrl = $"{_baseUrl}?access_key={_apiKey}&base={baseCurrency}";

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<CurrencyResponse>(jsonResponse);

                    if (data?.Rates != null && data.Rates.ContainsKey("UAH"))
                    {
                        return data.Rates["UAH"];
                    }
                }
                else
                {
                    Console.WriteLine("Error: Unable to retrieve data from the API.");
                }
            }
            return null;
        }
    }

    public class CurrencyResponse
    {
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

}
