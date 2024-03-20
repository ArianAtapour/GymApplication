using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongPlayer
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public WeatherCondition Condition { get; set; }
    }
}
