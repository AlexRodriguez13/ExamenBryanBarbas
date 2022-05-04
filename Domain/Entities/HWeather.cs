using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class HWeather
    {
        public class Weather
        {
            public int id { get; set; } 
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

        public class Current
        {
            public int dt { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
            public double temp { get; set; }
            public double feels_like { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double dew_point { get; set; }
            public int uvi { get; set; }
            public int clouds { get; set; }
            public int visibility { get; set; }
            public double wind_speed { get; set; }
            public int wind_deg { get; set; }
            public double wind_gust { get; set; }
            public List<Weather> weather { get; set; }
        }

        public class Snow
        {
            public double _1h { get; set; }
        }

        public class Hourly
        {
            public int dt { get; set; }
            public double temp { get; set; }
            public double feels_like { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double dew_point { get; set; }
            public double uvi { get; set; }
            public int clouds { get; set; }
            public int visibility { get; set; }
            public double wind_speed { get; set; }
            public int wind_deg { get; set; }
            public double wind_gust { get; set; }
            public List<Weather> weather { get; set; }
            public Snow snow { get; set; }
        }

        public class root
        {
           public double lat { get; set; }
           public double lon { get; set; }
           public string timezone { get; set; }
           public string timezone_offset { get; set; }
           public Current current { get; set; }
           public List<Hourly> hourly { get; set; }


               
            

        }



    }
}
