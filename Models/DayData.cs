using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorDataProcessor.Models
{
    public class DayData
    {
        public DateTime Date { get; set; }
        public double Energy { get; set; }
        public double Price { get; set; }
    }
}
