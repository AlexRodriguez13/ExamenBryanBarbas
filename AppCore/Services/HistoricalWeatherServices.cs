using AppCore.IServices;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class HistoricalWeatherServices : BaseServices<HistoricalWeather>, IHistorticalWeatherServices
    {

        IHistoricalWeatherModel Model;
        public HistoricalWeatherServices(IHistoricalWeatherModel model) : base(model)
        {
            this.Model = model;
        }

        public HistoricalWeather GetById(int id)
        {
            return Model.GetById(id);
        }

        public void Update(HistoricalWeather t)
        {
            Model.Update(t);
        }
    }
}
