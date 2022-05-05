using Common;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public class HttpHistoricalWeatherClient : IHttpHistoricalWeatherClient
    {
        public async Task<HistoricalWeather> GetWeatherByCityNameAsync(string lat, string lon, string time)
        {
            string url = $"{AppSettings.ApiUrl}lat={lat}&lon={lon}&dt={time}&appid={AppSettings.Token}";
            string jsonObject = string.Empty;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    jsonObject = await httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();
                }

                if (string.IsNullOrEmpty(jsonObject))
                {
                    throw new NullReferenceException("El objeto json no puede ser null.");
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<HistoricalWeather>(jsonObject);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
