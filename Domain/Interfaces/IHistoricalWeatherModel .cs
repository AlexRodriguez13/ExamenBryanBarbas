using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IHistoricalWeatherModel : IModel<HistoricalWeather>
    {
        HistoricalWeather GetById(int id);
        void Update(HistoricalWeather t);
    }
}
