using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutApp
{
    public class CSVEntryNotFoundException : Exception
    {
        public CSVEntryNotFoundException(string message) : base(message)
        {
        }
    }
}
