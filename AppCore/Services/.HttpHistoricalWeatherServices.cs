using AppCore.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class HttpHistoricalWeatherServices : IHttpHistoricalWeatherServices
    {
        IHttpHistoricalWeatherClient Client;

        public HttpHistoricalWeatherServices(IHttpHistoricalWeatherClient httpForeCastClient)
        {
            this.Client = httpForeCastClient;
        }
        public Task<HistoricalWeather> GetWeatherByCityNameAsync(string lat, string lon, string time)
        {
            return Client.GetWeatherByCityNameAsync(lat, lon, time);
        }
    }
}
