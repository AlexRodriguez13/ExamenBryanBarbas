using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.IServices
{
    public interface IHistorticalWeatherServices : IServices<HistoricalWeather>
    {
        HistoricalWeather GetById(int id);
        void Update(HistoricalWeather t);
    }
}
